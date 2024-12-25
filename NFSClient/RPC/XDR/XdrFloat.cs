using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Represents a serializable XDR float value.
    /// </summary>
    public class XdrFloat : IXdrData
    {
        private float value;

        public XdrFloat(float value)
        {
            this.value = value;
        }

        public XdrFloat()
        {
            this.value = 0;
        }

        public float FloatValue()
        {
            return this.value;
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeFloat(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeFloat();
        }
    }
}