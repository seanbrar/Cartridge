using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Data.Results
{
    public class PathConfResult : NFSv3Result
    {
        public NFSv3FileAttributes? Attributes { get; private set; }
        public NFSv3FileAttributes? PostOpAttributes { get; private set; }
        public XdrInt LinkMax { get; private set; } = new();  // XdrUnsignedInt
        public XdrInt NameMax { get; private set; } = new();  // XdrUnsignedInt
        public XdrBoolean NoTrunc { get; private set; } = new();
        public XdrBoolean ChownRestricted { get; private set; } = new();
        public XdrBoolean CaseInsensitive { get; private set; } = new();
        public XdrBoolean CasePreserving { get; private set; } = new();

        public override ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            throw new NotImplementedException("Encoding not needed for client-side results");
        }

        public override async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            await Status.XdrDecodeAsync(xdr);

            if (Status.Value == NFSv3Status.NFS3_OK)
            {
                var hasAttributes = new XdrBoolean();
                await hasAttributes.XdrDecodeAsync(xdr);
                if (hasAttributes.Value)
                {
                    Attributes = new NFSv3FileAttributes();
                    await Attributes.XdrDecodeAsync(xdr);
                }

                await LinkMax.XdrDecodeAsync(xdr);
                await NameMax.XdrDecodeAsync(xdr);
                await NoTrunc.XdrDecodeAsync(xdr);
                await ChownRestricted.XdrDecodeAsync(xdr);
                await CaseInsensitive.XdrDecodeAsync(xdr);
                await CasePreserving.XdrDecodeAsync(xdr);
            }
        }
    }
}