using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Attributes
{
    public class NFSv3SetAttributes : IXdrData
    {
        public uint? Mode { get; set; }
        public uint? Uid { get; set; }
        public uint? Gid { get; set; }
        public ulong? Size { get; set; }
        public NFSv3TimeValue? AccessTime { get; set; }
        public NFSv3TimeValue? ModifyTime { get; set; }

        public NFSv3SetAttributes()
        {
        }

        public NFSv3SetAttributes(uint mode)
        {
            Mode = mode;
        }

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            // Encode each field with a "set" flag followed by the value if set
            await xdr.EncodeBooleanAsync(Mode.HasValue);
            if (Mode.HasValue) await xdr.EncodeIntAsync((int)Mode.Value);

            await xdr.EncodeBooleanAsync(Uid.HasValue);
            if (Uid.HasValue) await xdr.EncodeIntAsync((int)Uid.Value);

            await xdr.EncodeBooleanAsync(Gid.HasValue);
            if (Gid.HasValue) await xdr.EncodeIntAsync((int)Gid.Value);

            await xdr.EncodeBooleanAsync(Size.HasValue);
            if (Size.HasValue) await xdr.EncodeLongAsync((long)Size.Value);

            await xdr.EncodeBooleanAsync(AccessTime != null);
            if (AccessTime != null)
            {
                await xdr.EncodeIntAsync((int)AccessTime.Seconds);
                await xdr.EncodeIntAsync((int)AccessTime.NanoSeconds);
            }

            await xdr.EncodeBooleanAsync(ModifyTime != null);
            if (ModifyTime != null)
            {
                await xdr.EncodeIntAsync((int)ModifyTime.Seconds);
                await xdr.EncodeIntAsync((int)ModifyTime.NanoSeconds);
            }
        }

        public ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            throw new NotImplementedException("Decoding not needed for client-side attributes");
        }
    }

    public class NFSv3TimeValue
    {
        public uint Seconds { get; set; }
        public uint NanoSeconds { get; set; }

        public NFSv3TimeValue(uint seconds, uint nanoSeconds = 0)
        {
            Seconds = seconds;
            NanoSeconds = nanoSeconds;
        }
    }
}
