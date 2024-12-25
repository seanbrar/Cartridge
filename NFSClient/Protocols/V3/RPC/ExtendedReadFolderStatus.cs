/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons; 
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class ExtendedReadFolderAccessOK : XdrAble
    {
        private PostOperationAttributes _dir_attributes;
        private byte[] _cookieverf;
        private FolderList _reply;

        public ExtendedReadFolderAccessOK()
        { }

        public ExtendedReadFolderAccessOK(XdrDecodingStream xdr)
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
            this._reply = new FolderList(xdr);
        }

        public PostOperationAttributes Attributes
        {
            get
            { return this._dir_attributes; }
        }

        public byte[] CookieVerification
        {
            get
            { return this._cookieverf; }
        }

        public FolderList Reply
        {
            get
            { return this._reply; }
        }
    }

    public class ExtendedReadFolderAccessFAIL : XdrAble
    {
        private PostOperationAttributes _dir_attributes;

        public ExtendedReadFolderAccessFAIL()
        { }

        public ExtendedReadFolderAccessFAIL(XdrDecodingStream xdr)
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

    public class FolderList : XdrAble
    {
        private FolderEntry _entries;
        private bool _eof;

        public FolderList()
        { }

        public FolderList(XdrDecodingStream xdr)
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
            this._entries = xdr.xdrDecodeBoolean() ? new FolderEntry(xdr) : null;
            this._eof = xdr.xdrDecodeBoolean();
        }

        public FolderEntry Entries
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

    public class FolderEntry : XdrAble
    {
        private long _fileid;
        private Name _name;
        private NFSCookie _cookie;
        private PostOperationAttributes _name_attributes;
        private NFSHandle _name_handle;
        private FolderEntry _nextentry;

        public FolderEntry()
        { }

        public FolderEntry(XdrDecodingStream xdr)
        { xdrDecode(xdr); }

        public void xdrEncode(XdrEncodingStream xdr)
        {
            FolderEntry _this = this;

            do
            {
                xdr.xdrEncodeLong(_this._fileid);
                _this._name.xdrEncode(xdr);
                _this._cookie.xdrEncode(xdr);
                _this._name_attributes.xdrEncode(xdr);
                _this._name_handle.xdrEncode(xdr);
                _this = _this._nextentry;
                xdr.xdrEncodeBoolean(_this != null);
            } while (_this != null);
        }

        public void xdrDecode(XdrDecodingStream xdr)
        {
            FolderEntry _this = this;
            FolderEntry _next;

            do
            {
                _this._fileid = xdr.xdrDecodeLong();
                _this._name = new Name(xdr);
                _this._cookie = new NFSCookie(xdr);
                _this._name_attributes = new PostOperationAttributes(xdr);
                _this._name_handle = (xdr.xdrDecodeBoolean() ? new NFSHandle(xdr, 3) : null);
                _next = xdr.xdrDecodeBoolean() ? new FolderEntry() : null;
                _this._nextentry = _next;
                _this = _next;
            } while (_this != null);
        }

        public long FileID
        {
            get
            { return this._fileid; }
        }

        public Name Name
        {
            get
            { return this._name; }
        }

        public NFSCookie Cookie
        {
            get
            { return this._cookie; }
        }

        public PostOperationAttributes Attributes
        {
            get
            { return this._name_attributes; }
        }

        public NFSHandle Handle
        {
            get
            { return this._name_handle; }
        }

        public FolderEntry NextEntry
        {
            get
            { return this._nextentry; }
        }
    }
    // End of READDIRPLUS3res.cs
}