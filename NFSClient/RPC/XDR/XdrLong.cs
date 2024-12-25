using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Represents a serializable XDR long value.
    /// </summary>
    public class XdrLong : XdrAble
    {
        private long value;

        public XdrLong(long value)
        {
            this.value = value;
        }

        public XdrLong()
        {
            this.value = 0;
        }

        public long LongValue()
        {
            return this.value;
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeLong(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.XdrDecodeLong();
        }
    }
} 