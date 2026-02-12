namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a fixed-length array of XDR-serializable elements.
    /// </summary>
    public sealed class XdrArray<T> : IXdrData where T : IXdrData, new()
    {
        private readonly T[] _items;

        public IReadOnlyList<T> Items => _items;
        public int Length => _items.Length;

        public XdrArray(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _items = new T[length];
            for (int i = 0; i < length; i++)
            {
                _items[i] = new T();
            }
        }

        public XdrArray(IEnumerable<T> items, int expectedLength)
        {
            _items = items.ToArray();
            if (_items.Length != expectedLength)
                throw new ArgumentException($"Array length must be exactly {expectedLength}", nameof(items));
        }

        public T this[int index] => _items[index];

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            foreach (var item in _items)
            {
                await item.XdrEncodeAsync(xdr);
            }
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                await _items[i].XdrDecodeAsync(xdr);
            }
        }

        /// <summary>
        /// Creates a new XdrArray from a span of items.
        /// </summary>
        public static XdrArray<T> FromSpan(ReadOnlySpan<T> items, int expectedLength)
        {
            if (items.Length != expectedLength)
                throw new ArgumentException($"Array length must be exactly {expectedLength}", nameof(items));

            var array = new T[expectedLength];
            items.CopyTo(array);
            return new XdrArray<T>(array, expectedLength);
        }

        /// <summary>
        /// Copies the array elements to the specified span.
        /// </summary>
        public void CopyTo(Span<T> destination)
        {
            _items.CopyTo(destination);
        }

        /// <summary>
        /// Creates a new array with the specified factory function.
        /// </summary>
        public static XdrArray<T> Create(int length, Func<int, T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var items = new T[length];
            for (int i = 0; i < length; i++)
            {
                items[i] = factory(i);
            }
            return new XdrArray<T>(items, length);
        }
    }
}