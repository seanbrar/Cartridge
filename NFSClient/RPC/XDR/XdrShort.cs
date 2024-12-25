// File: NFSClient/RPC/XDR/XdrShort.cs

namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR short value.
    /// </summary>
    public class XdrShort : IXdrData
    {
        private short value;

        public XdrShort(short value)
        {
            this.value = value;
        }

        public XdrShort()
        {
            this.value = 0;
        }

        public short ShortValue()
        {
            return this.value;
        }

        void IXdrData.XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeInt(value);
        }

        void IXdrData.XdrDecode(XdrDecodingStream xdr)
        {
            value = (short)xdr.xdrDecodeInt();
        }
    }
}