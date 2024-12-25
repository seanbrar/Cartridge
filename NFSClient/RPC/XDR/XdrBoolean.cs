using NFSClient.RPC.XDR;

namespace NFSClient.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR boolean value.
    /// </summary>
    public class XdrBoolean : IXdrData
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
            xdr.xdrEncodeBoolean(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeBoolean();
        }
    }
} 