using System.Net;
using System.Net.Sockets;
using Cartridge.Core.RPC.Core;
using Cartridge.Core.RPC.XDR;
using Microsoft.Extensions.Logging;

namespace Cartridge.Core.RPC.Transport.OncRpc
{
    public class OncRpcTransport : IRpcTransport
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _endpoint;
        private readonly int _program;
        private readonly int _version;
        private readonly ILogger<OncRpcTransport> _logger;
        private int _xid;
        private volatile ConnectionState _state;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        public OncRpcTransport(
            IPAddress host,
            int program,
            int version,
            int port,
            bool useSecurePort,
            ILogger<OncRpcTransport> logger)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endpoint = new IPEndPoint(host, port);
            _program = program;
            _version = version;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (useSecurePort && port == 0)
            {
                _socket.Bind(new IPEndPoint(IPAddress.Any, new Random().Next(1, 1024)));
            }
        }

        public async ValueTask ConnectAsync(CancellationToken cancellation = default)
        {
            if (_state != ConnectionState.Disconnected)
                throw new InvalidOperationException("Already connected or connecting");

            _state = ConnectionState.Connecting;
            try
            {
                using var timeoutCts = new CancellationTokenSource(_timeout);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellation, timeoutCts.Token);

                await _socket.ConnectAsync(_endpoint, linkedCts.Token);
                _state = ConnectionState.Connected;
            }
            catch (OperationCanceledException)
            {
                _state = ConnectionState.Failed;
                throw new TimeoutException($"Connection timed out after {_timeout.TotalSeconds}s");
            }
            catch (Exception ex)
            {
                _state = ConnectionState.Failed;
                _logger.LogError(ex, "Failed to connect to RPC endpoint");
                throw;
            }
        }

        public async ValueTask DisconnectAsync(CancellationToken cancellation = default)
        {
            if (_state == ConnectionState.Connected)
            {
                try
                {
                    await _socket.DisconnectAsync(false);
                    _state = ConnectionState.Disconnected;
                }
                catch (Exception ex)
                {
                    _state = ConnectionState.Failed;
                    _logger.LogError(ex, "Failed to disconnect from RPC endpoint");
                    throw;
                }
            }
        }

        ValueTask<TResponse> IRpcTransport.CallAsync<TRequest, TResponse>(
            int procedure,
            TRequest request,
            CancellationToken cancellation)
        {
            var response = Activator.CreateInstance<TResponse>();
            return CallAsyncInternalAsync(procedure, request, response, cancellation);
        }

        private async ValueTask<TResponse> CallAsyncInternalAsync<TRequest, TResponse>(
            int procedure,
            TRequest request,
            TResponse response,
            CancellationToken cancellation)
            where TRequest : IXdrData
            where TResponse : IXdrData
        {
            ValidateConnection();

            var callInfo = new OncRpcCallInformation
            {
                Program = _program,
                Version = _version,
                Procedure = procedure,
                Parameters = request
            };

            using var timeoutCts = new CancellationTokenSource(_timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellation, timeoutCts.Token);

            try
            {
                await using var xdr = new XdrMemoryEncodingStream();
                await EncodeCallMessageAsync(xdr, callInfo);
                var callData = xdr.GetEncodedData();

                if (callData.Length > RpcConstants.MAX_PACKET_SIZE)
                    throw new RpcException($"Call data size {callData.Length} exceeds maximum packet size {RpcConstants.MAX_PACKET_SIZE}");

                await _socket.SendAsync(callData, SocketFlags.None, linkedCts.Token);

                byte[] responseData = new byte[RpcConstants.DEFAULT_BUFFER_SIZE];
                int bytesRead = await _socket.ReceiveAsync(responseData, SocketFlags.None, linkedCts.Token);

                if (bytesRead > RpcConstants.MAX_PACKET_SIZE)
                    throw new RpcException($"Response size {bytesRead} exceeds maximum packet size {RpcConstants.MAX_PACKET_SIZE}");

                await using var rxdr = new XdrMemoryDecodingStream(responseData.AsMemory(0, bytesRead));
                await DecodeReplyMessageAsync(rxdr, response);
                return response;
            }
            catch (OperationCanceledException)
            {
                _state = ConnectionState.Failed;
                throw new TimeoutException($"RPC call timed out after {_timeout.TotalSeconds}s");
            }
            catch (Exception ex)
            {
                _state = ConnectionState.Failed;
                _logger.LogError(ex, "RPC call failed");
                throw;
            }
        }

        private async Task EncodeCallMessageAsync(XdrEncodingStream xdr, OncRpcCallInformation callInfo)
        {
            int xid = Interlocked.Increment(ref _xid);
            await xdr.EncodeIntAsync(xid);
            await xdr.EncodeIntAsync(0); // Call message type
            await xdr.EncodeIntAsync(2); // RPC version
            await xdr.EncodeIntAsync(callInfo.Program);
            await xdr.EncodeIntAsync(callInfo.Version);
            await xdr.EncodeIntAsync(callInfo.Procedure);
            await xdr.EncodeIntAsync(0); // AUTH_NONE
            await xdr.EncodeIntAsync(0); // Length 0
            await xdr.EncodeIntAsync(0); // VERIFIER_NONE
            await xdr.EncodeIntAsync(0); // Length 0
            await callInfo.Parameters.XdrEncodeAsync(xdr);
        }

        private async Task DecodeReplyMessageAsync(XdrDecodingStream xdr, IXdrData result)
        {
            int xid = await xdr.DecodeIntAsync();
            int messageType = await xdr.DecodeIntAsync();
            if (messageType != 1)
                throw new RpcException("Invalid RPC reply message type");

            int replyStatus = await xdr.DecodeIntAsync();
            if (replyStatus != 0)
                throw new RpcException("RPC call failed");

            await xdr.DecodeIntAsync(); // AUTH_NONE
            await xdr.DecodeIntAsync(); // Length 0

            await result.XdrDecodeAsync(xdr);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await DisconnectAsync();
                _socket.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RPC transport");
                throw;
            }
        }

        /// <summary>
        /// Contains information about an ONC/RPC call.
        /// </summary>
        public class OncRpcCallInformation
        {
            public int Program { get; set; }
            public int Version { get; set; }
            public int Procedure { get; set; }
            public IXdrData Parameters { get; set; } = null!;
        }

        private void ValidateConnection()
        {
            if (_state != ConnectionState.Connected)
                throw new InvalidOperationException("Not connected");
        }
    }

    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Failed
    }
}