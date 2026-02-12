using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.FileSystem
{
    public static class NFSAttributeMapper
    {
        public static FileInfo ToFileInfo(NFSv3FileAttributes attrs)
        {
            return new FileInfo
            {
                FileAttributes = ToWindowsAttributes(attrs),
                FileSize = attrs.Size,
                AllocationSize = (attrs.Size + 4095) & ~4095, // Round up to 4K
                CreationTime = DateTimeOffset.FromUnixTimeSeconds(attrs.CreateTime).ToFileTime(),
                LastAccessTime = DateTimeOffset.FromUnixTimeSeconds(attrs.AccessTime).ToFileTime(),
                LastWriteTime = DateTimeOffset.FromUnixTimeSeconds(attrs.ModifyTime).ToFileTime(),
                ChangeTime = DateTimeOffset.FromUnixTimeSeconds(attrs.ModifyTime).ToFileTime(),
                IndexNumber = 0 // Consider using a hash of the NFS handle
            };
        }

        public static FileAttributes ToWindowsAttributes(NFSv3FileAttributes attrs)
        {
            var result = FileAttributes.Normal;

            if (attrs.Type == NFSv3FileType.Directory)
                result |= FileAttributes.Directory;
            if (attrs.Type == NFSv3FileType.Regular)
                result |= FileAttributes.Archive;
            if (!attrs.IsWritable)
                result |= FileAttributes.ReadOnly;

            return result;
        }

        public static NTSTATUS MapNfsStatusToNTStatus(NFSv3Status status)
        {
            return status switch
            {
                NFSv3Status.NFS3_OK => NTSTATUS.Success,
                NFSv3Status.NFS3ERR_NOENT => NTSTATUS.ObjectNameNotFound,
                NFSv3Status.NFS3ERR_IO => NTSTATUS.DeviceNotReady,
                NFSv3Status.NFS3ERR_NXIO => NTSTATUS.NoSuchDevice,
                NFSv3Status.NFS3ERR_ACCES => NTSTATUS.AccessDenied,
                NFSv3Status.NFS3ERR_EXIST => NTSTATUS.ObjectNameCollision,
                NFSv3Status.NFS3ERR_NOTDIR => NTSTATUS.NotADirectory,
                NFSv3Status.NFS3ERR_ISDIR => NTSTATUS.FileIsADirectory,
                NFSv3Status.NFS3ERR_STALE => NTSTATUS.ObjectNoLongerExists,
                _ => NTSTATUS.InternalError
            };
        }
    }
}