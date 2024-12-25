using System.Text;

namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Implements XDR decoding from a memory buffer.
    /// </summary>
    public class XdrBufferDecodingStream : XdrDecodingStream
    {
        private readonly MemoryStream _buffer;
        private readonly BinaryReader _reader;

        public XdrBufferDecodingStream(byte[] data)
        {
            _buffer = new MemoryStream(data);
            _reader = new BinaryReader(_buffer);
        }

        public override int XdrDecodeInt()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt32());
        }

        public override long XdrDecodeLong()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt64());
        }

        public override float XdrDecodeFloat()
        {
            byte[] bytes = _reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public override double XdrDecodeDouble()
        {
            byte[] bytes = _reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        public override bool XdrDecodeBoolean()
        {
            return XdrDecodeInt() != 0;
        }

        public override string XdrDecodeString()
        {
            int length = XdrDecodeInt();
            byte[] bytes = _reader.ReadBytes(length);
            int padding = (4 - (length % 4)) % 4;
            _reader.ReadBytes(padding); // Skip padding
            return Encoding.UTF8.GetString(bytes);
        }

        public override byte[] XdrDecodeOpaque()
        {
            int length = XdrDecodeInt();
            byte[] bytes = _reader.ReadBytes(length);
            int padding = (4 - (length % 4)) % 4;
            _reader.ReadBytes(padding); // Skip padding
            return bytes;
        }

        public override void Dispose()
        {
            _reader.Dispose();
            _buffer.Dispose();
        }
    }
} 