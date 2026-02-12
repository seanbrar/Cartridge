namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR opaque data value.
    /// Supports both fixed and variable-length byte arrays.
    /// </summary>
    public sealed class XdrOpaque : IXdrData
    {
        private Memory<byte> _value;
        
        public ReadOnlyMemory<byte> Value => _value;
        public int Length => _value.Length;
        public bool IsFixedLength { get; }

        /// <summary>
        /// Creates a new empty variable-length XdrOpaque.
        /// </summary>
        public XdrOpaque()
        {
            _value = Array.Empty<byte>();
            IsFixedLength = false;
        }

        /// <summary>
        /// Creates a new XdrOpaque with the specified data.
        /// </summary>
        public XdrOpaque(ReadOnlyMemory<byte> value, bool isFixedLength = false)
        {
            _value = value.ToArray(); // Create defensive copy
            IsFixedLength = isFixedLength;
        }

        /// <summary>
        /// Creates a new fixed-length XdrOpaque with the specified length.
        /// </summary>
        public XdrOpaque(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _value = new byte[length];
            IsFixedLength = true;
        }

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeOpaqueAsync(_value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var decoded = await xdr.DecodeOpaqueAsync();
            
            if (IsFixedLength && decoded.Length != Length)
            {
                throw new XdrException($"Fixed-length opaque expected {Length} bytes but received {decoded.Length}");
            }

            // Replace the existing memory with the new data
            _value = decoded.ToArray();
        }

        /// <summary>
        /// Creates a slice of the opaque data.
        /// </summary>
        public XdrOpaque Slice(int start, int length)
            => new XdrOpaque(_value.Slice(start, length), IsFixedLength);

        /// <summary>
        /// Creates a new XdrOpaque from a byte array.
        /// </summary>
        public static XdrOpaque FromBytes(byte[] bytes, bool isFixedLength = false)
            => new XdrOpaque(bytes, isFixedLength);

        /// <summary>
        /// Creates a new fixed-length XdrOpaque filled with zeros.
        /// </summary>
        public static XdrOpaque CreateFixed(int length)
            => new XdrOpaque(length);
    }
}