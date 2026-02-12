using System.Threading;
using System.Threading.Tasks;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.RPC.Core
{
    public interface IRpcTransport : IAsyncDisposable
    {
        ValueTask ConnectAsync(CancellationToken cancellation = default);
        ValueTask DisconnectAsync(CancellationToken cancellation = default);
        ValueTask<TResponse> CallAsync<TRequest, TResponse>(
            int procedure,
            TRequest request,
            CancellationToken cancellation = default)
            where TRequest : IXdrData
            where TResponse : IXdrData;
    }
}