using System.Net;
using System.Net.Sockets;
using Cartridge.Core.RPC.XDR;
using Microsoft.Extensions.Logging;

namespace Cartridge.Core.RPC.Transport.OncRpc
{
    /// <summary>
    /// ONC/RPC server implementation.
    /// </summary>
    public sealed class OncRpcServer : IAsyncDisposable
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _endpoint;
        private readonly Dictionary<int, IOncRpcDispatchable> _handlers;
        private readonly CancellationTokenSource _cancellation;
        private readonly ILogger<OncRpcServer> _logger;
        private Task? _listenTask;

        public OncRpcServer(
            IPAddress bindAddr,
            int port,
            ILogger<OncRpcServer> logger)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endpoint = new IPEndPoint(bindAddr, port);
            _handlers = new Dictionary<int, IOncRpcDispatchable>();
            _cancellation = new CancellationTokenSource();
            _logger = logger;
        }

        public void RegisterHandler(int program, IOncRpcDispatchable handler)
        {
            _handlers[program] = handler;
        }

        public void Start()
        {
            try
            {
                _socket.Bind(_endpoint);
                _socket.Listen(10);
                _listenTask = ListenLoopAsync(_cancellation.Token);
                _logger.LogInformation("RPC server started on {Endpoint}", _endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start RPC server on {Endpoint}", _endpoint);
                throw;
            }
        }

        private async Task ListenLoopAsync(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    Socket client = await _socket.AcceptAsync(cancellation);
                    _ = HandleClientAsync(client, cancellation);
                }
                catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error accepting client connection");
                }
            }
        }

        private async Task HandleClientAsync(Socket client, CancellationToken cancellation)
        {
            try
            {
                using (client)
                {
                    byte[] buffer = new byte[8192];
                    while (!cancellation.IsCancellationRequested)
                    {
                        int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None, cancellation);
                        if (bytesRead == 0)
                            break;

                        await using var rxdr = new XdrBufferDecodingStream(buffer.AsSpan(0, bytesRead).ToArray());
                        await using var txdr = new XdrBufferEncodingStream();

                        // Decode call message
                        var xid = await rxdr.DecodeIntAsync();
                        var messageType = await rxdr.DecodeIntAsync();
                        if (messageType != 0)
                        {
                            _logger.LogWarning("Received invalid message type: {MessageType}", messageType);
                            continue;
                        }

                        var rpcVersion = await rxdr.DecodeIntAsync();
                        var program = await rxdr.DecodeIntAsync();
                        var version = await rxdr.DecodeIntAsync();
                        var procedure = await rxdr.DecodeIntAsync();

                        // Skip auth info
                        await rxdr.DecodeIntAsync(); // AUTH_NONE
                        await rxdr.DecodeIntAsync(); // Length 0
                        await rxdr.DecodeIntAsync(); // VERIFIER_NONE
                        await rxdr.DecodeIntAsync(); // Length 0

                        if (_handlers.TryGetValue(program, out var handler))
                        {
                            try
                            {
                                await handler.DispatchOperationAsync(procedure, rxdr, txdr);
                                var response = txdr.ToArray();
                                await client.SendAsync(response, SocketFlags.None, cancellation);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing RPC call: Program={Program}, Procedure={Procedure}",
                                    program, procedure);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("No handler registered for program: {Program}", program);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling client connection");
            }
        }

        public async ValueTask StopAsync()
        {
            try
            {
                await _cancellation.CancelAsync();
                if (_listenTask != null)
                {
                    await Task.WhenAny(_listenTask).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping RPC server");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await StopAsync();
                _socket.Dispose();
                _cancellation.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RPC server");
                throw;
            }
        }
    }
}