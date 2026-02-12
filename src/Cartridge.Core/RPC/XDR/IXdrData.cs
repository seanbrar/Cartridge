namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Defines the contract for XDR serializable data.
    /// </summary>
    public interface IXdrData
    {
        ValueTask XdrEncodeAsync(XdrEncodingStream xdr);
        ValueTask XdrDecodeAsync(XdrDecodingStream xdr);
    }
}
