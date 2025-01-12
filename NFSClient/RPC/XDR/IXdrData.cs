namespace Cartridge.Protocol.Rpc
{
    public interface IXdrData
    {
        void XdrEncode(XdrEncodingStream xdr);
        void XdrDecode(XdrDecodingStream xdr);
    }
} 