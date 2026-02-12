namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Interface for XDR encodable/decodable types.
    /// </summary>
    public interface IXdrType
    {
        ValueTask XdrEncodeAsync(XdrEncodingStream xdr);
        ValueTask XdrDecodeAsync(XdrDecodingStream xdr);
    }
} 