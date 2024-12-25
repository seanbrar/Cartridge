using Microsoft.Extensions.Logging;
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC;

namespace NFSLibrary.Protocols.V3
{
    /// <summary>
    /// Implementation of NFSv3 client operations.
    /// </summary>
    public class NFSv3Client : INFSv3Client
    {
        private readonly INFSRpcClient _rpcClient;
        private readonly ILogger<NFSv3Client> _logger;
        private bool _isMounted;
        private string? _currentMountPoint;

        public NFSv3Client(INFSRpcClient rpcClient, ILogger<NFSv3Client> logger)
        {
            _rpcClient = rpcClient;
            _logger = logger;
        }

        public async Task MountAsync(string server, string sharePath, string mountPoint)
        {
            try
            {
                await _rpcClient.ConnectAsync();
                await _rpcClient.InvokeAsync("Mount", new { server, sharePath, mountPoint });
                _isMounted = true;
                _currentMountPoint = mountPoint;
                _logger.LogInformation("Successfully mounted {SharePath} from {Server} to {MountPoint}", 
                    sharePath, server, mountPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mount {SharePath} from {Server} to {MountPoint}", 
                    sharePath, server, mountPoint);
                throw;
            }
        }

        public async Task UnmountAsync(string mountPoint)
        {
            if (!_isMounted || _currentMountPoint != mountPoint)
            {
                throw new InvalidOperationException($"Share is not mounted at {mountPoint}");
            }

            try
            {
                await _rpcClient.InvokeAsync("Unmount", mountPoint);
                _isMounted = false;
                _currentMountPoint = null;
                _logger.LogInformation("Successfully unmounted {MountPoint}", mountPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unmount {MountPoint}", mountPoint);
                throw;
            }
        }

        public async Task<NFSAttributes> GetAttributesAsync(string path)
        {
            EnsureMounted();
            return await _rpcClient.GetAttributesAsync(path);
        }

        public async Task SetAttributesAsync(string path, NFSAttributes attributes)
        {
            EnsureMounted();
            await _rpcClient.InvokeAsync("SetAttributes", new { path, attributes });
        }

        public async Task<byte[]> ReadAsync(string path, long offset, int count)
        {
            EnsureMounted();
            return await _rpcClient.ReadAsync(path, offset, count);
        }

        public async Task WriteAsync(string path, long offset, byte[] data)
        {
            EnsureMounted();
            await _rpcClient.WriteAsync(path, offset, data);
        }

        public async Task<string> CreateAsync(string path, NFSPermission mode)
        {
            EnsureMounted();
            var result = await _rpcClient.InvokeAsync<CreateResult>("Create", new { path, mode });
            return result.Handle;
        }

        public async Task CreateDirectoryAsync(string path, NFSPermission mode)
        {
            EnsureMounted();
            await _rpcClient.CreateDirectoryAsync(path, mode);
        }

        public async Task RemoveFileAsync(string path)
        {
            EnsureMounted();
            await _rpcClient.RemoveAsync(path);
        }

        public async Task RemoveDirectoryAsync(string path)
        {
            EnsureMounted();
            await _rpcClient.InvokeAsync("RemoveDirectory", path);
        }

        public async Task RenameAsync(string oldPath, string newPath)
        {
            EnsureMounted();
            await _rpcClient.RenameAsync(oldPath, newPath);
        }

        public async Task<IEnumerable<NFSDirectoryEntry>> ReadDirectoryAsync(string path)
        {
            EnsureMounted();
            var entries = await _rpcClient.InvokeAsync<NFSDirectoryEntry[]>("ReadDirectoryPlus", path);
            return entries;
        }

        public async Task<NFSStatistics> GetStatisticsAsync(string path)
        {
            EnsureMounted();
            return await _rpcClient.InvokeAsync<NFSStatistics>("GetStatistics", path);
        }

        private void EnsureMounted()
        {
            if (!_isMounted)
            {
                throw new InvalidOperationException("No NFS share is currently mounted");
            }
        }

        public void Dispose()
        {
            if (_isMounted && _currentMountPoint != null)
            {
                try
                {
                    UnmountAsync(_currentMountPoint).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during unmount in Dispose");
                }
            }
            _rpcClient.Dispose();
        }

        private class CreateResult
        {
            public string Handle { get; set; } = string.Empty;
        }
    }
} 