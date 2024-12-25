/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class PreOperationAttributes : IXdrData
    {
        private bool _attributes_follow;
        private WritingAttributes _attributes;

        public PreOperationAttributes()
        { }

        public PreOperationAttributes(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeBoolean(this._attributes_follow);
            if (this._attributes_follow)
            { this._attributes.XdrEncode(xdr); }
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._attributes_follow = xdr.xdrDecodeBoolean();
            if (this._attributes_follow)
            { this._attributes = new WritingAttributes(xdr); }
        }

        public bool AttributesExists
        {
            get
            { return this._attributes_follow; }
        }

        public WritingAttributes Attributes
        {
            get
            { return this._attributes; }
        }
    }
    // End of pre_op_attr.cs
}