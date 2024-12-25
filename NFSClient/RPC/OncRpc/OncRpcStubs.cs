using NFSClient.RPC.XDR;
using System.Net;

namespace NFSLibrary.RPC.OncRpc
{
    /// <summary>
    /// Base class for ONC/RPC client stubs.
    /// </summary>
    public abstract class OncRpcClientStub
    {
        protected readonly INFSRpcClient RpcClient;

        protected OncRpcClientStub(INFSRpcClient rpcClient)
        {
            RpcClient = rpcClient;
        }

        protected OncRpcClientStub(IPAddress host, int program, int version, int port, int protocol, bool useSecurePort)
        {
            RpcClient = new OncRpcClient(host, program, version, port, useSecurePort);
        }
    }

    /// <summary>
    /// Base class for ONC/RPC server stubs.
    /// </summary>
    public abstract class OncRpcServerStub
    {
        protected readonly IPAddress BindAddress;
        protected readonly int Port;

        protected OncRpcServerStub(IPAddress bindAddr, int port)
        {
            BindAddress = bindAddr;
            Port = port;
        }
    }

    /// <summary>
    /// Interface for ONC/RPC dispatchable operations.
    /// </summary>
    public interface OncRpcDispatchable
    {
        void DispatchOperation(int procedure, XdrDecodingStream xdr, XdrEncodingStream xdr2);
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
} 