using NFSLibrary.RPC.XDR;
using System.Net;
using System.Net.Sockets;

namespace NFSLibrary.RPC.OncRpc
{
    /// <summary>
    /// Base class for ONC/RPC server implementations.
    /// </summary>
    public class OncRpcServer : IDisposable
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _endpoint;
        private readonly Dictionary<int, OncRpcDispatchable> _handlers;
        private readonly CancellationTokenSource _cancellation;
        private Task? _listenTask;

        public OncRpcServer(IPAddress bindAddr, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endpoint = new IPEndPoint(bindAddr, port);
            _handlers = new Dictionary<int, OncRpcDispatchable>();
            _cancellation = new CancellationTokenSource();
        }

        public void RegisterHandler(int program, OncRpcDispatchable handler)
        {
            _handlers[program] = handler;
        }

        public void Start()
        {
            _socket.Bind(_endpoint);
            _socket.Listen(10);

            _listenTask = Task.Run(ListenLoop, _cancellation.Token);
        }

        private async Task ListenLoop()
        {
            while (!_cancellation.Token.IsCancellationRequested)
            {
                try
                {
                    Socket client = await Task.Factory.FromAsync(
                        _socket.BeginAccept,
                        _socket.EndAccept,
                        null);

                    _ = HandleClientAsync(client);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Log error and continue
                }
            }
        }

        private async Task HandleClientAsync(Socket client)
        {
            try
            {
                using (client)
                {
                    byte[] buffer = new byte[8192]; // Adjust buffer size as needed
                    while (!_cancellation.Token.IsCancellationRequested)
                    {
                        int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
                        if (bytesRead == 0)
                            break;

                        using var rxdr = new XdrBufferDecodingStream(buffer.AsSpan(0, bytesRead).ToArray());
                        using var txdr = new XdrBufferEncodingStream();

                        // Decode call message
                        int xid = rxdr.XdrDecodeInt();
                        int messageType = rxdr.XdrDecodeInt();
                        if (messageType != 0) // Call message type
                            continue;

                        int rpcVersion = rxdr.XdrDecodeInt();
                        int program = rxdr.XdrDecodeInt();
                        int version = rxdr.XdrDecodeInt();
                        int procedure = rxdr.XdrDecodeInt();

                        // Skip auth info
                        rxdr.XdrDecodeInt(); // AUTH_NONE
                        rxdr.XdrDecodeInt(); // Length 0
                        rxdr.XdrDecodeInt(); // VERIFIER_NONE
                        rxdr.XdrDecodeInt(); // Length 0

                        // Find handler
                        if (_handlers.TryGetValue(program, out var handler))
                        {
                            // Process call
                            handler.DispatchOperation(procedure, rxdr, txdr);

                            // Send response
                            byte[] response = txdr.GetBytes();
                            await client.SendAsync(response, SocketFlags.None);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log error
            }
        }

        public void Stop()
        {
            _cancellation.Cancel();
            _listenTask?.Wait();
        }

        public void Dispose()
        {
            _cancellation.Cancel();
            _socket.Dispose();
            _cancellation.Dispose();
        }
    }
} 