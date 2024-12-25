/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSClient.RPC.XDR;

namespace NFSLibrary.Protocols.Commons
{
    public class NFSHandle : IXdrData
    {
        private byte[] _value;
        private int _version;

        public NFSHandle()
        { }

        public NFSHandle(byte[] value, int version)
        {
            this._value = value;
            this._version = version;
        }

        public NFSHandle(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public NFSHandle(XdrDecodingStream xdr, int version)
        {
            this._version = version;
            XdrDecode(xdr);
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            switch (this._version)
            {
                case 2:
                    xdr.XdrEncodeOpaque(this._value);
                    break;
                case 3:
                    xdr.XdrEncodeOpaque(this._value);
                    break;
            }
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            switch (this._version)
            {
                case 2:
                    this._value = xdr.XdrDecodeOpaque(); 
                    break;
                case 3:
                    this._value = xdr.XdrDecodeOpaque(); 
                    break;
            }
        }

        public byte[] Value
        {
            get
            { return this._value; }
        }

        public int Version
        {
            get
            { return this._version; }
            set
            { this._version = value; }
        }
    }
    // End of nfshandle.cs
}