using StreamJsonRpc;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Extension methods for INFSRpcClient to support additional RPC operations.
    /// </summary>
    public static class NFSRpcClientExtensions
    {
        /// <summary>
        /// Invokes an RPC method with parameters.
        /// </summary>
        public static async Task<T> InvokeAsync<T>(this INFSRpcClient client, string method, object? args = null)
        {
            if (client is NFSRpcClient rpcClient)
            {
                return await rpcClient.InvokeInternalAsync<T>(method, args);
            }
            throw new InvalidOperationException("Client does not support raw RPC invocation");
        }

        /// <summary>
        /// Invokes an RPC method with parameters.
        /// </summary>
        public static async Task InvokeAsync(this INFSRpcClient client, string method, object? args = null)
        {
            if (client is NFSRpcClient rpcClient)
            {
                await rpcClient.InvokeInternalAsync(method, args);
                return;
            }
            throw new InvalidOperationException("Client does not support raw RPC invocation");
        }
    }

    // Add internal methods to NFSRpcClient to support the extensions
    public partial class NFSRpcClient
    {
        internal async Task<T> InvokeInternalAsync<T>(string method, object? args)
        {
            try
            {
                return args != null 
                    ? await _rpc.InvokeAsync<T>(method, args)
                    : await _rpc.InvokeAsync<T>(method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invoke RPC method {Method}", method);
                throw;
            }
        }

        internal async Task InvokeInternalAsync(string method, object? args)
        {
            try
            {
                if (args != null)
                {
                    await _rpc.InvokeAsync(method, args);
                }
                else
                {
                    await _rpc.InvokeAsync(method);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invoke RPC method {Method}", method);
                throw;
            }
        }
    }
} 