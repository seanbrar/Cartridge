using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class RenameResult : NFSv3Result
    {
        public NFSv3FileAttributes? FromDirWccData { get; private set; }
        public NFSv3FileAttributes? ToDirWccData { get; private set; }

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            var hasFromDirWccData = new XdrBoolean();
            await hasFromDirWccData.XdrDecodeAsync(xdr);
            if (hasFromDirWccData.Value)
            {
                FromDirWccData = new NFSv3FileAttributes();
                await FromDirWccData.XdrDecodeAsync(xdr);
            }

            var hasToDirWccData = new XdrBoolean();
            await hasToDirWccData.XdrDecodeAsync(xdr);
            if (hasToDirWccData.Value)
            {
                ToDirWccData = new NFSv3FileAttributes();
                await ToDirWccData.XdrDecodeAsync(xdr);
            }
        }
    }
} 