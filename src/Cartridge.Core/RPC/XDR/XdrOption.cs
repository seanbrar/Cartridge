namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents an optional XDR-serializable value.
    /// </summary>
    public sealed class XdrOption<T> : IXdrData where T : IXdrData, new()
    {
        private T? _value;

        public bool HasValue => _value != null;
        public T Value => _value ?? throw new InvalidOperationException("Option has no value");

        public XdrOption()
        {
            _value = default;
        }

        public XdrOption(T? value)
        {
            _value = value;
        }

        public void SetValue(T value)
        {
            _value = value;
        }

        public void Clear()
        {
            _value = default;
        }

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await xdr.EncodeBooleanAsync(HasValue);
            if (HasValue)
            {
                await _value!.XdrEncodeAsync(xdr);
            }
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var hasValue = await xdr.DecodeBooleanAsync();
            if (hasValue)
            {
                _value = new T();
                await _value.XdrDecodeAsync(xdr);
            }
            else
            {
                _value = default;
            }
        }

        // Implicit conversion for convenience
        public static implicit operator XdrOption<T>(T? value) => new(value);
    }
}