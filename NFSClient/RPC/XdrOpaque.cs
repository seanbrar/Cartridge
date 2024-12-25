using NFSClient.RPC.XDR;

namespace NFSClient.RPC
{
    /// <summary>
    /// Represents a serializable XDR opaque value.
    /// </summary>
    public class XdrOpaque : IXdrData
    {
        private byte[] value;

        public XdrOpaque(byte[] value)
        {
            this.value = value;
        }

        public XdrOpaque(int length)
        {
            this.value = new byte[length];
        }

        public byte[] OpaqueValue()
        {
            return this.value;
        }

        void IXdrData.XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeOpaque(value);
        }

        void IXdrData.XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeOpaque();
        }
    }
} 