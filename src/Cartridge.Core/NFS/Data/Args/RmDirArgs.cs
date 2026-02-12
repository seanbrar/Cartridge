using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class RmDirArgs : NFSv3Args
    {
        public NFSv3Handle DirHandle { get; }
        public XdrString Name { get; } = new();

        public RmDirArgs(NFSv3Handle dirHandle, string name)
        {
            DirHandle = dirHandle ?? throw new ArgumentNullException(nameof(dirHandle));
            Name.Value = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await DirHandle.XdrEncodeAsync(xdr);
            await Name.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}