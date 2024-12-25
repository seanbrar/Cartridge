/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class RemoveAccessOK : XdrAble
    {
        private WritingData _dir_wcc;

        public RemoveAccessOK()
        { }

        public RemoveAccessOK(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        { this._dir_wcc.XdrEncode(xdr); }

        public void XdrDecode(XdrDecodingStream xdr)
        { this._dir_wcc = new WritingData(xdr); }

        public WritingData Data
        {
            get
            { return this._dir_wcc; }
        }
    }

    public class RemoveAccessFAIL : XdrAble
    {
        private WritingData _dir_wcc;

        public RemoveAccessFAIL()
        { }

        public RemoveAccessFAIL(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        { this._dir_wcc.XdrEncode(xdr); }

        public void XdrDecode(XdrDecodingStream xdr)
        { this._dir_wcc = new WritingData(xdr); }

        public WritingData Data
        {
            get
            { return this._dir_wcc; }
        }
    }
    // End of RMDIR3res.cs
}