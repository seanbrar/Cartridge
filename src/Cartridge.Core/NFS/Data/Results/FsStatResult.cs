using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class FsStatResult : NFSv3Result
    {
        public NFSv3FileAttributes? PostOpAttributes { get; private set; }
        public XdrHyper TotalBytes { get; private set; } = new();
        public XdrHyper FreeBytes { get; private set; } = new();
        public XdrHyper AvailableBytes { get; private set; } = new();
        public XdrHyper TotalFiles { get; private set; } = new();
        public XdrHyper FreeFiles { get; private set; } = new();
        public XdrHyper AvailableFiles { get; private set; } = new();
        public XdrInt InvariantSeconds { get; private set; } = new();

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

                await TotalBytes.XdrDecodeAsync(xdr);
                await FreeBytes.XdrDecodeAsync(xdr);
                await AvailableBytes.XdrDecodeAsync(xdr);
                await TotalFiles.XdrDecodeAsync(xdr);
                await FreeFiles.XdrDecodeAsync(xdr);
                await AvailableFiles.XdrDecodeAsync(xdr);
                await InvariantSeconds.XdrDecodeAsync(xdr);
            }
        }
    }
}