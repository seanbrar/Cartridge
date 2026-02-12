namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a variable-length vector of XDR-serializable elements with an optional maximum size.
    /// </summary>
    public sealed class XdrVector<T> : IXdrData where T : IXdrData, new()
    {
        private readonly List<T> _items;
        private readonly int? _maxLength;

        public IReadOnlyList<T> Items => _items;
        public int Count => _items.Count;
        public int? MaxLength => _maxLength;

        public XdrVector(int? maxLength = null)
        {
            _maxLength = maxLength;
            _items = new List<T>();
        }

        public XdrVector(IEnumerable<T> items, int? maxLength = null)
        {
            _maxLength = maxLength;
            _items = new List<T>(items);

            if (_maxLength.HasValue && _items.Count > _maxLength.Value)
                throw new ArgumentException($"Vector length exceeds maximum of {_maxLength.Value}", nameof(items));
        }

        public void Add(T item)
        {
            if (_maxLength.HasValue && _items.Count >= _maxLength.Value)
                throw new InvalidOperationException($"Cannot add item: vector length would exceed maximum of {_maxLength.Value}");

            _items.Add(item);
        }

        public T this[int index] => _items[index];

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await xdr.EncodeIntAsync(_items.Count);
            foreach (var item in _items)
            {
                await item.XdrEncodeAsync(xdr);
            }
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            _items.Clear();
            var count = await xdr.DecodeIntAsync();

            if (_maxLength.HasValue && count > _maxLength.Value)
                throw new XdrException($"Vector length {count} exceeds maximum of {_maxLength.Value}");

            for (int i = 0; i < count; i++)
            {
                var item = new T();
                await item.XdrDecodeAsync(xdr);
                _items.Add(item);
            }
        }

        /// <summary>
        /// Creates a vector from a span of items.
        /// </summary>
        public static XdrVector<T> FromSpan(ReadOnlySpan<T> items, int? maxLength = null)
        {
            if (maxLength.HasValue && items.Length > maxLength.Value)
                throw new ArgumentException($"Vector length exceeds maximum of {maxLength.Value}", nameof(items));

            return new XdrVector<T>(items.ToArray(), maxLength);
        }

        /// <summary>
        /// Tries to add an item to the vector.
        /// </summary>
        public bool TryAdd(T item)
        {
            if (_maxLength.HasValue && _items.Count >= _maxLength.Value)
                return false;

            _items.Add(item);
            return true;
        }

        /// <summary>
        /// Adds a range of items to the vector.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Creates an empty vector with the specified capacity.
        /// </summary>
        public static XdrVector<T> CreateEmpty(int capacity, int? maxLength = null)
        {
            if (maxLength.HasValue && capacity > maxLength.Value)
                throw new ArgumentException($"Initial capacity exceeds maximum length", nameof(capacity));

            var vector = new XdrVector<T>(maxLength);
            ((List<T>)vector._items).Capacity = capacity;
            return vector;
        }
    }
}