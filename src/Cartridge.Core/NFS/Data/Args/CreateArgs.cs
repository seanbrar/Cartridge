using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class CreateArgs : NFSv3Args
    {
        public NFSv3Handle DirHandle { get; }
        public XdrString Name { get; } = new();
        public XdrInt CreateMode { get; } = new();
        public NFSv3SetAttributes Attributes { get; }

        public CreateArgs(NFSv3Handle dirHandle, string name, NFSv3SetAttributes attributes)
        {
            DirHandle = dirHandle ?? throw new ArgumentNullException(nameof(dirHandle));
            Name.Value = name ?? throw new ArgumentNullException(nameof(name));
            Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await DirHandle.XdrEncodeAsync(xdr);
            await Name.XdrEncodeAsync(xdr);
            await CreateMode.XdrEncodeAsync(xdr);
            await Attributes.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}

