/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class WritingAttributes : IXdrData
    {
        private long _size;
        private NFSTimeValue _mtime;
        private NFSTimeValue _ctime;

        public WritingAttributes()
        { }

        public WritingAttributes(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeLong(this._size);
            this._mtime.XdrEncode(xdr);
            this._ctime.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._size = xdr.XdrDecodeLong();
            this._mtime = new NFSTimeValue(xdr);
            this._ctime = new NFSTimeValue(xdr);
        }

        public long Size
        {
            get
            { return this._size; }
        }

        public NFSTimeValue ModifiedTime
        {
            get
            { return this._mtime; }
        }

        public NFSTimeValue CreateTime
        {
            get
            { return this._ctime; }
        }
    }
    // End of wcc_attr.cs
}