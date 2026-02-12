namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Defines the contract for a buffer pool that can rent and return Memory<byte> buffers.
    /// </summary>
    public interface IBufferPool
    {
        ValueTask<Memory<byte>> RentAsync(int minimumSize);
        ValueTask ReturnAsync(Memory<byte> buffer);
    }
}