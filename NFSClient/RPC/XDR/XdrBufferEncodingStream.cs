using System.Text;

namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Implements XDR encoding into a memory buffer.
    /// </summary>
    public class XdrBufferEncodingStream : XdrEncodingStream
    {
        private readonly MemoryStream _buffer;
        private readonly BinaryWriter _writer;

        public XdrBufferEncodingStream(int initialSize = 1024)
        {
            _buffer = new MemoryStream(initialSize);
            _writer = new BinaryWriter(_buffer);
        }

        public override void XdrEncodeInt(int value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public override void XdrEncodeLong(long value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public override void XdrEncodeFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public override void XdrEncodeDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public override void XdrEncodeBoolean(bool value)
        {
            XdrEncodeInt(value ? 1 : 0);
        }

        public override void XdrEncodeString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            XdrEncodeInt(bytes.Length);
            _writer.Write(bytes);
            int padding = (4 - (bytes.Length % 4)) % 4;
            for (int i = 0; i < padding; i++)
                _writer.Write((byte)0);
        }

        public override void XdrEncodeOpaque(byte[] value)
        {
            XdrEncodeInt(value.Length);
            _writer.Write(value);
            int padding = (4 - (value.Length % 4)) % 4;
            for (int i = 0; i < padding; i++)
                _writer.Write((byte)0);
        }

        public byte[] GetBytes()
        {
            _writer.Flush();
            return _buffer.ToArray();
        }

        public override void Dispose()
        {
            _writer.Dispose();
            _buffer.Dispose();
        }
    }
} 