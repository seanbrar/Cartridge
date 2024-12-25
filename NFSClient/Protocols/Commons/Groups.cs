/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.Commons
{
    public class Groups : XdrAble
    {
        private GroupNode _value;

        public Groups()
        { }

        public Groups(GroupNode value)
        { this._value = value; }

        public Groups(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            if (this._value != null)
            {
                xdr.XdrEncodeBoolean(true);
                this._value.XdrEncode(xdr);
            }
            else { xdr.XdrEncodeBoolean(false); };
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._value = xdr.XdrDecodeBoolean() ? new GroupNode(xdr) : null;
        }

        public GroupNode Value
        {
            get
            { return this._value; }
        }
    }

    public class GroupNode : XdrAble
    {
        private Name _grname;
        private Groups _grnext;

        public GroupNode()
        { }

        public GroupNode(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._grname.XdrEncode(xdr);
            this._grnext.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._grname = new Name(xdr);
            this._grnext = new Groups(xdr);
        }

        public Name GroupName
        {
            get
            { return this._grname; }
        }

        public Groups NextGroup
        {
            get
            { return this._grnext; }
        }
    }
    // End of groups.cs
}