using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class ReadResult : NFSv3Result
    {
        public NFSv3FileAttributes? Attributes { get; private set; }
        public XdrInt Count { get; private set; } = new();
        public XdrBoolean Eof { get; private set; } = new();
        public XdrOpaque Data { get; private set; } = new();

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

                await Count.XdrDecodeAsync(xdr);
                await Eof.XdrDecodeAsync(xdr);
                await Data.XdrDecodeAsync(xdr);
            }
        }
    }
}