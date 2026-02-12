using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class CommitResult : NFSv3Result
    {
        public NFSv3FileAttributes? Attributes { get; private set; }
        public XdrHyper Verf { get; private set; } = new();

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                var hasAttributes = new XdrBoolean();
                await hasAttributes.XdrDecodeAsync(xdr);
                if (hasAttributes.Value)
                {
                    Attributes = new NFSv3FileAttributes();
                    await Attributes.XdrDecodeAsync(xdr);
                }
                await Verf.XdrDecodeAsync(xdr);
            }
        }
    }
}