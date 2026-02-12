namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR integer value.
    /// </summary>
    public sealed class XdrInt : IXdrData
    {
        public int Value { get; set; }

        public XdrInt(int value = 0) => Value = value;

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeIntAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeIntAsync();
        }

        // Implicit conversion operators
        public static implicit operator int(XdrInt value) => value.Value;
        public static implicit operator XdrInt(int value) => new(value);
    }
}