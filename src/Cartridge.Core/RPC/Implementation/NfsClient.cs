using Cartridge.Core.RPC.Core;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.NFS.Data.Results;
using Cartridge.Core.NFS.Core;
using Cartridge.Core.NFS.Data.Attributes;
using Microsoft.Extensions.Logging;
using Cartridge.Core.NFS.Data.Args;

namespace Cartridge.Core.RPC.Implementation
{
    public class NfsClient : INfsClient
    {
        private readonly IRpcTransport _transport;
        private readonly ILogger<NfsClient> _logger;

        public NfsClient(IRpcTransport transport, ILogger<NfsClient> logger)
        {
            _transport = transport;
            _logger = logger;
        }

        public ValueTask ConnectAsync(CancellationToken cancellation = default)
        {
            return _transport.ConnectAsync(cancellation);
        }

        public async ValueTask<NFSv3FileAttributes> GetAttributesAsync(
            string path,
            CancellationToken cancellation = default)
        {
            try
            {
                var request = new GetAttrArgs(new NFSv3Handle(path));
                var response = await _transport.CallAsync<GetAttrArgs, GetAttrResult>(
                    (int)NFSProcedures.GetAttr,
                    request,
                    cancellation);

                return response.Attributes ?? throw new InvalidOperationException("Attributes not returned");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attributes for {Path}", path);
                throw;
            }
        }

        public async ValueTask<byte[]> ReadAsync(
            string path,
            long offset,
            int length,
            CancellationToken cancellation = default)
        {
            try
            {
                var request = new ReadArgs(new NFSv3Handle(path), offset, length);
                var response = await _transport.CallAsync<ReadArgs, ReadResult>(
                    (int)NFSProcedures.Read,
                    request,
                    cancellation);

                return response.Data.Value.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read from {Path} at offset {Offset}", path, offset);
                throw;
            }
        }

        public async ValueTask WriteAsync(
            string path,
            long offset,
            byte[] data,
            CancellationToken cancellation = default)
        {
            try
            {
                var request = new WriteArgs(new NFSv3Handle(path), offset, data);
                await _transport.CallAsync<WriteArgs, WriteResult>(
                    (int)NFSProcedures.Write,
                    request,
                    cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to {Path} at offset {Offset}", path, offset);
                throw;
            }
        }

        public async ValueTask<string[]> ReadDirectoryAsync(
            string path,
            CancellationToken cancellation = default)
        {
            try
            {
                var request = new ReadDirPlusArgs(new NFSv3Handle(path), 0);
                var response = await _transport.CallAsync<ReadDirPlusArgs, ReadDirPlusResult>(
                    (int)NFSProcedures.ReadDirPlus,
                    request,
                    cancellation);

                return response.Entries.Select(e => e.Name.Value).ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read directory {Path}", path);
                throw;
            }
        }

        public async ValueTask CreateDirectoryAsync(
            string path,
            NFSPermission mode,
            CancellationToken cancellation = default)
        {
            try
            {
                var attributes = new NFSv3SetAttributes { Mode = (uint)mode };
                var dirPath = Path.GetDirectoryName(path) ?? throw new ArgumentException("Invalid path", nameof(path));
                var dirName = Path.GetFileName(path);
                
                var request = new MkDirArgs(new NFSv3Handle(dirPath), dirName, attributes);
                await _transport.CallAsync<MkDirArgs, MkDirResult>(
                    (int)NFSProcedures.MkDir,
                    request,
                    cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create directory {Path}", path);
                throw;
            }
        }

        public async ValueTask RemoveAsync(
            string path,
            CancellationToken cancellation = default)
        {
            try
            {
                var dirPath = Path.GetDirectoryName(path) ?? throw new ArgumentException("Invalid path", nameof(path));
                var name = Path.GetFileName(path);
                
                var request = new RemoveArgs(new NFSv3Handle(dirPath), name);
                await _transport.CallAsync<RemoveArgs, RemoveResult>(
                    (int)NFSProcedures.Remove,
                    request,
                    cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove {Path}", path);
                throw;
            }
        }

        public async ValueTask RenameAsync(
            string oldPath,
            string newPath,
            CancellationToken cancellation = default)
        {
            try
            {
                var fromDirPath = Path.GetDirectoryName(oldPath) ?? throw new ArgumentException("Invalid path", nameof(oldPath));
                var fromName = Path.GetFileName(oldPath);
                var toDirPath = Path.GetDirectoryName(newPath) ?? throw new ArgumentException("Invalid path", nameof(newPath));
                var toName = Path.GetFileName(newPath);

                var request = new RenameArgs(
                    new NFSv3Handle(fromDirPath), 
                    fromName,
                    new NFSv3Handle(toDirPath), 
                    toName);

                await _transport.CallAsync<RenameArgs, RenameResult>(
                    (int)NFSProcedures.Rename,
                    request,
                    cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rename {OldPath} to {NewPath}", oldPath, newPath);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _transport.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing NFS client");
                throw;
            }
        }
    }
}