using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class AccessResult : NFSv3Result
    {
        public NFSv3FileAttributes? Attributes { get; private set; }
        public XdrInt Access { get; } = new();

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
                    hasPostOpAttributes = new NFSv3FileAttributes();
                    await hasPostOpAttributes.XdrDecodeAsync(xdr);
                }

                await Access.XdrDecodeAsync(xdr);
            }
        }
    }
}