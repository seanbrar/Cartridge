using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class ReadDirPlusResult : NFSv3Result
    {
        public NFSv3FileAttributes? DirAttributes { get; private set; }
        public XdrOpaque CookieVerf { get; private set; } = new(8);
        public List<Entry> Entries { get; private set; } = new();
        public XdrBoolean Eof { get; private set; } = new();

        public class Entry : IXdrData
        {
            public XdrHyper FileId { get; set; } = new();
            public XdrString Name { get; set; } = new();
            public XdrHyper Cookie { get; set; } = new();
            public NFSv3Handle? Handle { get; set; }
            public NFSv3FileAttributes? Attributes { get; set; }

            public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
            {
                await FileId.XdrEncodeAsync(xdr);
                await Name.XdrEncodeAsync(xdr);
                await Cookie.XdrEncodeAsync(xdr);

                var hasHandle = new XdrBoolean(Handle != null);
                await hasHandle.XdrEncodeAsync(xdr);
                if (Handle != null)
                {
                    await Handle.XdrEncodeAsync(xdr);
                }

                var hasAttributes = new XdrBoolean(Attributes != null);
                await hasAttributes.XdrEncodeAsync(xdr);
                if (Attributes != null)
                {
                    await Attributes.XdrEncodeAsync(xdr);
                }
            }

            public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
            {
                await FileId.XdrDecodeAsync(xdr);
                await Name.XdrDecodeAsync(xdr);
                await Cookie.XdrDecodeAsync(xdr);

                var hasHandle = new XdrBoolean();
                await hasHandle.XdrDecodeAsync(xdr);
                if (hasHandle.Value)
                {
                    Handle = new NFSv3Handle(forDecoding: true);
                    await Handle.XdrDecodeAsync(xdr);
                }

                var hasAttributes = new XdrBoolean();
                await hasAttributes.XdrDecodeAsync(xdr);
                if (hasAttributes.Value)
                {
                    Attributes = new NFSv3FileAttributes();
                    await Attributes.XdrDecodeAsync(xdr);
                }
            }
        }

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                var hasDirAttributes = new XdrBoolean();
                await hasDirAttributes.XdrDecodeAsync(xdr);
                if (hasDirAttributes.Value)
                {
                    DirAttributes = new NFSv3FileAttributes();
                    await DirAttributes.XdrDecodeAsync(xdr);
                }

                await CookieVerf.XdrDecodeAsync(xdr);

                var hasMoreEntries = new XdrBoolean();
                while (true)
                {
                    await hasMoreEntries.XdrDecodeAsync(xdr);
                    if (!hasMoreEntries.Value)
                        break;

                    var entry = new Entry();
                    await entry.XdrDecodeAsync(xdr);
                    Entries.Add(entry);
                }

                await Eof.XdrDecodeAsync(xdr);
            }
        }
    }
}