using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Attributes
{
    public class NFSv3FileAttributes : IXdrData
    {
        public NFSv3FileType Type { get; private set; }
        public XdrInt Mode { get; private set; } = new();
        public XdrInt NLink { get; private set; } = new();
        public XdrInt Uid { get; private set; } = new();
        public XdrInt Gid { get; private set; } = new();
        public XdrHyper Size { get; private set; } = new();
        public XdrHyper Used { get; private set; } = new();
        public XdrInt RDev { get; private set; } = new();
        public XdrHyper FSId { get; private set; } = new();
        public XdrHyper FileId { get; private set; } = new();
        public NFSv3Time ATime { get; private set; } = new();
        public NFSv3Time MTime { get; private set; } = new();
        public NFSv3Time CTime { get; private set; } = new();

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            var fileType = new XdrInt((int)Type);
            await fileType.XdrEncodeAsync(xdr);
            await Mode.XdrEncodeAsync(xdr);
            await NLink.XdrEncodeAsync(xdr);
            await Uid.XdrEncodeAsync(xdr);
            await Gid.XdrEncodeAsync(xdr);
            await Size.XdrEncodeAsync(xdr);
            await Used.XdrEncodeAsync(xdr);
            await RDev.XdrEncodeAsync(xdr);
            await FSId.XdrEncodeAsync(xdr);
            await FileId.XdrEncodeAsync(xdr);
            await ATime.XdrEncodeAsync(xdr);
            await MTime.XdrEncodeAsync(xdr);
            await CTime.XdrEncodeAsync(xdr);
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var fileType = new XdrInt();
            await fileType.XdrDecodeAsync(xdr);
            Type = (NFSv3FileType)fileType.Value;

            await Mode.XdrDecodeAsync(xdr);
            await NLink.XdrDecodeAsync(xdr);
            await Uid.XdrDecodeAsync(xdr);
            await Gid.XdrDecodeAsync(xdr);
            await Size.XdrDecodeAsync(xdr);
            await Used.XdrDecodeAsync(xdr);
            await RDev.XdrDecodeAsync(xdr);
            await FSId.XdrDecodeAsync(xdr);
            await FileId.XdrDecodeAsync(xdr);
            await ATime.XdrDecodeAsync(xdr);
            await MTime.XdrDecodeAsync(xdr);
            await CTime.XdrDecodeAsync(xdr);
        }
    }
}