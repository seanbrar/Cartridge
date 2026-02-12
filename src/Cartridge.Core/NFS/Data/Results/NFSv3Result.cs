using Cartridge.Core.RPC.XDR;
using Cartridge.Core.NFS.Types;

namespace Cartridge.Core.NFS.Data.Results
{
    public abstract class NFSv3Result : IXdrData
    {
        public XdrNFSv3Status Status { get; set; } = new();

        public abstract ValueTask XdrEncodeAsync(XdrEncodingStream xdr);
        public abstract ValueTask XdrDecodeAsync(XdrDecodingStream xdr);
    }
}