namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR double-precision (64-bit) floating-point value.
    /// </summary>
    public sealed class XdrDouble : IXdrData
    {
        public double Value { get; set; }

        public XdrDouble(double value = 0.0) => Value = value;

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeDoubleAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeDoubleAsync();
        }

        // Implicit conversion operators
        public static implicit operator double(XdrDouble value) => value.Value;
        public static implicit operator XdrDouble(double value) => new(value);
    }
}