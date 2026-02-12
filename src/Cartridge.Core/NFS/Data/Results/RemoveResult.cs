using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class RemoveResult : NFSv3Result
    {
        public NFSv3FileAttributes? DirWccData { get; private set; }

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            var hasDirWccData = new XdrBoolean();
            await hasDirWccData.XdrDecodeAsync(xdr);
            if (hasDirWccData.Value)
            {
                DirWccData = new NFSv3FileAttributes();
                await DirWccData.XdrDecodeAsync(xdr);
            }
        }
    }
}