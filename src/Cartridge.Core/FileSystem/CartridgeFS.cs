using Cartridge.Core.FileSystem;
using Cartridge.Core.FileSystem.Security;
using Cartridge.Core.NFS.Core;

namespace Cartridge.Core.FileSystem
{
    public class CartridgeFS : FileSystemBase
    {
        private readonly NFSOperationBridge _bridge;
        private readonly NFSSecurityProvider _securityProvider;

        public CartridgeFS(INFSv3Operations nfsClient, ILogger<CartridgeFS> logger)
        {
            _bridge = new NFSOperationBridge(nfsClient, logger);
            _securityProvider = new NFSSecurityProvider();
        }

        protected override bool Read(
            string fileName,
            object context,
            IntPtr buffer,
            long offset,
            int length,
            out int bytesTransferred)
        {
            var fileContext = (NFSFileContext)context;
            uint bytesOut;

            var status = _bridge.Read(
                FileSystemHandle,
                fileContext,
                buffer,
                (ulong)offset,
                (uint)length,
                out bytesOut
            );

            bytesTransferred = (int)bytesOut;
            return status == NTSTATUS.Success;
        }

        protected override bool GetVolumeInfo(
            out VolumeInfo volumeInfo)
        {
            volumeInfo = new VolumeInfo
            {
                TotalSize = 1024L * 1024 * 1024 * 1024,     // 1TB default
                FreeSize = 512L * 1024 * 1024 * 1024,       // 512GB default
                VolumeLabel = "NFS Volume"                   // Can be customized
            };

            // TODO: Implement actual NFS FSSTAT call here
            var status = _bridge.GetVolumeInfo(
                FileSystemHandle,
                out volumeInfo);

            return status == NTSTATUS.Success;
        }

        protected override bool GetSecurityByName(
            string fileName,
            out FileInfo fileInfo,
            out string securityDescriptor)
        {
            fileInfo = default;
            securityDescriptor = null;

            // Convert Windows path to NFS path and get file attributes
            var status = _bridge.GetFileAttributes(
                FileSystemHandle,
                fileName,
                out var attributes);

            if (status != NTSTATUS.Success)
                return false;

            fileInfo = new FileInfo
            {
                FileAttributes = attributes.FileAttributes,
                FileSize = attributes.Size,
                AllocationSize = (attributes.Size + AllocationUnit - 1) / AllocationUnit * AllocationUnit,
                CreationTime = attributes.CreationTime,
                LastAccessTime = attributes.LastAccessTime,
                LastWriteTime = attributes.LastWriteTime,
                ChangeTime = attributes.ChangeTime,
                IndexNumber = attributes.FileId
            };

            // Create security descriptor based on NFS attributes
            securityDescriptor = _securityProvider.CreateSecurityDescriptor(
                attributes.Uid,
                attributes.Gid,
                attributes.Mode);

            return true;
        }

        // Other overridden operations that use the bridge...
    }
}