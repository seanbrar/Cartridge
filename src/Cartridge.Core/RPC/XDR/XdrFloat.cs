namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR floating-point (32-bit) value.
    /// </summary>
    public sealed class XdrFloat : IXdrData
    {
        public float Value { get; set; }

        public XdrFloat(float value = 0.0f) => Value = value;

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeFloatAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeFloatAsync();
        }

        // Implicit conversion operators
        public static implicit operator float(XdrFloat value) => value.Value;
        public static implicit operator XdrFloat(float value) => new(value);
    }
}