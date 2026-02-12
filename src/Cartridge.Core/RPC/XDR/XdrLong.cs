namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR long (64-bit) integer value.
    /// </summary>
    public sealed class XdrLong : IXdrData
    {
        public long Value { get; set; }

        public XdrLong(long value = 0) => Value = value;

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeLongAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeLongAsync();
        }

        // Implicit conversion operators
        public static implicit operator long(XdrLong value) => value.Value;
        public static implicit operator XdrLong(long value) => new(value);
    }
}