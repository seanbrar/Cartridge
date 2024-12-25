using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Represents a serializable XDR integer value.
    /// </summary>
    public class XdrInt : XdrAble
    {
        private int value;

        public XdrInt(int value)
        {
            this.value = value;
        }

        public XdrInt()
        {
            this.value = 0;
        }

        public int IntValue()
        {
            return this.value;
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeInt(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.XdrDecodeInt();
        }
    }
} 