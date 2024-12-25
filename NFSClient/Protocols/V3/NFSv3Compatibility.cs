using Microsoft.Extensions.Logging;
using NFSLibrary.Protocols.Commons;

namespace NFSLibrary.Protocols.V3
{
    /// <summary>
    /// Provides compatibility with legacy NFS client code while using the new implementation.
    /// This is a temporary bridge to allow incremental modernization.
    /// </summary>
    public class NFSv3Compatibility
    {
        private readonly INFSv3Client _client;
        private readonly ILogger<NFSv3Compatibility> _logger;

        public NFSv3Compatibility(INFSv3Client client, ILogger<NFSv3Compatibility> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Creates a file handle in the legacy format.
        /// </summary>
        public async Task<byte[]> GetHandleAsync(string path)
        {
            try
            {
                var attrs = await _client.GetAttributesAsync(path);
                return attrs.Handle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get handle for {Path}", path);
                throw;
            }
        }

        /// <summary>
        /// Converts legacy mode flags to NFSPermission.
        /// </summary>
        public static NFSPermission ToNFSPermission(int mode)
        {
            return new NFSPermission(mode & 0x1FF);
        }

        /// <summary>
        /// Converts NFSPermission to legacy mode flags.
        /// </summary>
        public static int FromNFSPermission(NFSPermission permission)
        {
            return permission.Mode;
        }

        /// <summary>
        /// Gets file attributes in a format compatible with the legacy implementation.
        /// </summary>
        public async Task<(NFSAttributes Attrs, byte[] Handle)> GetAttributesWithHandleAsync(string path)
        {
            try
            {
                var attrs = await _client.GetAttributesAsync(path);
                return (attrs, attrs.Handle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attributes with handle for {Path}", path);
                throw;
            }
        }

        /// <summary>
        /// Reads directory entries in a format compatible with the legacy implementation.
        /// </summary>
        public async Task<IEnumerable<(string Name, long FileId)>> ReadDirectoryEntriesAsync(string path)
        {
            try
            {
                var entries = await _client.ReadDirectoryAsync(path);
                return entries.Select(e => (e.Name, e.FileId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read directory entries for {Path}", path);
                throw;
            }
        }

        /// <summary>
        /// Gets filesystem statistics in a format compatible with the legacy implementation.
        /// </summary>
        public async Task<(long Total, long Free, long Available)> GetStatisticsAsync(string path)
        {
            try
            {
                var stats = await _client.GetStatisticsAsync(path);
                return (stats.TotalBytes, stats.FreeBytes, stats.AvailableBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get statistics for {Path}", path);
                throw;
            }
        }

        /// <summary>
        /// Provides direct access to the modern client implementation.
        /// </summary>
        public INFSv3Client Client => _client;
    }
} 