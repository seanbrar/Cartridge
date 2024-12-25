using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
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

        public void xdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeOpaque(value);
        }

        public void xdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.xdrDecodeOpaque();
        }
    }
} 