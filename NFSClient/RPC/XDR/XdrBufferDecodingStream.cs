using System;
using System.Net;
using System.Text;

namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Implements XDR decoding from a memory buffer.
    /// </summary>
    public class XdrBufferDecodingStream : XdrDecodingStream
    {
        private readonly byte[] _buffer;
        private int _position;

        public XdrBufferDecodingStream(byte[] buffer)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _position = 0;
        }

        public override bool XdrDecodeBoolean()
        {
            return XdrDecodeInt() != 0;
        }

        public override byte XdrDecodeByte()
        {
            EnsureSpace(4);
            _position += 3; // Skip padding
            return _buffer[_position++];
        }

        public override short XdrDecodeShort()
        {
            return (short)XdrDecodeInt();
        }

        public override int XdrDecodeInt()
        {
            EnsureSpace(4);
            int value = (_buffer[_position++] << 24) |
                       (_buffer[_position++] << 16) |
                       (_buffer[_position++] << 8) |
                       _buffer[_position++];
            return value;
        }

        public override long XdrDecodeLong()
        {
            EnsureSpace(8);
            long value = ((long)_buffer[_position++] << 56) |
                        ((long)_buffer[_position++] << 48) |
                        ((long)_buffer[_position++] << 40) |
                        ((long)_buffer[_position++] << 32) |
                        ((long)_buffer[_position++] << 24) |
                        ((long)_buffer[_position++] << 16) |
                        ((long)_buffer[_position++] << 8) |
                        _buffer[_position++];
            return value;
        }

        public override float XdrDecodeFloat()
        {
            int bits = XdrDecodeInt();
            return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0);
        }

        public override double XdrDecodeDouble()
        {
            long bits = XdrDecodeLong();
            return BitConverter.ToDouble(BitConverter.GetBytes(bits), 0);
        }

        public override string XdrDecodeString()
        {
            int length = XdrDecodeInt();
            if (length == 0)
                return string.Empty;

            byte[] bytes = XdrDecodeOpaque();
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        public override byte[] XdrDecodeOpaque()
        {
            int length = XdrDecodeInt();
            if (length == 0)
                return Array.Empty<byte>();

            EnsureSpace(length);
            byte[] result = new byte[length];
            Array.Copy(_buffer, _position, result, 0, length);
            _position += length;

            // Skip padding
            int padding = (4 - (length & 3)) & 3;
            _position += padding;

            return result;
        }

        public override IPAddress XdrDecodeIPAddress()
        {
            byte[] addressBytes = XdrDecodeOpaque();
            if (addressBytes.Length != 4)
                throw new InvalidOperationException("Invalid IP address length");

            return new IPAddress(addressBytes);
        }

        private void EnsureSpace(int needed)
        {
            if (_position + needed > _buffer.Length)
                throw new InvalidOperationException("Buffer underflow");
        }

        public override void Dispose()
        {
            // No unmanaged resources to dispose
        }
    }
} 