using NFSLibrary.RPC.XDR;
using System.Net;
using System.Net.Sockets;

namespace NFSLibrary.RPC.OncRpc
{
    /// <summary>
    /// Base class for ONC/RPC client connections.
    /// </summary>
    public class OncRpcClient : INFSRpcClient, IDisposable
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _endpoint;
        private readonly int _program;
        private readonly int _version;
        private int _xid;

        public OncRpcClient(IPAddress host, int program, int version, int port, bool useSecurePort)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endpoint = new IPEndPoint(host, port);
            _program = program;
            _version = version;
            _xid = 0;

            if (useSecurePort && port == 0)
            {
                // Use privileged port range (1-1023) for secure connections
                _socket.Bind(new IPEndPoint(IPAddress.Any, new Random().Next(1, 1024)));
            }
        }

        public void Connect()
        {
            _socket.Connect(_endpoint);
        }

        public void Disconnect()
        {
            _socket.Disconnect(true);
        }

        public void Call(int procedure, XdrAble args, XdrAble result)
        {
            // Create RPC call message
            var callInfo = new OncRpcCallInformation
            {
                Program = _program,
                Version = _version,
                Procedure = procedure,
                Parameters = args
            };

            // Encode call message
            using var xdr = new XdrBufferEncodingStream();
            EncodeCallMessage(xdr, callInfo);
            byte[] callData = xdr.GetBytes();

            // Send call message
            _socket.Send(callData);

            // Receive response
            byte[] responseData = new byte[8192]; // Adjust buffer size as needed
            int bytesRead = _socket.Receive(responseData);

            // Decode response
            using var rxdr = new XdrBufferDecodingStream(responseData.AsSpan(0, bytesRead).ToArray());
            DecodeReplyMessage(rxdr, result);
        }

        private void EncodeCallMessage(XdrEncodingStream xdr, OncRpcCallInformation callInfo)
        {
            int xid = Interlocked.Increment(ref _xid);
            xdr.XdrEncodeInt(xid); // XID
            xdr.XdrEncodeInt(0); // Call message type
            xdr.XdrEncodeInt(2); // RPC version
            xdr.XdrEncodeInt(callInfo.Program);
            xdr.XdrEncodeInt(callInfo.Version);
            xdr.XdrEncodeInt(callInfo.Procedure);
            xdr.XdrEncodeInt(0); // AUTH_NONE
            xdr.XdrEncodeInt(0); // Length 0
            xdr.XdrEncodeInt(0); // VERIFIER_NONE
            xdr.XdrEncodeInt(0); // Length 0
            callInfo.Parameters.XdrEncode(xdr);
        }

        private void DecodeReplyMessage(XdrDecodingStream xdr, XdrAble result)
        {
            int xid = xdr.XdrDecodeInt();
            int messageType = xdr.XdrDecodeInt();
            if (messageType != 1) // Reply message type
                throw new Exception("Invalid RPC reply message type");

            int replyStatus = xdr.XdrDecodeInt();
            if (replyStatus != 0) // Success status
                throw new Exception("RPC call failed");

            xdr.XdrDecodeInt(); // AUTH_NONE
            xdr.XdrDecodeInt(); // Length 0

            result.XdrDecode(xdr);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
} 