namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR void type (no value).
    /// </summary>
    public sealed class XdrVoid : IXdrData
    {
        // Singleton instance since XdrVoid is stateless
        public static readonly XdrVoid Instance = new();

        private XdrVoid() { }

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr) => default;
        public ValueTask XdrDecodeAsync(XdrDecodingStream xdr) => default;

        // Prevent creating new instances
        public static XdrVoid Create() => Instance;
    }
}