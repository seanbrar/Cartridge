// TODO: Assess placement of this file in NFS folder

using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Types
{
    public class XdrNFSv3Status : IXdrData
    {
        public NFSv3Status Value { get; set; }

        public XdrNFSv3Status(NFSv3Status value = NFSv3Status.NFS3_OK)
        {
            Value = value;
        }

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            return xdr.EncodeIntAsync((int)Value);
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var value = await xdr.DecodeIntAsync();
            Value = (NFSv3Status)value;
        }
    }
} 