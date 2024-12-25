using NFSLibrary.RPC.XDR;

namespace NFSClient.RPC.XDR
{
    /// <summary>
    /// Represents a void XDR value, used for procedures that don't take parameters or return values.
    /// </summary>
    public sealed class XdrVoid : IXdrData
    {
        public void XdrEncode(XdrEncodingStream xdr)
        {
            // No data to encode
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            // No data to decode
        }
    }
} 