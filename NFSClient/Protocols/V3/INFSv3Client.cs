using NFSLibrary.Protocols.Commons;

namespace NFSLibrary.Protocols.V3
{
    /// <summary>
    /// Interface for NFSv3 client operations.
    /// </summary>
    public interface INFSv3Client : IDisposable
    {
        /// <summary>
        /// Mounts an NFS share.
        /// </summary>
        /// <param name="server">The NFS server address.</param>
        /// <param name="sharePath">The path to the share on the server.</param>
        /// <param name="mountPoint">The local mount point.</param>
        Task MountAsync(string server, string sharePath, string mountPoint);

        /// <summary>
        /// Unmounts an NFS share.
        /// </summary>
        /// <param name="mountPoint">The local mount point to unmount.</param>
        Task UnmountAsync(string mountPoint);

        /// <summary>
        /// Gets the attributes of a file or directory.
        /// </summary>
        Task<NFSAttributes> GetAttributesAsync(string path);

        /// <summary>
        /// Sets the attributes of a file or directory.
        /// </summary>
        Task SetAttributesAsync(string path, NFSAttributes attributes);

        /// <summary>
        /// Reads from a file.
        /// </summary>
        Task<byte[]> ReadAsync(string path, long offset, int count);

        /// <summary>
        /// Writes to a file.
        /// </summary>
        Task WriteAsync(string path, long offset, byte[] data);

        /// <summary>
        /// Creates a new file.
        /// </summary>
        Task<string> CreateAsync(string path, NFSPermission mode);

        /// <summary>
        /// Creates a new directory.
        /// </summary>
        Task CreateDirectoryAsync(string path, NFSPermission mode);

        /// <summary>
        /// Removes a file.
        /// </summary>
        Task RemoveFileAsync(string path);

        /// <summary>
        /// Removes a directory.
        /// </summary>
        Task RemoveDirectoryAsync(string path);

        /// <summary>
        /// Renames a file or directory.
        /// </summary>
        Task RenameAsync(string oldPath, string newPath);

        /// <summary>
        /// Lists the contents of a directory.
        /// </summary>
        Task<IEnumerable<NFSDirectoryEntry>> ReadDirectoryAsync(string path);

        /// <summary>
        /// Gets the filesystem statistics.
        /// </summary>
        Task<NFSStatistics> GetStatisticsAsync(string path);
    }

    /// <summary>
    /// Represents an entry in an NFS directory listing.
    /// </summary>
    public class NFSDirectoryEntry
    {
        public string Name { get; set; } = string.Empty;
        public NFSAttributes Attributes { get; set; } = null!;
        public long FileId { get; set; }
    }

    /// <summary>
    /// Represents NFS filesystem statistics.
    /// </summary>
    public class NFSStatistics
    {
        public long TotalBytes { get; set; }
        public long FreeBytes { get; set; }
        public long AvailableBytes { get; set; }
        public int TotalFileSlots { get; set; }
        public int FreeFileSlots { get; set; }
        public int AvailableFileSlots { get; set; }
        public int InvariantSeconds { get; set; }
    }
} 