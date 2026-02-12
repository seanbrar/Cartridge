using System;
using System.Buffers.Binary;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Implements XDR decoding with buffer support.
    /// </summary>
    public sealed class XdrBufferDecodingStream : XdrDecodingStream
    {
        public XdrBufferDecodingStream(ReadOnlyMemory<byte> initialData)
            : base(initialData.Length)
        {
            ArgumentOutOfRangeException.ThrowIfZero(initialData.Length);
            initialData.CopyTo(Buffer);
        }

        public override ValueTask<int> DecodeIntAsync()
        {
            ValidateBufferBounds(sizeof(int));
            var value = BinaryPrimitives.ReadInt32BigEndian(Buffer.Span[Position..]);
            Position += sizeof(int);
            return new ValueTask<int>(value);
        }

        public override ValueTask<long> DecodeLongAsync()
        {
            ValidateBufferBounds(sizeof(long));
            var value = BinaryPrimitives.ReadInt64BigEndian(Buffer.Span[Position..]);
            Position += sizeof(long);
            return new ValueTask<long>(value);
        }

        public override ValueTask<float> DecodeFloatAsync()
        {
            ValidateBufferBounds(sizeof(float));
            var intValue = BinaryPrimitives.ReadInt32BigEndian(Buffer.Span[Position..]);
            Position += sizeof(float);
            return new ValueTask<float>(BitConverter.Int32BitsToSingle(intValue));
        }

        public override ValueTask<double> DecodeDoubleAsync()
        {
            ValidateBufferBounds(sizeof(double));
            var longValue = BinaryPrimitives.ReadInt64BigEndian(Buffer.Span[Position..]);
            Position += sizeof(double);
            return new ValueTask<double>(BitConverter.Int64BitsToDouble(longValue));
        }

        public override async ValueTask<bool> DecodeBooleanAsync()
        {
            var value = await DecodeIntAsync();
            return value != 0;
        }

        public override async ValueTask<string> DecodeStringAsync()
        {
            var length = await DecodeIntAsync();
            if (length < 0)
                throw new XdrException("Invalid string length");
            if (length == 0)
                return string.Empty;

            var bytes = await DecodeOpaqueInternalAsync(length);
            return Encoding.UTF8.GetString(bytes.Span);
        }

        public override async ValueTask<Memory<byte>> DecodeOpaqueAsync()
        {
            var length = await DecodeIntAsync();
            if (length < 0)
                throw new XdrException("Invalid opaque length");
            return await DecodeOpaqueInternalAsync(length);
        }

        private ValueTask<Memory<byte>> DecodeOpaqueInternalAsync(int length)
        {
            if (length == 0)
                return new ValueTask<Memory<byte>>(Memory<byte>.Empty);

            var padding = GetPadding(length);
            var totalLength = length + padding;
            
            ValidateBufferBounds(totalLength);
            var result = Buffer.Slice(Position, length);
            Position += totalLength;

            return new ValueTask<Memory<byte>>(result);
        }

        public override async ValueTask<IPAddress> DecodeIPAddressAsync()
        {
            var addressBytes = await DecodeOpaqueAsync();
            if (addressBytes.Length != 4)
                throw new XdrException("Invalid IP address length");

            return new IPAddress(addressBytes.Span);
        }
    }
}