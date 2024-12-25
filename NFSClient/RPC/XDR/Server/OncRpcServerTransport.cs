using System.Net;
using NFSLibrary.RPC.OncRpc;

namespace NFSLibrary.RPC.XDR.Server
{
    public class OncRpcServerTransportRegistrationInfo
    {
        public int Program { get; }
        public int Version { get; }

        public OncRpcServerTransportRegistrationInfo(int program, int version)
        {
            Program = program;
            Version = version;
        }
    }

    public abstract class OncRpcServerTransport
    {
        protected readonly OncRpcDispatchable Dispatcher;
        protected readonly IPAddress BindAddress;
        protected readonly int Port;
        protected readonly OncRpcServerTransportRegistrationInfo[] Info;
        protected readonly int BufferSize;

        protected OncRpcServerTransport(OncRpcDispatchable dispatcher, IPAddress bindAddr, int port, OncRpcServerTransportRegistrationInfo[] info, int bufferSize)
        {
            Dispatcher = dispatcher;
            BindAddress = bindAddr;
            Port = port;
            Info = info;
            BufferSize = bufferSize;
        }
    }

    public class OncRpcUdpServerTransport : OncRpcServerTransport
    {
        public OncRpcUdpServerTransport(OncRpcDispatchable dispatcher, IPAddress bindAddr, int port, OncRpcServerTransportRegistrationInfo[] info, int bufferSize)
            : base(dispatcher, bindAddr, port, info, bufferSize)
        {
        }
    }

    public class OncRpcTcpServerTransport : OncRpcServerTransport
    {
        public OncRpcTcpServerTransport(OncRpcDispatchable dispatcher, IPAddress bindAddr, int port, OncRpcServerTransportRegistrationInfo[] info, int bufferSize)
            : base(dispatcher, bindAddr, port, info, bufferSize)
        {
        }
    }
} 