using Cartridge.Core.NFS.Data.Attributes;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents a serializable XDR boolean value.
    /// </summary>
    public sealed class XdrBoolean : IXdrData
    {
        // Simple get/set - no need for init-only or complex immutability
        public bool Value { get; set; }

        public XdrBoolean(bool value = false) => Value = value;

        // Clean, straightforward async methods
        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            => xdr.EncodeBooleanAsync(Value);

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Value = await xdr.DecodeBooleanAsync();
        }

        // Convenient conversions
        public static implicit operator bool(XdrBoolean value) => value.Value;
        public static implicit operator XdrBoolean(bool value) => new(value);

        public static implicit operator XdrBoolean(NFSv3FileAttributes v)
        {
            throw new NotImplementedException();
        }
    }
}