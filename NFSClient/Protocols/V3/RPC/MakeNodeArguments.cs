/*
 * Automatically generated by jrpcgen 1.0.7 on 29.11.2012
 * jrpcgen is part of the "Remote Tea.Net" ONC/RPC package for C#
 * See http://remotetea.sourceforge.net for details
 */
using NFSLibrary.Protocols.Commons;
using NFSLibrary.RPC.XDR;

namespace NFSLibrary.Protocols.V3.RPC
{
    public class MakeNodeArguments : XdrAble
    {
        private ItemOperationArguments _where;
        private MakeNodeData _what;

        public MakeNodeArguments()
        { }

        public MakeNodeArguments(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._where.XdrEncode(xdr);
            this._what.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._where = new ItemOperationArguments(xdr);
            this._what = new MakeNodeData(xdr);
        }

        public ItemOperationArguments Where
        {
            get
            { return this._where; }
            set
            { this._where = value; }
        }

        public MakeNodeData What
        {
            get
            { return this._what; }
            set
            { this._what = value; }
        }
    }

    public class MakeNodeData : XdrAble
    {
        private NFSItemTypes _type;
        private DeviceData _device_chr;
        private DeviceData _device_blk;
        private MakeAttributes _pipe_attributes_sock;
        private MakeAttributes _pipe_attributes_fifo;

        public MakeNodeData()
        { }

        public MakeNodeData(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeInt((int)this._type);

            switch (this._type)
            {
                case NFSItemTypes.Character:
                    this._device_chr.XdrEncode(xdr);
                    break;
                case NFSItemTypes.Block:
                    this._device_blk.XdrEncode(xdr);
                    break;
                case NFSItemTypes.Socket:
                    this._pipe_attributes_sock.XdrEncode(xdr);
                    break;
                case NFSItemTypes.NamedPipe:
                    this._pipe_attributes_fifo.XdrEncode(xdr);
                    break;
                default:
                    break;
            }
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._type = (NFSItemTypes)xdr.xdrDecodeInt();

            switch (this._type)
            {
                case NFSItemTypes.Character:
                    this._device_chr = new DeviceData(xdr);
                    break;
                case NFSItemTypes.Block:
                    this._device_blk = new DeviceData(xdr);
                    break;
                case NFSItemTypes.Socket:
                    this._pipe_attributes_sock = new MakeAttributes(xdr);
                    break;
                case NFSItemTypes.NamedPipe:
                    this._pipe_attributes_fifo = new MakeAttributes(xdr);
                    break;
                default:
                    break;
            }
        }

        public NFSItemTypes Type
        {
            get
            { return this._type; }
            set
            { this._type = value; }
        }

        public DeviceData DeviceChar
        {
            get
            { return this._device_chr; }
            set
            { this._device_chr = value; }
        }

        public DeviceData DeviceBlock
        {
            get
            { return this._device_blk; }
            set
            { this._device_blk = value; }
        }

        public MakeAttributes SOCKAttributes
        {
            get
            { return this._pipe_attributes_sock; }
            set
            { this._pipe_attributes_sock = value; }
        }

        public MakeAttributes FIFOAttributes
        {
            get
            { return this._pipe_attributes_fifo; }
            set
            { this._pipe_attributes_fifo = value; }
        }
    }

    public class DeviceData : XdrAble
    {
        private MakeAttributes _dev_attributes;
        private SpecInformation _spec;

        public DeviceData()
        { }

        public DeviceData(XdrDecodingStream xdr)
        { XdrDecode(xdr); }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            this._dev_attributes.XdrEncode(xdr);
            this._spec.XdrEncode(xdr);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            this._dev_attributes = new MakeAttributes(xdr);
            this._spec = new SpecInformation(xdr);
        }

        public MakeAttributes Attributes
        {
            get
            { return this._dev_attributes; }
            set
            { this._dev_attributes = value; }
        }

        public SpecInformation Spec
        {
            get
            { return this._spec; }
            set
            { this._spec = value; }
        }
    }
    // End of MKNOD3args.cs
}