using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Represents a void XDR value, used for procedures that don't take parameters or return values.
    /// </summary>
    public class XdrVoid : XdrAble
    {
        public static readonly XdrVoid XDR_VOID = new XdrVoid();

        public XdrVoid()
        {
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            // No-op for void type
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            // No-op for void type
        }
    }
} 