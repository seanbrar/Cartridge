using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Types
{
    public enum NFSv3FileType
    {
        NF3REG = 1,    // Regular File
        NF3DIR = 2,    // Directory
        NF3BLK = 3,    // Block Special
        NF3CHR = 4,    // Character Special
        NF3LNK = 5,    // Symbolic Link
        NF3SOCK = 6,   // Socket
        NF3FIFO = 7    // Named Pipe
    }

    public struct NFSv3Time : IXdrData
    {
        public uint Seconds { get; set; }
        public uint NanoSeconds { get; set; }

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await xdr.EncodeIntAsync((int)Seconds);
            await xdr.EncodeIntAsync((int)NanoSeconds);
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Seconds = (uint)await xdr.DecodeIntAsync();
            NanoSeconds = (uint)await xdr.DecodeIntAsync();
        }
    }

    public enum NFSConnectionState
    {
        Disconnected,
        Connected,
        Mounted
    }

    public enum NFSProcedures
    {
        Null = 0,
        GetAttr = 1,
        Read = 6,
        Write = 7,
        Create = 8,
        MkDir = 9,
        Remove = 12,
        Rename = 14,
        ReadDir = 16,
        ReadDirPlus = 17
    }

    [Flags]
    public enum NFSPermission : uint
    {
        None = 0,
        
        // User permissions (owner)
        UserRead = 0x100,      // 400 octal
        UserWrite = 0x080,     // 200 octal
        UserExecute = 0x040,   // 100 octal
        
        // Group permissions
        GroupRead = 0x020,     // 040 octal
        GroupWrite = 0x010,    // 020 octal
        GroupExecute = 0x008,  // 010 octal
        
        // Others permissions
        OtherRead = 0x004,     // 004 octal
        OtherWrite = 0x002,    // 002 octal
        OtherExecute = 0x001,  // 001 octal

        // Common combinations
        Default = UserRead | UserWrite | GroupRead | OtherRead,  // 0644 octal
        ExecutableDefault = UserRead | UserWrite | UserExecute | GroupRead | GroupExecute | OtherRead | OtherExecute,  // 0755 octal
        AllRead = UserRead | GroupRead | OtherRead,     // 0444 octal
        AllWrite = UserWrite | GroupWrite | OtherWrite, // 0222 octal
        AllExecute = UserExecute | GroupExecute | OtherExecute, // 0111 octal
        All = UserRead | UserWrite | UserExecute | GroupRead | GroupWrite | GroupExecute | OtherRead | OtherWrite | OtherExecute // 0777 octal
    }
}