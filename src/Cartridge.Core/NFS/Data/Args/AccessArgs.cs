using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class AccessArgs : NFSv3Args
    {
        public NFSv3Handle FileHandle { get; }
        public XdrInt AccessToCheck { get; } = new();

        public AccessArgs(NFSv3Handle fileHandle, uint accessToCheck)
        {
            FileHandle = fileHandle ?? throw new ArgumentNullException(nameof(fileHandle));
            AccessToCheck.Value = (int)accessToCheck;
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await FileHandle.XdrEncodeAsync(xdr);
            await AccessToCheck.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}