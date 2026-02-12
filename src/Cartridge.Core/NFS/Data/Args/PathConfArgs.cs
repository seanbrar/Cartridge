using System;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;


namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class PathConfArgs : NFSv3Args
    {
        public NFSv3Handle FileHandle { get; }

        public PathConfArgs(NFSv3Handle fileHandle)
        {
            FileHandle = fileHandle;
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await FileHandle.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}