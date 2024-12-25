using Microsoft.Extensions.Logging;
using NFSLibrary.Protocols.Commons;
using NFSLibrary.Protocols.V3;
using System.Collections.Concurrent;

namespace NFSLibrary
{
    /// <summary>
    /// Bridge class to adapt the modern NFSv3Client to work with legacy Dokan code.
    /// This is a temporary solution to maintain compatibility during modernization.
    /// </summary>
    public class NFSClient
    {
        private readonly NFSv3Compatibility _compat;
        private readonly ILogger<NFSClient> _logger;
        private readonly ConcurrentDictionary<string, byte[]> _handleCache;
        private string _currentPath = string.Empty;

        public NFSClient(NFSv3Compatibility compat, ILogger<NFSClient> logger)
        {
            _compat = compat;
            _logger = logger;
            _handleCache = new ConcurrentDictionary<string, byte[]>();
        }

        public bool FileExists(string path)
        {
            try
            {
                var attrs = _compat.Client.GetAttributesAsync(path).GetAwaiter().GetResult();
                return attrs.NFSType == NFSItemTypes.Regular;
            }
            catch
            {
                return false;
            }
        }

        public bool IsDirectory(string path)
        {
            try
            {
                var attrs = _compat.Client.GetAttributesAsync(path).GetAwaiter().GetResult();
                return attrs.NFSType == NFSItemTypes.Directory;
            }
            catch
            {
                return false;
            }
        }

        public void CreateFile(string path)
        {
            var mode = NFSv3Compatibility.ToNFSPermission(0644); // rw-r--r--
            _compat.Client.CreateAsync(path, mode).GetAwaiter().GetResult();
        }

        public void DeleteFile(string path)
        {
            _compat.Client.RemoveFileAsync(path).GetAwaiter().GetResult();
            _handleCache.TryRemove(path, out _);
        }

        public void CreateDirectory(string path, NFSPermission mode)
        {
            _compat.Client.CreateDirectoryAsync(path, mode).GetAwaiter().GetResult();
        }

        public void Read(string path, long offset, ref long count, ref byte[] buffer)
        {
            var data = _compat.Client.ReadAsync(path, offset, (int)count).GetAwaiter().GetResult();
            Array.Copy(data, 0, buffer, 0, data.Length);
            count = data.Length;
        }

        public void Write(string path, long offset, uint count, byte[] buffer, out uint written)
        {
            var data = new byte[count];
            Array.Copy(buffer, 0, data, 0, count);
            _compat.Client.WriteAsync(path, offset, data).GetAwaiter().GetResult();
            written = count;
        }

        public void CompleteIO()
        {
            // No-op in modern implementation
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path) ?? string.Empty;
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string Combine(string fileName, string directory)
        {
            return Path.Combine(directory, fileName);
        }

        public void Mount(string server, string sharePath, string mountPoint)
        {
            _compat.Client.MountAsync(server, sharePath, mountPoint).GetAwaiter().GetResult();
            _currentPath = mountPoint;
        }

        public void Unmount()
        {
            if (!string.IsNullOrEmpty(_currentPath))
            {
                _compat.Client.UnmountAsync(_currentPath).GetAwaiter().GetResult();
                _currentPath = string.Empty;
            }
        }

        public (long Size, DateTime Created, DateTime Accessed, DateTime Modified) GetFileInformation(string path)
        {
            var attrs = _compat.Client.GetAttributesAsync(path).GetAwaiter().GetResult();
            return (
                attrs.Size,
                attrs.CreateDateTime,
                attrs.LastAccessedDateTime,
                attrs.ModifiedDateTime
            );
        }
    }
} 