namespace Cartridge.Core.FileSystem.Cache
{
    public static class FileContextCache
    {
        private static readonly ConcurrentDictionary<string, NFSFileContext> _cache =
            new ConcurrentDictionary<string, NFSFileContext>();
        private static readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(5);

        public static bool TryGet(string path, out NFSFileContext context)
        {
            if (_cache.TryGetValue(path, out context))
            {
                if (DateTime.UtcNow - context.LastAccess > _expirationTime)
                {
                    Remove(path);
                    context = null;
                    return false;
                }
                return true;
            }
            context = null;
            return false;
        }

        public static void Add(string path, NFSFileContext context)
        {
            _cache.AddOrUpdate(path, context, (_, __) => context);
        }

        public static void Remove(string path)
        {
            _cache.TryRemove(path, out _);
        }

        // Periodic cleanup of expired entries
        private static readonly Timer _cleanupTimer = new Timer(
            _ => CleanupExpiredEntries(),
            null,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(5)
        );

        private static void CleanupExpiredEntries()
        {
            var expiredPaths = _cache
                .Where(kvp => DateTime.UtcNow - kvp.Value.LastAccess > _expirationTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var path in expiredPaths)
            {
                Remove(path);
            }
        }
    }
}