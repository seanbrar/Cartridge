using NFSLibrary.Protocols.Commons;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Defines the contract for NFS RPC operations.
    /// </summary>
    public interface INFSRpcClient : IDisposable
    {
        /// <summary>
        /// Establishes a connection to the NFS server.
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Gets the attributes of a file or directory.
        /// </summary>
        /// <param name="path">The path to the file or directory.</param>
        /// <returns>The attributes of the file or directory.</returns>
        Task<NFSAttributes> GetAttributesAsync(string path);

        /// <summary>
        /// Reads data from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="offset">The offset in the file to start reading from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The read data.</returns>
        Task<byte[]> ReadAsync(string path, long offset, int length);

        /// <summary>
        /// Writes data to a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="offset">The offset in the file to start writing at.</param>
        /// <param name="data">The data to write.</param>
        Task WriteAsync(string path, long offset, byte[] data);

        /// <summary>
        /// Lists the contents of a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>An array of file and directory names in the directory.</returns>
        Task<string[]> ReadDirectoryAsync(string path);

        /// <summary>
        /// Creates a new directory.
        /// </summary>
        /// <param name="path">The path where to create the directory.</param>
        /// <param name="mode">The permissions to set on the new directory.</param>
        Task CreateDirectoryAsync(string path, NFSPermission mode);

        /// <summary>
        /// Removes a file or directory.
        /// </summary>
        /// <param name="path">The path to the file or directory to remove.</param>
        Task RemoveAsync(string path);

        /// <summary>
        /// Renames a file or directory.
        /// </summary>
        /// <param name="oldPath">The current path of the file or directory.</param>
        /// <param name="newPath">The new path for the file or directory.</param>
        Task RenameAsync(string oldPath, string newPath);
    }
} 