namespace Cartridge.Core.RPC.Core
{
    public class RpcException : Exception
    {
        public RpcException(string message) : base(message) { }
        public RpcException(string message, Exception innerException) : base(message, innerException) { }
    }
} 