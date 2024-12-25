namespace NFSClient.RPC.XDR
{
    public interface IXdrData
    {
        void XdrEncode(XdrEncodingStream xdr);
        void XdrDecode(XdrDecodingStream xdr);
    }
} 