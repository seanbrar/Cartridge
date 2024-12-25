/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class PathConfigurationArguments : XdrAble
    {
        private NFSHandle _obj;

        public PathConfigurationArguments()
        { }

        public PathConfigurationArguments(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        { this._obj.XdrEncode(xdr); }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._obj = new NFSHandle();
            this._obj.Version = V3.RPC.NFSv3Protocol.NFS_V3;
            this._obj.XdrDecode(xdr);
        }

        public NFSHandle Handle
        {
            get
            { return this._obj; }
            set
            { this._obj = value; }
        }
    }
    // End of PATHCONF3args.cs
}