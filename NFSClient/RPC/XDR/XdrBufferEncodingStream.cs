using System;
using System.Net;
using System.Text;

namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Implements XDR encoding into a memory buffer.
    /// </summary>
    public class XdrBufferEncodingStream : XdrEncodingStream
    {
        private readonly byte[] _buffer;
        private int _position;

        public XdrBufferEncodingStream(int size)
        {
            _buffer = new byte[size];
            _position = 0;
        }

        public byte[] GetBuffer() => _buffer;
        public int GetBufferLength() => _position;

        public override void XdrEncodeBoolean(bool value)
        {
            XdrEncodeInt(value ? 1 : 0);
        }

        public override void XdrEncodeByte(byte value)
        {
            EnsureSpace(4);
            _buffer[_position++] = 0;
            _buffer[_position++] = 0;
            _buffer[_position++] = 0;
            _buffer[_position++] = value;
        }

        public override void XdrEncodeShort(short value)
        {
            XdrEncodeInt(value);
        }

        public override void XdrEncodeInt(int value)
        {
            EnsureSpace(4);
            _buffer[_position++] = (byte)((value >> 24) & 0xFF);
            _buffer[_position++] = (byte)((value >> 16) & 0xFF);
            _buffer[_position++] = (byte)((value >> 8) & 0xFF);
            _buffer[_position++] = (byte)(value & 0xFF);
        }

        public override void XdrEncodeLong(long value)
        {
            EnsureSpace(8);
            _buffer[_position++] = (byte)((value >> 56) & 0xFF);
            _buffer[_position++] = (byte)((value >> 48) & 0xFF);
            _buffer[_position++] = (byte)((value >> 40) & 0xFF);
            _buffer[_position++] = (byte)((value >> 32) & 0xFF);
            _buffer[_position++] = (byte)((value >> 24) & 0xFF);
            _buffer[_position++] = (byte)((value >> 16) & 0xFF);
            _buffer[_position++] = (byte)((value >> 8) & 0xFF);
            _buffer[_position++] = (byte)(value & 0xFF);
        }

        public override void XdrEncodeFloat(float value)
        {
            XdrEncodeInt(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
        }

        public override void XdrEncodeDouble(double value)
        {
            XdrEncodeLong(BitConverter.ToInt64(BitConverter.GetBytes(value), 0));
        }

        public override void XdrEncodeString(string value)
        {
            if (value == null)
            {
                XdrEncodeInt(0);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            XdrEncodeInt(bytes.Length);
            XdrEncodeOpaque(bytes);
        }

        public override void XdrEncodeOpaque(byte[] value)
        {
            if (value == null)
            {
                XdrEncodeInt(0);
                return;
            }

            int length = value.Length;
            int padding = (4 - (length & 3)) & 3;
            
            EnsureSpace(length + padding);
            Array.Copy(value, 0, _buffer, _position, length);
            _position += length;

            // Add padding
            while (padding-- > 0)
            {
                _buffer[_position++] = 0;
            }
        }

        public override void XdrEncodeIPAddress(IPAddress value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            byte[] addressBytes = value.GetAddressBytes();
            if (addressBytes.Length != 4)
                throw new ArgumentException("Only IPv4 addresses are supported", nameof(value));

            XdrEncodeOpaque(addressBytes);
        }

        private void EnsureSpace(int needed)
        {
            if (_position + needed > _buffer.Length)
                throw new InvalidOperationException("Buffer overflow");
        }
    }
} 