using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.RPC.Auth
{
    public interface IAuthProvider
    {
        ValueTask AuthenticateAsync(XdrEncodingStream xdr, CancellationToken cancellation = default);
        int AuthenticationType { get; }
    }

    public static class AuthTypes
    {
        public const int None = 0;
        public const int Unix = 1;
        public const int Short = 2;
    }
}