using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR string value.
    /// </summary>
    public class XdrString : IXdrData
    {
        private string value;

        public XdrString(string value)
        {
            this.value = value;
        }

        public XdrString()
        {
            this.value = string.Empty;
        }

        public string StringValue()
        {
            return this.value;
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeString(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeString();
        }
    }
} 