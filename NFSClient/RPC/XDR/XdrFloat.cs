using NFSClient.RPC.XDR;

namespace NFSClient.RPC.XDR
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

        void IXdrData.XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeFloat(value);
        }

        void IXdrData.XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeFloat();
        }
    }
}