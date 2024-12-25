/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class CommitArguments : XdrAble
    {
        private NFSHandle _file;
        private long _offset;
        private int _count;

        public CommitArguments()
        { }

        public CommitArguments(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._file.XdrEncode(xdr);
            xdr.xdrEncodeLong(this._offset);
            xdr.xdrEncodeInt(this._count);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._file = new NFSHandle();
            this._file.Version = V3.RPC.NFSv3Protocol.NFS_V3;
            this._file.XdrDecode(xdr);
            this._offset = xdr.xdrDecodeLong();
            this._count = xdr.xdrDecodeInt();
        }

        public NFSHandle File
        {
            get
            { return this._file; }
            set
            { this._file = value; }
        }

        public long Offset
        {
            get
            { return this._offset; }
            set
            { this._offset = value; }
        }

        public int Count
        {
            get
            { return this._count; }
            set
            { this._count = value; }
        }
    }
    // End of COMMIT3args.cs
}