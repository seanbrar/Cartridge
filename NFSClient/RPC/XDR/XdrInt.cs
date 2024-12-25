namespace NFSClient.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR integer value.
    /// </summary>
    public class XdrInt : IXdrData
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
            xdr.xdrEncodeInt(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeInt();
        }
    }
} 