using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Args
{
    public sealed class WriteArgs : NFSv3Args
    {
        public NFSv3Handle FileHandle { get; }
        public XdrHyper Offset { get; } = new();
        public XdrInt Count { get; } = new();
        public XdrOpaque Data { get; }
        public XdrInt StableHow { get; } = new();

        public WriteArgs(NFSv3Handle fileHandle, long offset, ReadOnlyMemory<byte> data, bool stable = true)
        {
            if (fileHandle == null)
                throw new ArgumentNullException(nameof(fileHandle));
            
            FileHandle = fileHandle;
            Offset.Value = offset;
            Data = new XdrOpaque(data);
            Count.Value = data.Length;
            StableHow.Value = stable ? 2 : 0; // FILE_SYNC = 2, UNSTABLE = 0
        }

        public override async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await FileHandle.XdrEncodeAsync(xdr);
            await Offset.XdrEncodeAsync(xdr);
            await Count.XdrEncodeAsync(xdr);
            await StableHow.XdrEncodeAsync(xdr);
            await Data.XdrEncodeAsync(xdr);
        }

        public override ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side args");
        }
    }
}