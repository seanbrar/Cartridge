using System.Net;
using NFSClient.RPC.XDR;

namespace NFSLibrary.RPC.Auth
{
    public class OncRpcClientAuthUnix : IXdrData
    {
        private readonly string _machineName;
        private readonly int _uid;
        private readonly int _gid;
        private readonly int[] _gids;
        private readonly int _stamp;

        public OncRpcClientAuthUnix(string machineName, int uid = 0, int gid = 0, int[]? gids = null, int stamp = 0)
        {
            _machineName = machineName ?? Dns.GetHostName();
            _uid = uid;
            _gid = gid;
            _gids = gids ?? Array.Empty<int>();
            _stamp = stamp;
        }

        public void XdrEncode(XdrEncodingStream xdr)
        {
            xdr.xdrEncodeString(_machineName);
            xdr.xdrEncodeInt(_uid);
            xdr.xdrEncodeInt(_gid);
            xdr.xdrEncodeInt(_gids.Length);
            foreach (int gid in _gids)
            {
                xdr.xdrEncodeInt(gid);
            }
            xdr.xdrEncodeInt(_stamp);
        }

        public void XdrDecode(XdrDecodingStream xdr)
        {
            throw new NotSupportedException("Decoding of Unix authentication credentials is not supported");
        }
    }
} 