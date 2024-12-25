/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSClient.RPC.XDR;

namespace NFSClient.Protocols.V3.RPC
{
    public class MakeFileAccessOK : IXdrData
    {
        private NFSHandle _handle;
        private PostOperationAttributes _attributes;
        private WritingData _dir_wcc;

        public MakeFileAccessOK()
        { }

        public MakeFileAccessOK(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._handle.XdrEncode(xdr);
            this._attributes.XdrEncode(xdr);
            this._dir_wcc.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._handle = new NFSHandle();
            this._handle.Version = V3.RPC.NFSv3Protocol.NFS_V3;
            this._handle.XdrDecode(xdr);
            this._attributes = new PostOperationAttributes(xdr);
            this._dir_wcc = new WritingData(xdr);
        }

        public NFSHandle Handle
        {
            get
            { return this._handle; }
        }

        public PostOperationAttributes Attributes
        {
            get
            { return this._attributes; }
        }

        public WritingData Data
        {
            get
            { return this._dir_wcc; }
        }
    }

    public class MakeFileAccessFAIL : IXdrData
    {
        private WritingData _dir_wcc;

        public MakeFileAccessFAIL()
        { }

        public MakeFileAccessFAIL(XdrDecodingStream xdr)
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
    // End of MKDIR3res.cs
}