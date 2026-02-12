using System;
using System.Buffers.Binary;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Implements XDR encoding with pooled buffer support.
    /// </summary>
    public sealed class XdrBufferEncodingStream : XdrEncodingStream
    {
        public XdrBufferEncodingStream(int initialCapacity = 8192)
            : base(initialCapacity)
        {
        }

        public byte[] ToArray()
        {
            return Buffer[..Position].ToArray();
        }

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
            ArgumentNullException.ThrowIfNull(value);

            var bytes = Encoding.UTF8.GetBytes(value);
            await EncodeIntAsync(bytes.Length);
            await EncodeOpaqueAsync(bytes);
        }

        public override async ValueTask EncodeOpaqueAsync(ReadOnlyMemory<byte> value)
        {
            var totalLength = value.Length;
            var padding = GetPadding(totalLength);
            
            await EnsureBufferSizeAsync(totalLength + padding);
            value.CopyTo(Buffer[Position..]);
            Position += totalLength + padding; // Skip over padding bytes (they're already zeroed)
        }
    }
}