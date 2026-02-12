using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public abstract class NFSv3Args : IXdrData
    {
        public abstract ValueTask XdrEncodeAsync(XdrEncodingStream xdr);
        public abstract ValueTask XdrDecodeAsync(XdrDecodingStream xdr);
    }
}