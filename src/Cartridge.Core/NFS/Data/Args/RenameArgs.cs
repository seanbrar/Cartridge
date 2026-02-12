using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class RenameArgs : NFSv3Args
    {
        public NFSv3Handle FromDirHandle { get; }
        public XdrString FromName { get; } = new();
        public NFSv3Handle ToDirHandle { get; }
        public XdrString ToName { get; } = new();

        public RenameArgs(NFSv3Handle fromDirHandle, string fromName, NFSv3Handle toDirHandle, string toName)
        {
            FromDirHandle = fromDirHandle ?? throw new ArgumentNullException(nameof(fromDirHandle));
            FromName.Value = fromName ?? throw new ArgumentNullException(nameof(fromName));
            ToDirHandle = toDirHandle ?? throw new ArgumentNullException(nameof(toDirHandle));
            ToName.Value = toName ?? throw new ArgumentNullException(nameof(toName));
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await FromDirHandle.XdrEncodeAsync(xdr);
            await FromName.XdrEncodeAsync(xdr);
            await ToDirHandle.XdrEncodeAsync(xdr);
            await ToName.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
} 