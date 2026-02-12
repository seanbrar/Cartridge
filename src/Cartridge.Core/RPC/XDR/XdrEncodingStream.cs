using System;
using System.Net;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Base class for XDR encoding operations.
    /// </summary>
    public abstract class XdrEncodingStream : XdrStream
    {
        protected XdrEncodingStream(int initialCapacity = 8192)
            : base(initialCapacity)
        {
        }

        public abstract ValueTask EncodeIntAsync(int value);
        public abstract ValueTask EncodeLongAsync(long value);
        public abstract ValueTask EncodeFloatAsync(float value);
        public abstract ValueTask EncodeDoubleAsync(double value);
        public abstract ValueTask EncodeBooleanAsync(bool value);
        public abstract ValueTask EncodeStringAsync(string value);
        public abstract ValueTask EncodeOpaqueAsync(ReadOnlyMemory<byte> value);

        public virtual async ValueTask EncodeFixedOpaqueAsync(ReadOnlyMemory<byte> value, int length)
        {
            if (value.Length != length)
                throw new ArgumentException($"Fixed opaque data must be exactly {length} bytes");
            await EncodeOpaqueAsync(value);
        }

        public virtual async ValueTask EncodeVariableOpaqueAsync(ReadOnlyMemory<byte> value, int maxLength)
        {
            if (value.Length > maxLength)
                throw new ArgumentException($"Variable opaque data must not exceed {maxLength} bytes");
            await EncodeOpaqueAsync(value);
        }

        public virtual async ValueTask EncodeIPAddressAsync(IPAddress value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var addressBytes = value.GetAddressBytes();
            if (addressBytes.Length != 4)
                throw new ArgumentException("Only IPv4 addresses are supported", nameof(value));

            await EncodeOpaqueAsync(addressBytes);
        }
    }
}