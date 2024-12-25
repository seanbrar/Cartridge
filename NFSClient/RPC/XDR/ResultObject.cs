namespace NFSLibrary.RPC.XDR
{
    public class ResultObject<TOK, TFAIL> : XdrAble where TOK : XdrAble where TFAIL : XdrAble
    {
        private NFSLibrary.Protocols.Commons.NFSStats _status;
        private TOK _ok;
        private TFAIL _fail;

        public ResultObject()
        { }

        public ResultObject(NFSLibrary.Protocols.Commons.NFSStats status, TOK ok, TFAIL fail)
        {
            _status = status;
            _ok = ok;
            _fail = fail;
        }

        public ResultObject(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.XdrEncodeInt((int)_status);
            if (_status == NFSLibrary.Protocols.Commons.NFSStats.NFS_OK)
                _ok.XdrEncode(xdr);
            else
                _fail.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            _status = (NFSLibrary.Protocols.Commons.NFSStats)xdr.XdrDecodeInt();
            if (_status == NFSLibrary.Protocols.Commons.NFSStats.NFS_OK)
                _ok = (TOK)Activator.CreateInstance(typeof(TOK), xdr);
            else
                _fail = (TFAIL)Activator.CreateInstance(typeof(TFAIL), xdr);
        }

        public NFSLibrary.Protocols.Commons.NFSStats Status
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