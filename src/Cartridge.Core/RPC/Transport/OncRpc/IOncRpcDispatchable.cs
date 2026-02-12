using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.RPC.Transport.OncRpc
{
    /// <summary>
    /// Interface for ONC/RPC dispatchable operations.
    /// </summary>
    public interface IOncRpcDispatchable
    {
        /// <summary>
        /// Dispatches an RPC operation asynchronously.
        /// </summary>
        /// <param name="procedure">The procedure number to dispatch.</param>
        /// <param name="decoder">The XDR stream containing the request parameters.</param>
        /// <param name="encoder">The XDR stream to write the response to.</param>
        /// <returns>A ValueTask representing the asynchronous operation.</returns>
        ValueTask DispatchOperationAsync(int procedure, XdrDecodingStream decoder, XdrEncodingStream encoder);
    }
}