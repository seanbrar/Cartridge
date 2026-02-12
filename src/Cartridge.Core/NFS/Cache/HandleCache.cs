using System.Collections.Concurrent;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.NFS.Cache
{
    public class HandleCache
    {
        private readonly ConcurrentDictionary<string, NFSv3Handle> _handles = new();
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

        public void Add(NFSv3Handle handle)
        {
            _handles.TryAdd(handle.ToString(), handle);
        }

        public bool TryGet(string key, out NFSv3Handle? handle)
        {
            return _handles.TryGetValue(key, out handle);
        }

        public void Invalidate(string key)
        {
            _handles.TryRemove(key, out _);
        }

        public void Clear()
        {
            _handles.Clear();
        }
    }
}