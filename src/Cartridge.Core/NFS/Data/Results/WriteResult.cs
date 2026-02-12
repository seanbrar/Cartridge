using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class WriteResult : NFSv3Result
    {
        public NFSv3FileAttributes? PreOpAttributes { get; private set; }
        public NFSv3FileAttributes? PostOpAttributes { get; private set; }
        public XdrInt Count { get; private set; } = new();
        public XdrInt CommitLevel { get; private set; } = new();
        public XdrOpaque Verf { get; private set; } = new(8); // Write verifier is always 8 bytes

        public bool Committed => CommitLevel.Value == 2; // FILE_SYNC = 2

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                var hasPreOpAttributes = new XdrBoolean();
                await hasPreOpAttributes.XdrDecodeAsync(xdr);
                if (hasPreOpAttributes.Value)
                {
                    PreOpAttributes = new NFSv3FileAttributes();
                    await PreOpAttributes.XdrDecodeAsync(xdr);
                }

                var hasPostOpAttributes = new XdrBoolean();
                await hasPostOpAttributes.XdrDecodeAsync(xdr);
                if (hasPostOpAttributes.Value)
                {
                    PostOpAttributes = new NFSv3FileAttributes();
                    await PostOpAttributes.XdrDecodeAsync(xdr);
                }

                await Count.XdrDecodeAsync(xdr);
                await CommitLevel.XdrDecodeAsync(xdr);
                await Verf.XdrDecodeAsync(xdr);
            }
        }
    }
}