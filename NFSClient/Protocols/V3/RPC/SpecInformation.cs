/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class SpecInformation : IXdrData
    {
        private int _specdata1;
        private int _specdata2;

        public SpecInformation()
        { }

        public SpecInformation(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeInt(this._specdata1);
            xdr.XdrEncodeInt(this._specdata2);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._specdata1 = xdr.XdrDecodeInt();
            this._specdata2 = xdr.XdrDecodeInt();
        }

        public int SpecData1
        {
            get
            { return this._specdata1; }
        }

        public int SpecData2
        {
            get
            { return this._specdata2; }
        }
    }
    // End of specdata3.cs
}