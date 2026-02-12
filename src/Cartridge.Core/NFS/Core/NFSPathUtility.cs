namespace Cartridge.Core.NFS.Core
{
    public static class NFSPathUtility
    {
        public static bool IsNFSSpecialPath(string path)
        {
            return path == "." || path == "..";
        }

        public static string NormalizeNFSPath(string path)
        {
            // Handle only NFS-specific path normalization
            // This is separate from filesystem path handling
            if (string.IsNullOrEmpty(path))
                return ".";

            // Remove duplicate separators and handle . and .. properly
            var segments = path.Split('/', '\\')
                              .Where(s => !string.IsNullOrEmpty(s))
                              .ToList();

            return segments.Any() ? string.Join("/", segments) : ".";
        }
    }
}