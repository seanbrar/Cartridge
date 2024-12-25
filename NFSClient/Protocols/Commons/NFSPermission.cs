namespace NFSLibrary.Protocols.Commons
{
    /// <summary>
    /// Represents Unix-style file permissions for NFS operations.
    /// </summary>
    public class NFSPermission
    {
        /// <summary>
        /// Gets the user access permissions (owner).
        /// </summary>
        public string UserAccess => GetPermissionString((Mode >> 6) & 0x7);

        /// <summary>
        /// Gets the group access permissions.
        /// </summary>
        public string GroupAccess => GetPermissionString((Mode >> 3) & 0x7);

        /// <summary>
        /// Gets the other access permissions.
        /// </summary>
        public string OtherAccess => GetPermissionString(Mode & 0x7);

        /// <summary>
        /// Gets the raw mode value.
        /// </summary>
        public int Mode { get; }

        /// <summary>
        /// Creates a new NFSPermission instance with the specified mode.
        /// </summary>
        /// <param name="mode">The Unix-style permission mode (e.g., 0644).</param>
        public NFSPermission(int mode)
        {
            Mode = mode & 0x1FF; // Only use the last 9 bits
        }

        /// <summary>
        /// Creates a new NFSPermission instance with separate user, group, and other permissions.
        /// </summary>
        /// <param name="user">User (owner) permissions (0-7)</param>
        /// <param name="group">Group permissions (0-7)</param>
        /// <param name="other">Other permissions (0-7)</param>
        public NFSPermission(int user, int group, int other)
        {
            user &= 0x7;
            group &= 0x7;
            other &= 0x7;
            Mode = (user << 6) | (group << 3) | other;
        }

        /// <summary>
        /// Creates a new NFSPermission instance from string representation (e.g., "rwxr-xr--").
        /// </summary>
        /// <param name="permissions">The permission string in rwx format.</param>
        public static NFSPermission FromString(string permissions)
        {
            if (permissions.Length != 9)
                throw new ArgumentException("Permission string must be 9 characters long", nameof(permissions));

            int mode = 0;
            for (int i = 0; i < 9; i++)
            {
                if (permissions[i] != '-')
                {
                    mode |= 1 << (8 - i);
                }
            }
            return new NFSPermission(mode);
        }

        private static string GetPermissionString(int bits)
        {
            return new string(new[]
            {
                (bits & 4) != 0 ? 'r' : '-',
                (bits & 2) != 0 ? 'w' : '-',
                (bits & 1) != 0 ? 'x' : '-'
            });
        }

        public override string ToString()
        {
            return $"{UserAccess}{GroupAccess}{OtherAccess}";
        }
    }
}