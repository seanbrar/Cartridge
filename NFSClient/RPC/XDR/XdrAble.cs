namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Interface for XDR-encodable types.
    /// </summary>
    public interface XdrAble
    {
        /// <summary>
        /// Encodes this object to an XDR stream.
        /// </summary>
        void XdrEncode(XdrEncodingStream xdr);

        /// <summary>
        /// Decodes this object from an XDR stream.
        /// </summary>
        void XdrDecode(XdrDecodingStream xdr);
    }
} 