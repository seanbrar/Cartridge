/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSClient.RPC.XDR;

namespace NFSClient.Protocols.V3.RPC
{
    public class NFSCookie : IXdrData
    {
        private long _value;

        public NFSCookie()
        { }

        public NFSCookie(long value)
        { this._value = value; }

        public NFSCookie(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        { xdr.XdrEncodeLong(this._value); }

        public void XdrDecode(XdrDecodingStream xdr)
        { this._value = xdr.XdrDecodeLong(); }

        public long Value
        {
            get
            { return this._value; }
        }
    }
    // End of cookie3.cs
}

