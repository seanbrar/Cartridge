using System.Net;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.RPC.Auth
{
    public class UnixAuthProvider : IAuthProvider
    {
        private readonly string _machineName;
        private readonly int _uid;
        private readonly int _gid;
        private readonly IReadOnlyList<int> _gids;
        private readonly int _stamp;

        public int AuthenticationType => AuthTypes.Unix;

        public UnixAuthProvider(
            string? machineName = null,
            int uid = 0,
            int gid = 0,
            IEnumerable<int>? gids = null,
            int stamp = 0)
        {
            _machineName = machineName ?? Dns.GetHostName();
            _uid = uid;
            _gid = gid;
            _gids = (gids ?? Array.Empty<int>()).ToList().AsReadOnly();
            _stamp = stamp;
        }

        public async ValueTask AuthenticateAsync(XdrEncodingStream xdr, CancellationToken cancellation = default)
        {
            await xdr.EncodeIntAsync(AuthenticationType);
            await xdr.EncodeIntAsync(28 + _machineName.Length + (4 * _gids.Count)); // credential length
            await xdr.EncodeStringAsync(_machineName);
            await xdr.EncodeIntAsync(_uid);
            await xdr.EncodeIntAsync(_gid);
            await xdr.EncodeIntAsync(_gids.Count);
            foreach (int gid in _gids)
            {
                await xdr.EncodeIntAsync(gid);
            }
            await xdr.EncodeIntAsync(_stamp);

            // Verifier (AUTH_NONE)
            await xdr.EncodeIntAsync(AuthTypes.None);
            await xdr.EncodeIntAsync(0);
        }
    }
}