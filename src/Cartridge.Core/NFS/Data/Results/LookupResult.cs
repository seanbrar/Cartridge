using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class LookupResult : NFSv3Result
    {
        public NFSv3Handle? Handle { get; private set; }
        public NFSv3FileAttributes? Attributes { get; private set; }
        public NFSv3FileAttributes? DirAttributes { get; private set; }

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                Handle = new NFSv3Handle(forDecoding: true);
                await Handle.XdrDecodeAsync(xdr);

                var hasAttributes = new XdrBoolean();
                await hasAttributes.XdrDecodeAsync(xdr);
                if (hasAttributes.Value)
                {
                    Attributes = new NFSv3FileAttributes();
                    await Attributes.XdrDecodeAsync(xdr);
                }

                var hasDirAttributes = new XdrBoolean();
                await hasDirAttributes.XdrDecodeAsync(xdr);
                if (hasDirAttributes.Value)
                {
                    DirAttributes = new NFSv3FileAttributes();
                    await DirAttributes.XdrDecodeAsync(xdr);
                }
            }
        }
    }
}