using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cartridge.Core.NFS.Core;
using Cartridge.Core.NFS.Exceptions;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.NFS.Operations
{
    public class FileOperations
    {
        private readonly INFSv3Operations _nfs;
        private readonly IProgress<long>? _progress;

        public FileOperations(INFSv3Operations nfs, IProgress<long>? progress = null)
        {
            _nfs = nfs ?? throw new ArgumentNullException(nameof(nfs));
            _progress = progress;
        }

        public async Task ReadFileAsync(
            NFSv3Handle fileHandle,
            Stream destination,
            long offset = 0,
            CancellationToken ct = default)
        {
            var attrs = await _nfs.GetAttrAsync(fileHandle, ct);
            if (attrs.Status.Value != NFSv3Status.NFS3_OK || attrs.Attributes == null)
            {
                throw new NFSv3Exception($"Failed to get file attributes: {attrs.Status.Value}");
            }

            var fileSize = attrs.Attributes.Size.Value;
            var bytesRemaining = fileSize - offset;
            var currentOffset = offset;

            while (bytesRemaining > 0 && !ct.IsCancellationRequested)
            {
                var chunkSize = (int)Math.Min(NFSv3Constants.DEFAULT_CHUNK_SIZE, bytesRemaining);

                var readResult = await _nfs.ReadAsync(fileHandle, currentOffset, chunkSize, ct);
                if (readResult.Status.Value != NFSv3Status.NFS3_OK)
                {
                    throw new NFSv3Exception($"Read failed at offset {currentOffset}: {readResult.Status.Value}");
                }

                if (readResult.Data.Length == 0)
                {
                    // EOF reached early
                    break;
                }

                await destination.WriteAsync(readResult.Data.Value.ToArray(), 0, readResult.Data.Length, ct);

                currentOffset += readResult.Data.Length;
                bytesRemaining -= readResult.Data.Length;

                _progress?.Report(currentOffset);
            }
        }

        public async Task WriteFileAsync(
            NFSv3Handle fileHandle,
            Stream source,
            long offset = 0,
            CancellationToken ct = default)
        {
            var buffer = new byte[NFSv3Constants.DEFAULT_CHUNK_SIZE];
            var currentOffset = offset;

            while (!ct.IsCancellationRequested)
            {
                int bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, ct);
                if (bytesRead == 0)
                {
                    break; // EOF
                }

                var writeResult = await _nfs.WriteAsync(fileHandle, currentOffset, buffer.AsMemory(0, bytesRead), ct);
                if (writeResult.Status.Value != NFSv3Status.NFS3_OK)
                {
                    throw new NFSv3Exception($"Write failed at offset {currentOffset}: {writeResult.Status.Value}");
                }

                currentOffset += bytesRead;
                _progress?.Report(currentOffset);
            }

            // Ensure all writes are committed
            await _nfs.CommitAsync(fileHandle, ct);
        }
    }
}