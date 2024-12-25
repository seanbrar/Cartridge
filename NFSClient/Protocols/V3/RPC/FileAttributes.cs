/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class FileAttributes : XdrAble
    {
        private NFSItemTypes _type;
        private NFSPermission _mode;
        private int _nlink;
        private int _uid;
        private int _gid;
        private long _size;
        private long _used;
        private SpecInformation _rdev;
        private long _fsid;
        private long _fileid;
        private NFSTimeValue _atime;
        private NFSTimeValue _mtime;
        private NFSTimeValue _ctime;

        public FileAttributes()
        { }

        public FileAttributes(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeInt((int)this._type);
            xdr.XdrEncodeInt(this._mode.Mode);
            xdr.XdrEncodeInt(this._nlink);
            xdr.XdrEncodeInt(this._uid);
            xdr.XdrEncodeInt(this._gid);
            xdr.XdrEncodeLong(this._size);
            xdr.XdrEncodeLong(this._used);
            this._rdev.XdrEncode(xdr);
            xdr.XdrEncodeLong(this._fsid);
            xdr.XdrEncodeLong(this._fileid);
            this._atime.XdrEncode(xdr);
            this._mtime.XdrEncode(xdr);
            this._ctime.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._type = (NFSItemTypes)xdr.XdrDecodeInt();

            this._mode = new NFSPermission();
            this._mode.Mode = xdr.XdrDecodeInt();

            this._nlink = xdr.XdrDecodeInt();
            this._uid = xdr.XdrDecodeInt();
            this._gid = xdr.XdrDecodeInt();
            this._size = xdr.XdrDecodeLong();
            this._used = xdr.XdrDecodeLong();
            
            this._rdev = new SpecInformation();
            this._rdev.XdrDecode(xdr);
            
            this._fsid = xdr.XdrDecodeLong();
            this._fileid = xdr.XdrDecodeLong();
            this._atime = new NFSTimeValue(xdr);
            this._mtime = new NFSTimeValue(xdr);
            this._ctime = new NFSTimeValue(xdr);
        }

        public NFSItemTypes Type
        {
            get
            { return this._type; }
        }

        public NFSPermission Mode
        {
            get
            { return this._mode; }
        }

        public int Link
        {
            get
            { return this._nlink; }
        }

        public int UserID
        {
            get
            { return this._uid; }
        }

        public int GroupID
        {
            get
            { return this._gid; }
        }

        public long Size
        {
            get
            { return this._size; }
        }

        public long Used
        {
            get
            { return this._used; }
        }

        public SpecInformation BlockInfo
        {
            get
            { return this._rdev; }
        }

        public long FSID
        {
            get
            { return this._fsid; }
        }

        public long FileID
        {
            get
            { return this._fileid; }
        }

        public NFSTimeValue LastAccessedTime
        {
            get
            { return this._atime; }
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
    // End of fattr3.cs
}