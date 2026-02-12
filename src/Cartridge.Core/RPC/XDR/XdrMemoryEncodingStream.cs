using System.Buffers.Binary;
using System.Text;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Implements XDR encoding into a memory buffer with async support.
    /// </summary>
    public sealed class XdrMemoryEncodingStream : XdrEncodingStream
    {
        private const int DefaultInitialCapacity = 8192;

        public XdrMemoryEncodingStream(int initialCapacity = DefaultInitialCapacity)
            : base(initialCapacity)
        {
        }

        public ReadOnlyMemory<byte> GetEncodedData() => Buffer[..Position];

        public override async ValueTask EncodeIntAsync(int value)
        {
            await EnsureBufferSizeAsync(sizeof(int));
            BinaryPrimitives.WriteInt32BigEndian(Buffer.Span[Position..], value);
            Position += sizeof(int);
        }

        public override async ValueTask EncodeLongAsync(long value)
        {
            await EnsureBufferSizeAsync(sizeof(long));
            BinaryPrimitives.WriteInt64BigEndian(Buffer.Span[Position..], value);
            Position += sizeof(long);
        }

        public override async ValueTask EncodeFloatAsync(float value)
        {
            await EnsureBufferSizeAsync(sizeof(float));
            var intValue = BitConverter.SingleToInt32Bits(value);
            BinaryPrimitives.WriteInt32BigEndian(Buffer.Span[Position..], intValue);
            Position += sizeof(float);
        }

        public override async ValueTask EncodeDoubleAsync(double value)
        {
            await EnsureBufferSizeAsync(sizeof(double));
            var longValue = BitConverter.DoubleToInt64Bits(value);
            BinaryPrimitives.WriteInt64BigEndian(Buffer.Span[Position..], longValue);
            Position += sizeof(double);
        }

        public override async ValueTask EncodeBooleanAsync(bool value)
        {
            await EncodeIntAsync(value ? 1 : 0);
        }

        public override async ValueTask EncodeStringAsync(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var bytes = Encoding.UTF8.GetBytes(value);
            await EncodeIntAsync(bytes.Length);
            await EncodeOpaqueAsync(bytes);
        }

        public override async ValueTask EncodeOpaqueAsync(ReadOnlyMemory<byte> value)
        {
            var totalLength = value.Length;
            var padding = (4 - (totalLength % 4)) % 4;
            
            await EnsureBufferSizeAsync(totalLength + padding);
            value.CopyTo(Buffer.Slice(Position));
            Position += totalLength + padding; // Skip over padding bytes (they're already zeroed)
        }
    }
}