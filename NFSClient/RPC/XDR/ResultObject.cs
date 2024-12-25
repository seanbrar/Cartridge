using System;
using NFSLibrary.Protocols.Commons;

namespace NFSLibrary.RPC.XDR
{
    public class ResultObject<TOK, TFAIL> : IXdrData where TOK : IXdrData where TFAIL : IXdrData
    {
        private NFSStats _status;
        private TOK _ok;
        private TFAIL _fail;

        public ResultObject()
        { }

        public ResultObject(NFSStats status, TOK ok, TFAIL fail)
        {
            _status = status;
            _ok = ok;
            _fail = fail;
        }

        public ResultObject(XdrDecodingStream xdr)
        {
            XdrDecode(xdr);
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeInt((int)_status);
            if (_status == NFSStats.NFS_OK)
                _ok.XdrEncode(xdr);
            else
                _fail.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            _status = (NFSStats)xdr.xdrDecodeInt();
            if (_status == NFSStats.NFS_OK)
                _ok = (TOK)Activator.CreateInstance(typeof(TOK), xdr);
            else
                _fail = (TFAIL)Activator.CreateInstance(typeof(TFAIL), xdr);
        }

        public NFSStats Status
        {
            get { return _status; }
        }

        public TOK OK
        {
            get { return _ok; }
        }

        public TFAIL Fail
        {
            get { return _fail; }
        }
    }
} 