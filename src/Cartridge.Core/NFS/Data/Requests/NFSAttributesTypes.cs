using Cartridge.Core.RPC.XDR;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.NFS.Data.Attributes;

namespace Cartridge.Core.NFS.Data.Requests
{
    public class NFSAttributesRequest : IXdrData
    {
        public required XdrString Path { get; set; } = new();

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => Path.XdrEncodeAsync(xdr);

        public ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
            => Path.XdrDecodeAsync(xdr);
    }

    public class NFSAttributesResponse : IXdrData
    {
        public required NFSv3FileAttributes Attributes { get; set; } = new();

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => Attributes.XdrEncodeAsync(xdr);

        public ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
            => Attributes.XdrDecodeAsync(xdr);
    }

    public class NFSReadRequest : IXdrData
    {
        public required XdrString Path { get; set; } = new();
        public required XdrLong Offset { get; set; } = new();
        public required XdrInt Length { get; set; } = new();

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await Path.XdrEncodeAsync(xdr);
            await Offset.XdrEncodeAsync(xdr);
            await Length.XdrEncodeAsync(xdr);
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Path.XdrDecodeAsync(xdr);
            await Offset.XdrDecodeAsync(xdr);
            await Length.XdrDecodeAsync(xdr);
        }
    }
} 