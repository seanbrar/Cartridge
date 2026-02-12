using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.RPC.Auth
{
    public class NullAuthProvider : IAuthProvider
    {
        public int AuthenticationType => AuthTypes.None;

        public async ValueTask AuthenticateAsync(XdrEncodingStream xdr, CancellationToken cancellation = default)
        {
            await xdr.EncodeIntAsync(AuthTypes.None);
            await xdr.EncodeIntAsync(0);
            await xdr.EncodeIntAsync(AuthTypes.None);
            await xdr.EncodeIntAsync(0);
        }
    }
} 