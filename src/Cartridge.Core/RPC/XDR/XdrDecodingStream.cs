using System;
using System.Net;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Base class for XDR decoding operations.
    /// </summary>
    public abstract class XdrDecodingStream : XdrStream
    {
        protected XdrDecodingStream(int initialCapacity = 8192)
            : base(initialCapacity)
        {
        }

        public abstract ValueTask<int> DecodeIntAsync();
        public abstract ValueTask<long> DecodeLongAsync();
        public abstract ValueTask<float> DecodeFloatAsync();
        public abstract ValueTask<double> DecodeDoubleAsync();
        public abstract ValueTask<bool> DecodeBooleanAsync();
        public abstract ValueTask<string> DecodeStringAsync();
        public abstract ValueTask<Memory<byte>> DecodeOpaqueAsync();

        public virtual async ValueTask<Memory<byte>> DecodeFixedOpaqueAsync(int length)
        {
            var value = await DecodeOpaqueAsync();
            if (value.Length != length)
                throw new ArgumentException($"Fixed opaque data must be exactly {length} bytes");
            return value;
        }

        public virtual async ValueTask<Memory<byte>> DecodeVariableOpaqueAsync(int maxLength)
        {
            var value = await DecodeOpaqueAsync();
            if (value.Length > maxLength)
                throw new ArgumentException($"Variable opaque data must not exceed {maxLength} bytes");
            return value;
        }

        public virtual async ValueTask<IPAddress> DecodeIPAddressAsync()
        {
            var addressBytes = await DecodeOpaqueAsync();
            if (addressBytes.Length != 4)
                throw new InvalidOperationException("Invalid IP address length");

            return new IPAddress(addressBytes.Span);
        }
    }
}