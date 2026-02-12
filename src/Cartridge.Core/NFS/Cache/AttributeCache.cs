using System.Collections.Concurrent;
using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.NFS.Cache
{
    public class AttributeCache
    {
        private class CacheEntry
        {
            public required NFSv3FileAttributes Attributes { get; set; }
            public required DateTime ExpirationTime { get; set; }
        }

        private readonly ConcurrentDictionary<NFSv3Handle, CacheEntry> _cache = new();
        private readonly TimeSpan _defaultTtl;

        public AttributeCache(TimeSpan? defaultTtl = null)
        {
            _defaultTtl = defaultTtl ?? TimeSpan.FromMinutes(5);
        }

        public void Update(NFSv3Handle handle, NFSv3FileAttributes attributes)
        {
            var entry = new CacheEntry
            {
                Attributes = attributes,
                ExpirationTime = DateTime.UtcNow.Add(_defaultTtl)
            };

            _cache.AddOrUpdate(handle, entry, (_, _) => entry);
        }

        public bool TryGet(NFSv3Handle handle, out NFSv3FileAttributes? attributes)
        {
            attributes = null;

            if (_cache.TryGetValue(handle, out var entry))
            {
                if (DateTime.UtcNow < entry.ExpirationTime)
                {
                    attributes = entry.Attributes;
                    return true;
                }

                // Entry expired, remove it
                _cache.TryRemove(handle, out _);
            }

            return false;
        }

        public void Invalidate(NFSv3Handle handle)
        {
            _cache.TryRemove(handle, out _);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        // Optional: Method to adjust TTL for specific entries
        public void UpdateTtl(NFSv3Handle handle, TimeSpan newTtl)
        {
            if (_cache.TryGetValue(handle, out var entry))
            {
                entry.ExpirationTime = DateTime.UtcNow.Add(newTtl);
            }
        }

        // Optional: Method to extend TTL for specific entries
        public void ExtendTtl(NFSv3Handle handle, TimeSpan extension)
        {
            if (_cache.TryGetValue(handle, out var entry))
            {
                entry.ExpirationTime = entry.ExpirationTime.Add(extension);
            }
        }
    }
}