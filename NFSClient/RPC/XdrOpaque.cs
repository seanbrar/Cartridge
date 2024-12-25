using NFSLibrary.RPC.XDR;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Represents a serializable XDR opaque value.
    /// </summary>
    public class XdrOpaque : XdrAble
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

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeOpaque(value);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            value = xdr.XdrDecodeOpaque();
        }
    }
} 