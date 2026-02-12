using System.Collections.Concurrent;
using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Data.Results;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.NFS.Cache
{
    public class DirectoryCache
    {
        private class CacheEntry
        {
            public required List<ReadDirPlusResult.Entry> Entries { get; set; }
            public required DateTime ExpirationTime { get; set; }
            public required byte[] CookieVerf { get; set; }
            public required bool IsComplete { get; set; }  // Indicates if we have the full directory listing
            public required NFSv3FileAttributes DirAttributes { get; set; }
        }

        private readonly ConcurrentDictionary<NFSv3Handle, CacheEntry> _cache = new();
        private readonly TimeSpan _defaultTtl;
        private readonly int _maxEntriesPerDirectory;
        private readonly int _maxDirectories;

        public DirectoryCache(
            TimeSpan? defaultTtl = null,
            int maxEntriesPerDirectory = 10000,  // Reasonable limit for most use cases
            int maxDirectories = 1000)           // Limit total memory usage
        {
            _defaultTtl = defaultTtl ?? TimeSpan.FromMinutes(5);
            _maxEntriesPerDirectory = maxEntriesPerDirectory;
            _maxDirectories = maxDirectories;
        }

        public void Update(
            NFSv3Handle dirHandle,
            IEnumerable<ReadDirPlusResult.Entry> newEntries,
            byte[] cookieVerf,
            NFSv3FileAttributes dirAttributes,
            bool isComplete)
        {
            var entry = _cache.GetOrAdd(dirHandle, _ => new CacheEntry
            {
                Entries = new List<ReadDirPlusResult.Entry>(),
                ExpirationTime = DateTime.UtcNow.Add(_defaultTtl),
                CookieVerf = cookieVerf,
                IsComplete = false,
                DirAttributes = dirAttributes
            });

            lock (entry) // Ensure thread-safe updates to the entry's contents
            {
                // If cookie verifier changed, the directory was modified
                if (!entry.CookieVerf.SequenceEqual(cookieVerf))
                {
                    entry.Entries.Clear();
                }

                entry.Entries.AddRange(newEntries);
                entry.CookieVerf = cookieVerf;
                entry.IsComplete = isComplete;
                entry.DirAttributes = dirAttributes;
                entry.ExpirationTime = DateTime.UtcNow.Add(_defaultTtl);

                // Enforce entry limit
                if (entry.Entries.Count > _maxEntriesPerDirectory)
                {
                    entry.Entries = entry.Entries.Take(_maxEntriesPerDirectory).ToList();
                    entry.IsComplete = false; // Mark as incomplete since we truncated
                }
            }

            // Enforce directory limit through LRU-like eviction
            if (_cache.Count > _maxDirectories)
            {
                var oldest = _cache
                    .OrderBy(kvp => kvp.Value.ExpirationTime)
                    .Take(_cache.Count - _maxDirectories)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var handle in oldest)
                {
                    _cache.TryRemove(handle, out _);
                }
            }
        }

        public bool TryGetEntries(
            NFSv3Handle dirHandle,
            out IReadOnlyList<ReadDirPlusResult.Entry>? entries,
            out byte[]? cookieVerf,
            out NFSv3FileAttributes? dirAttributes,
            out bool isComplete)
        {
            entries = null;
            cookieVerf = null;
            dirAttributes = null;
            isComplete = false;

            if (_cache.TryGetValue(dirHandle, out var entry))
            {
                if (DateTime.UtcNow < entry.ExpirationTime)
                {
                    lock (entry)
                    {
                        entries = entry.Entries.ToList().AsReadOnly(); // Return a copy to prevent external modifications
                        cookieVerf = entry.CookieVerf;
                        dirAttributes = entry.DirAttributes;
                        isComplete = entry.IsComplete;
                        return true;
                    }
                }

                // Entry expired, remove it
                _cache.TryRemove(dirHandle, out _);
            }

            return false;
        }

        public bool TryGetEntry(
            NFSv3Handle dirHandle,
            string name,
            out ReadDirPlusResult.Entry? entry)
        {
            entry = null;

            if (_cache.TryGetValue(dirHandle, out var cacheEntry))
            {
                if (DateTime.UtcNow < cacheEntry.ExpirationTime)
                {
                    lock (cacheEntry)
                    {
                        entry = cacheEntry.Entries.FirstOrDefault(e => e.Name == name);
                        return entry != null;
                    }
                }

                // Entry expired, remove it
                _cache.TryRemove(dirHandle, out _);
            }

            return false;
        }

        public void Invalidate(NFSv3Handle dirHandle)
        {
            _cache.TryRemove(dirHandle, out _);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        // Statistics for monitoring
        public (int DirectoryCount, int TotalEntries) GetStats()
        {
            var totalEntries = 0;
            foreach (var entry in _cache.Values)
            {
                lock (entry)
                {
                    totalEntries += entry.Entries.Count;
                }
            }
            return (_cache.Count, totalEntries);
        }
    }
}