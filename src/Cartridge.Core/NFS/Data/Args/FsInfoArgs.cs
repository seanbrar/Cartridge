using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class FsInfoArgs : NFSv3Args
    {
        public NFSv3Handle RootHandle { get; }

        public FsInfoArgs(NFSv3Handle rootHandle)
        {
            RootHandle = rootHandle ?? throw new ArgumentNullException(nameof(rootHandle));
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await RootHandle.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}