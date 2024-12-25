using NFSClient.RPC.XDR;

namespace NFSClient.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR long value.
    /// </summary>
    public class XdrLong : IXdrData
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
            xdr.xdrEncodeLong(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeLong();
        }
    }
} 