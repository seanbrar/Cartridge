using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class FsInfoResult : NFSv3Result
    {
        public NFSv3FileAttributes? PostOpAttributes { get; private set; }
        public XdrInt RtMax { get; private set; } = new();      // Max read transfer size
        public XdrInt RtPref { get; private set; } = new();     // Preferred read transfer size
        public XdrInt RtMult { get; private set; } = new();     // Suggested read multiple
        public XdrInt WtMax { get; private set; } = new();      // Max write transfer size
        public XdrInt WtPref { get; private set; } = new();     // Preferred write transfer size
        public XdrInt WtMult { get; private set; } = new();     // Suggested write multiple
        public XdrInt DtPref { get; private set; } = new();     // Preferred readdir size
        public XdrHyper MaxFileSize { get; private set; } = new(); // Maximum file size
        public NFSv3Time TimeDelta { get; private set; } = new(); // Server time granularity

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                var hasPostOpAttributes = new XdrBoolean();
                await hasPostOpAttributes.XdrDecodeAsync(xdr);
                if (hasPostOpAttributes.Value)
                {
                    PostOpAttributes = new NFSv3FileAttributes();
                    await PostOpAttributes.XdrDecodeAsync(xdr);
                }

                await RtMax.XdrDecodeAsync(xdr);
                await RtPref.XdrDecodeAsync(xdr);
                await RtMult.XdrDecodeAsync(xdr);
                await WtMax.XdrDecodeAsync(xdr);
                await WtPref.XdrDecodeAsync(xdr);
                await WtMult.XdrDecodeAsync(xdr);
                await DtPref.XdrDecodeAsync(xdr);
                await MaxFileSize.XdrDecodeAsync(xdr);
                await TimeDelta.XdrDecodeAsync(xdr);
            }
        }
    }
}