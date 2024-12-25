using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Represents a serializable XDR boolean value.
    /// </summary>
    public class XdrBoolean : XdrAble
    {
        private bool value;

        public XdrBoolean(bool value)
        {
            this.value = value;
        }

        public XdrBoolean()
        {
            this.value = false;
        }

        public bool BooleanValue()
        {
            return this.value;
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeBoolean(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.XdrDecodeBoolean();
        }
    }
} 