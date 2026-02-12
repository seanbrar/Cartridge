using System;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class ReadArgs : NFSv3Args
    {
        public NFSv3Handle FileHandle { get; }
        public XdrHyper Offset { get; } = new();
        public XdrInt Count { get; } = new();

        public ReadArgs(NFSv3Handle fileHandle, long offset, int count)
        {
            FileHandle = fileHandle ?? throw new ArgumentNullException(nameof(fileHandle));
            Offset.Value = offset;
            Count.Value = count;
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await FileHandle.XdrEncodeAsync(xdr);
            await Offset.XdrEncodeAsync(xdr);
            await Count.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}