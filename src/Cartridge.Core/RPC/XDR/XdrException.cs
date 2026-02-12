namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents errors that occur during XDR encoding/decoding operations.
    /// </summary>
    public class XdrException : Exception
    {
        public XdrException(string message) : base(message) { }

        public XdrException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}