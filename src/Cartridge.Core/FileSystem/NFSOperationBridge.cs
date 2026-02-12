using Winfsp;
using Cartridge.Core.NFS.Core;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.FileSystem
{
    /// <summary>
    /// Bridges between async NFS operations and sync WinFsp callbacks
    /// </summary>
    public class NFSOperationBridge
    {
        private readonly INFSv3Operations _nfsClient;
        private readonly ILogger<NFSOperationBridge> _logger;
        private readonly SemaphoreSlim _operationLock = new SemaphoreSlim(1, 1);
        private const int MAX_RETRIES = 3;
        private const int RETRY_DELAY_MS = 500;

        public NFSOperationBridge(INFSv3Operations nfsClient, ILogger<NFSOperationBridge> logger)
        {
            _nfsClient = nfsClient;
            _logger = logger;
        }

        public async Task<(NTSTATUS Status, NFSFileContext Context)> CreateFileContextAsync(
            string path,
            CancellationToken ct = default)
        {
            // Ensure thread-safe NFS operations
            await _operationLock.WaitAsync(ct);
            try
            {
                // First try from cache if available
                if (FileContextCache.TryGet(path, out var cachedContext))
                {
                    // Validate the cached handle is still valid
                    var validateResult = await _nfsClient.GetAttrAsync(
                        cachedContext.Handle,
                        ct
                    );

                    if (validateResult.Status == NFSv3Status.NFS3_OK)
                    {
                        cachedContext.CachedAttributes = validateResult.Attributes;
                        cachedContext.LastAccess = DateTime.UtcNow;
                        return (NTSTATUS.Success, cachedContext);
                    }
                    else if (validateResult.Status == NFSv3Status.NFS3ERR_STALE)
                    {
                        // Handle is stale, remove from cache
                        FileContextCache.Remove(path);
                    }
                }

                // Perform LOOKUP with retry logic
                var lookupResult = await RetryOperationAsync(
                    async () => await _nfsClient.LookupAsync(GetParentHandle(path), GetFileName(path), ct)
                );

                if (lookupResult.Status != NFSv3Status.NFS3_OK)
                {
                    return (NFSAttributeMapper.MapNfsStatusToNTStatus(lookupResult.Status), null);
                }

                var context = new NFSFileContext(
                    lookupResult.Handle,
                    path,
                    lookupResult.Attributes.Type == NFSv3FileType.Directory
                )
                {
                    CachedAttributes = lookupResult.Attributes
                };

                FileContextCache.Add(path, context);
                return (NTSTATUS.Success, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating file context for {Path}", path);
                return (NTSTATUS.InternalError, null);
            }
            finally
            {
                _operationLock.Release();
            }
        }

        public NTSTATUS Read(
            FSP_FILE_SYSTEM fileSystem,
            NFSFileContext fileContext,
            IntPtr buffer,
            ulong offset,
            uint length,
            out uint bytesTransferred)
        {
            bytesTransferred = 0;

            try
            {
                // TODO: Implement read-ahead logic for large reads
                if (length > 32768) // 32KB threshold
                {
                    return ReadWithReadAhead(fileSystem, fileContext, buffer, offset, length, out bytesTransferred);
                }

                var result = RetryOperationAsync(
                    async () => await _nfsClient.ReadAsync(fileContext.Handle, offset, length)
                ).GetAwaiter().GetResult();

                if (result.Status != NFSv3Status.NFS3_OK)
                {
                    return NFSAttributeMapper.MapNfsStatusToNTStatus(result.Status);
                }

                Marshal.Copy(result.Data, 0, buffer, result.Data.Length);
                bytesTransferred = (uint)result.Data.Length;
                return NTSTATUS.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Read operation for {Path}", fileContext.Path);
                return NTSTATUS.InternalError;
            }
        }

        private NTSTATUS ReadWithReadAhead(
            FSP_FILE_SYSTEM fileSystem,
            NFSFileContext fileContext,
            IntPtr buffer,
            ulong offset,
            uint length,
            out uint bytesTransferred)
        {
            // Implementation for read-ahead logic
            // This would:
            // 1. Check if requested data is in read-ahead buffer
            // 2. If not, initiate async read-ahead for next chunk
            // 3. Return current chunk while next chunk is being prefetched
            throw new NotImplementedException("Read-ahead not yet implemented");
        }

        private async Task<T> RetryOperationAsync<T>(
            Func<Task<T>> operation,
            int maxRetries = MAX_RETRIES)
            where T : class, INFSv3Result
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var result = await operation();

                    if (result.Status == NFSv3Status.NFS3ERR_STALE && i < maxRetries - 1)
                    {
                        // Handle stale file handle
                        await Task.Delay(RETRY_DELAY_MS * (i + 1));
                        continue;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Operation failed, attempt {Attempt} of {MaxRetries}", i + 1, maxRetries);

                    if (i == maxRetries - 1)
                        throw;

                    await Task.Delay(RETRY_DELAY_MS * (i + 1));
                }
            }

            throw new NFSv3Exception("Operation failed after maximum retries");
        }

        private NFSv3Handle GetParentHandle(string path)
        {
            // Implementation to get parent directory handle
            throw new NotImplementedException();
        }

        private string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}