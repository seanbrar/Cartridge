using System.Buffers.Binary;
using System.Text;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Implements XDR decoding from a memory buffer with async support.
    /// </summary>
    public sealed class XdrMemoryDecodingStream : XdrDecodingStream
    {
        public XdrMemoryDecodingStream(ReadOnlyMemory<byte> data)
            : base(data.Length)
        {
            data.CopyTo(Buffer);
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

            var padding = (4 - (length % 4)) % 4;
            var totalLength = length + padding;
            
            ValidateBufferBounds(totalLength);
            var result = Buffer.Slice(Position, length);
            Position += totalLength;

            return new ValueTask<Memory<byte>>(result);
        }
    }
}