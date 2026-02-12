namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR string value.
    /// </summary>
    public sealed class XdrString : IXdrData
    {
        public string Value { get; set; }

        public XdrString(string value = "")
        {
            Value = value ?? string.Empty;
        }

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeStringAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeStringAsync();
        }

        // Implicit conversion operators
        public static implicit operator string(XdrString value) => value.Value;
        public static implicit operator XdrString(string value) => new(value);

        public override string ToString() => Value;
    }
}