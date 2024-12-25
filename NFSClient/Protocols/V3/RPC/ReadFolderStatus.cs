/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class ReadFolderAccessResultOK : XdrAble
    {
        private PostOperationAttributes _dir_attributes;
        private byte[] _cookieverf;
        private ItemAccessOK _reply;

        public ReadFolderAccessResultOK()
        { }

        public ReadFolderAccessResultOK(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._dir_attributes.XdrEncode(xdr);
            xdr.xdrEncodeOpaque(this._cookieverf, NFSv3Protocol.NFS3_COOKIEVERFSIZE);
            this._reply.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._dir_attributes = new PostOperationAttributes(xdr);
            this._cookieverf = xdr.xdrDecodeOpaque(NFSv3Protocol.NFS3_COOKIEVERFSIZE);
            this._reply = new ItemAccessOK(xdr);
        }

        public PostOperationAttributes Attributes
        {
            get
            { return this._dir_attributes; }
        }

        public byte[] CookieData
        {
            get
            { return this._cookieverf; }
        }

        public ItemAccessOK Reply
        {
            get
            { return this._reply; }
        }
    }

    public class ItemAccessOK : XdrAble
    {
        private Entry _entries;
        private bool _eof;

        public ItemAccessOK()
        { }

        public ItemAccessOK(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            if (this._entries != null)
            {
                xdr.xdrEncodeBoolean(true);
                this._entries.XdrEncode(xdr);
            }
            else { xdr.xdrEncodeBoolean(false); };

            xdr.xdrEncodeBoolean(this._eof);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._entries = xdr.xdrDecodeBoolean() ? new Entry(xdr) : null;
            this._eof = xdr.xdrDecodeBoolean();
        }

        public Entry Entries
        {
            get
            { return this._entries; }
        }

        public bool EOF
        {
            get
            { return this._eof; }
        }
    }

    public class ReadFolderAccessResultFAIL : XdrAble
    {
        private PostOperationAttributes _dir_attributes;

        public ReadFolderAccessResultFAIL()
        { }

        public ReadFolderAccessResultFAIL(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        { this._dir_attributes.XdrEncode(xdr); }

        public void XdrDecode(XdrDecodingStream xdr)
        { this._dir_attributes = new PostOperationAttributes(xdr); }

        public PostOperationAttributes Attributes
        {
            get
            { return this._dir_attributes; }
        }
    }
    // End of READDIR3res.cs
}