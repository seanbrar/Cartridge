namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents an XDR-serializable enumeration value.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    public sealed class XdrEnum<T> : IXdrData where T : struct, Enum
    {
        public T Value { get; set; }

        public XdrEnum(T value = default) => Value = value;

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            // XDR enums are encoded as 32-bit integers
            await xdr.EncodeIntAsync(Convert.ToInt32(Value));
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var intValue = await xdr.DecodeIntAsync();

            // Validate that the decoded value is defined in the enum
            if (!Enum.IsDefined(typeof(T), intValue))
            {
                throw new XdrException($"Invalid enum value {intValue} for type {typeof(T).Name}");
            }

            Value = (T)Enum.ToObject(typeof(T), intValue);
        }

        // Implicit conversion operators
        public static implicit operator T(XdrEnum<T> value) => value.Value;
        public static implicit operator XdrEnum<T>(T value) => new(value);

        public override string ToString() => Value.ToString();
    }
}