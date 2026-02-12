namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR short (16-bit) integer value.
    /// Note: XDR protocol always encodes integers as 32-bit, but this type maintains 16-bit value semantics.
    /// </summary>
    public sealed class XdrShort : IXdrData
    {
        public short Value { get; set; }

        public XdrShort(short value = 0) => Value = value;

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeIntAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var value = await xdr.DecodeIntAsync();
            if (value < short.MinValue || value > short.MaxValue)
            {
                throw new XdrException($"Value {value} is outside the valid range for a short");
            }
            Value = (short)value;
        }
            
        // Implicit conversion operators
        public static implicit operator short(XdrShort value) => value.Value;
        public static implicit operator XdrShort(short value) => new(value);
    }
}