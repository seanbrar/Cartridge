using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Data.Results;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.NFS.Core
{
    public interface INFSv3Operations
    {
        // Connection state
        NFSConnectionState ConnectionState { get; }
        Task ConnectAsync(CancellationToken ct = default);
        Task MountAsync(string exportPath, CancellationToken ct = default);
        Task UnmountAsync(CancellationToken ct = default);
        Task DisconnectAsync(CancellationToken ct = default);

        // Essential directory operations
        Task<LookupResult> LookupAsync(NFSv3Handle dirHandle, string name, CancellationToken ct = default);
        Task<ReadDirPlusResult> ReadDirPlusAsync(NFSv3Handle dirHandle, ulong cookie = 0, CancellationToken ct = default);

        // Essential file operations
        Task<ReadResult> ReadAsync(NFSv3Handle fileHandle, long offset, int count, CancellationToken ct = default);
        Task<GetAttrResult> GetAttrAsync(NFSv3Handle fileHandle, CancellationToken ct = default);

        // Basic file management (needed for write support later)
        Task<CreateResult> CreateAsync(NFSv3Handle dirHandle, string name, NFSv3SetAttributes attributes, CancellationToken ct = default);
        Task<RemoveResult> RemoveAsync(NFSv3Handle dirHandle, string name, CancellationToken ct = default);

        // Basic directory management
        Task<MkDirResult> MkDirAsync(NFSv3Handle dirHandle, string name, NFSv3SetAttributes attributes, CancellationToken ct = default);
        Task<RmDirResult> RmDirAsync(NFSv3Handle dirHandle, string name, CancellationToken ct = default);

        // Write operations
        Task<WriteResult> WriteAsync(NFSv3Handle fileHandle, long offset, ReadOnlyMemory<byte> data, CancellationToken ct = default);
        Task<CommitResult> CommitAsync(NFSv3Handle fileHandle, CancellationToken ct = default);
    }
}