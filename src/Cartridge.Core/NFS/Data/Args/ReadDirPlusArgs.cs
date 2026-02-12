using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class ReadDirPlusArgs : NFSv3Args
    {
        public NFSv3Handle DirHandle { get; }
        public XdrHyper Cookie { get; } = new();
        public XdrOpaque CookieVerf { get; } = new(8); // NFS3 cookie verifier is always 8 bytes
        public XdrInt DirCount { get; } = new();    // Limit on directory bytes
        public XdrInt MaxCount { get; } = new();    // Limit on total response size

        public ReadDirPlusArgs(NFSv3Handle dirHandle, ulong cookie, uint dirCount = 8192, uint maxCount = 16384)
        {
            DirHandle = dirHandle ?? throw new ArgumentNullException(nameof(dirHandle));
            Cookie.Value = (long)cookie;
            DirCount.Value = (int)dirCount;
            MaxCount.Value = (int)maxCount;
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await DirHandle.XdrEncodeAsync(xdr);
            await Cookie.XdrEncodeAsync(xdr);
            await CookieVerf.XdrEncodeAsync(xdr);
            await DirCount.XdrEncodeAsync(xdr);
            await MaxCount.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}