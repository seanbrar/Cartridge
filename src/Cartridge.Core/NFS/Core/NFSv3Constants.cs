namespace Cartridge.Core.NFS.Core
{
    public static class NFSv3Constants
    {
        // NFSv3 Program and Version
        public const int NFS_PROGRAM = 100003;
        public const int NFS_V3 = 3;

        // NFSv3 Procedure Numbers (as per RFC 1813)
        public const int NFSPROC3_NULL = 0;
        public const int NFSPROC3_GETATTR = 1;
        public const int NFSPROC3_SETATTR = 2;
        public const int NFSPROC3_LOOKUP = 3;
        public const int NFSPROC3_ACCESS = 4;
        public const int NFSPROC3_READLINK = 5;
        public const int NFSPROC3_READ = 6;
        public const int NFSPROC3_WRITE = 7;
        public const int NFSPROC3_CREATE = 8;
        public const int NFSPROC3_MKDIR = 9;
        public const int NFSPROC3_SYMLINK = 10;
        public const int NFSPROC3_MKNOD = 11;
        public const int NFSPROC3_REMOVE = 12;
        public const int NFSPROC3_RMDIR = 13;
        public const int NFSPROC3_RENAME = 14;
        public const int NFSPROC3_LINK = 15;
        public const int NFSPROC3_READDIR = 16;
        public const int NFSPROC3_READDIRPLUS = 17;
        public const int NFSPROC3_FSSTAT = 18;
        public const int NFSPROC3_FSINFO = 19;
        public const int NFSPROC3_PATHCONF = 20;
        public const int NFSPROC3_COMMIT = 21;

        // Operation constants
        public const int DEFAULT_CHUNK_SIZE = 32768; // 32KB default chunk size for read/write operations

        // Access bits for ACCESS procedure
        public const int ACCESS_READ = 0x0001;
        public const int ACCESS_LOOKUP = 0x0002;
        public const int ACCESS_MODIFY = 0x0004;
        public const int ACCESS_EXTEND = 0x0008;
        public const int ACCESS_DELETE = 0x0010;
        public const int ACCESS_EXECUTE = 0x0020;

        // Transfer related
        public const int DEFAULT_READ_SIZE = 32768;  // 32KB
        public const int DEFAULT_WRITE_SIZE = 32768; // 32KB
        public const int MAXIMUM_READ_SIZE = 1048576; // 1MB
        public const int MAXIMUM_WRITE_SIZE = 1048576; // 1MB

        // Write stability
        public const int WRITE_UNSTABLE = 0;
        public const int WRITE_DATA_SYNC = 1;
        public const int WRITE_FILE_SYNC = 2;

        // Operation timeouts
        public const int QUICK_RPC_TIMEOUT = 5000;  // 5 seconds
        public const int DEFAULT_RPC_TIMEOUT = 30000; // 30 seconds
        public const int EXTENDED_RPC_TIMEOUT = 120000; // 2 minutes
    }
}