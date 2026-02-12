using Cartridge.Core.NFS.Types;
using Cartridge.Core.NFS.Data.Attributes;

namespace Cartridge.Core.RPC.Core
{
    /// <summary>
    /// Defines the contract for NFS operations.
    /// </summary>
    public interface INfsClient : IAsyncDisposable
    {
        /// <summary>
        /// Establishes a connection to the NFS server.
        /// </summary>
        /// <param name="cancellation">The cancellation token.</param>
        ValueTask ConnectAsync(CancellationToken cancellation = default);

        /// <summary>
        /// Gets the attributes of a file or directory.
        /// </summary>
        /// <param name="path">The path to the file or directory.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <returns>The attributes of the file or directory.</returns>
        ValueTask<NFSv3FileAttributes> GetAttributesAsync(
            string path,
            CancellationToken cancellation = default);

        /// <summary>
        /// Reads data from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="offset">The offset in the file to start reading from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <returns>The read data.</returns>
        ValueTask<byte[]> ReadAsync(
            string path,
            long offset,
            int length,
            CancellationToken cancellation = default);

        /// <summary>
        /// Writes data to a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="offset">The offset in the file to start writing at.</param>
        /// <param name="data">The data to write.</param>
        /// <param name="cancellation">The cancellation token.</param>
        ValueTask WriteAsync(
            string path,
            long offset,
            byte[] data,
            CancellationToken cancellation = default);

        /// <summary>
        /// Lists the contents of a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <returns>An array of file and directory names in the directory.</returns>
        ValueTask<string[]> ReadDirectoryAsync(
            string path,
            CancellationToken cancellation = default);

        /// <summary>
        /// Creates a new directory.
        /// </summary>
        /// <param name="path">The path where to create the directory.</param>
        /// <param name="mode">The permissions to set on the new directory.</param>
        /// <param name="cancellation">The cancellation token.</param>
        ValueTask CreateDirectoryAsync(
            string path,
            NFSPermission mode,
            CancellationToken cancellation = default);

        /// <summary>
        /// Removes a file or directory.
        /// </summary>
        /// <param name="path">The path to the file or directory to remove.</param>
        /// <param name="cancellation">The cancellation token.</param>
        ValueTask RemoveAsync(
            string path,
            CancellationToken cancellation = default);

        /// <summary>
        /// Renames a file or directory.
        /// </summary>
        /// <param name="oldPath">The current path of the file or directory.</param>
        /// <param name="newPath">The new path for the file or directory.</param>
        /// <param name="cancellation">The cancellation token.</param>
        ValueTask RenameAsync(
            string oldPath,
            string newPath,
            CancellationToken cancellation = default);
    }
}