using System;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents an XDR hyper integer (64-bit).
    /// </summary>
    public sealed class XdrHyper : IXdrType
    {
        public long Value { get; set; }

        public XdrHyper(long value = 0)
        {
            Value = value;
        }

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await xdr.EncodeLongAsync(Value);
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeLongAsync();
        }
    }
} 