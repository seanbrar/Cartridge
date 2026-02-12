namespace Cartridge.Core.FileSystem
{
    public class NFSFileContext
    {
        public NFSv3Handle Handle { get; }
        public string Path { get; }
        public bool IsDirectory { get; }

        // Cache-related properties
        public DateTime LastAccess { get; set; }
        public NFSv3FileAttributes? CachedAttributes { get; set; }

        public NFSFileContext(NFSv3Handle handle, string path, bool isDirectory)
        {
            Handle = handle;
            Path = path;
            IsDirectory = isDirectory;
            LastAccess = DateTime.UtcNow;
        }
    }
}