/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class Entry : IXdrData
    {
        private long _fileid;
        private Name _name;
        private NFSCookie _cookie;
        private Entry _nextentry;

        public Entry()
        { }

        public Entry(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            Entry _this = this;

            do
            {
                xdr.XdrEncodeLong(_this._fileid);

                _this._name.XdrEncode(xdr);
                _this._cookie.XdrEncode(xdr);
                _this = _this._nextentry;

                xdr.XdrEncodeBoolean(_this != null);
            } while (_this != null);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            Entry _this = this;
            Entry _next;

            do
            {
                _this._fileid = xdr.XdrDecodeLong();
                _this._name = new Name(xdr);
                _this._cookie = new NFSCookie(xdr);
                _next = xdr.XdrDecodeBoolean() ? new Entry() : null;
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

        public Entry NextEntry
        {
            get
            { return this._nextentry; }
        }
    }
    // End of entry3.cs
}
