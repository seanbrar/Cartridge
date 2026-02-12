using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class CommitArgs : NFSv3Args
    {
        public NFSv3Handle Handle { get; }

        public CommitArgs(NFSv3Handle handle)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await Handle.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}