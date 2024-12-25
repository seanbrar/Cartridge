/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSClient.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class GetAttributeArguments : IXdrData
    {
        private NFSHandle _handle;

        public GetAttributeArguments()
        { }

        public GetAttributeArguments(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        { this._handle.XdrEncode(xdr); }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._handle = new NFSHandle();
            this._handle.Version = V3.RPC.NFSv3Protocol.NFS_V3;
            this._handle.XdrDecode(xdr);
        }

        public NFSHandle Handle
        {
            get
            { return this._handle; }
            set
            { this._handle = value; }
        }
    }
    // End of GETATTR3args.cs
}
