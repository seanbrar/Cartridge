/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class ItemOperationArguments : XdrAble
    {
        private NFSHandle _dir;
        private Name _name;

        public ItemOperationArguments()
        { }

        public ItemOperationArguments(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._dir.XdrEncode(xdr);
            this._name.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._dir = new NFSHandle();
            this._dir.Version = V3.RPC.NFSv3Protocol.NFS_V3;
            this._dir.XdrDecode(xdr);
            this._name = new Name(xdr);
        }

        public NFSHandle Directory
        {
            get
            { return this._dir; }
            set
            { this._dir = value; }
        }

        public Name Name
        {
            get
            { return this._name; }
            set
            { this._name = value; }
        }
    }
    // End of diropargs.cs
}