using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC.OncRpc
{
    /// <summary>
    /// Interface for NFS RPC client operations.
    /// </summary>
    public interface INFSRpcClient
    {
        /// <summary>
        /// Executes an RPC call.
        /// </summary>
        /// <param name="procedure">The procedure number to call.</param>
        /// <param name="args">The arguments to pass to the procedure.</param>
        /// <param name="result">The result object to populate.</param>
        void Call(int procedure, XdrAble args, XdrAble result);

        /// <summary>
        /// Connects to the RPC server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the RPC server.
        /// </summary>
        void Disconnect();
    }
} 