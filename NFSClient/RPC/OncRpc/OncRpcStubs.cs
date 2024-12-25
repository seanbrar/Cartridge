using NFSLibrary.RPC.XDR;

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
    }

    /// <summary>
    /// Base class for ONC/RPC server stubs.
    /// </summary>
    public abstract class OncRpcServerStub
    {
        protected readonly INFSRpcClient RpcClient;

        protected OncRpcServerStub(INFSRpcClient rpcClient)
        {
            RpcClient = rpcClient;
        }
    }

    /// <summary>
    /// Interface for ONC/RPC dispatchable operations.
    /// </summary>
    public interface OncRpcDispatchable
    {
        void DispatchOperation(int operation, XdrDecodingStream xdr, XdrEncodingStream xdr2);
    }

    /// <summary>
    /// Contains information about an ONC/RPC call.
    /// </summary>
    public class OncRpcCallInformation
    {
        public int Program { get; set; }
        public int Version { get; set; }
        public int Procedure { get; set; }
        public XdrAble Parameters { get; set; } = null!;
    }
} 