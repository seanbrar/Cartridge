using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class CreateResult : NFSv3Result
    {
        public NFSv3Handle? Handle { get; private set; }
        public XdrBoolean HandleSet { get; private set; } = new();
        public NFSv3FileAttributes? PostOpAttributes { get; private set; }
        public NFSv3FileAttributes? DirWccData { get; private set; }

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                await HandleSet.XdrDecodeAsync(xdr);
                if (HandleSet.Value)
                {
                    Handle = new NFSv3Handle(forDecoding: true);
                    await Handle.XdrDecodeAsync(xdr);
                }

                var hasPostOpAttributes = new XdrBoolean();
                await hasPostOpAttributes.XdrDecodeAsync(xdr);
                if (hasPostOpAttributes.Value)
                {
                    PostOpAttributes = new NFSv3FileAttributes();
                    await PostOpAttributes.XdrDecodeAsync(xdr);
                }

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
}