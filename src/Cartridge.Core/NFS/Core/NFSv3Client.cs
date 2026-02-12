using System;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using Cartridge.Core.NFS.Data.Results;
using Cartridge.Core.NFS.Data.Args;
using Cartridge.Core.NFS.Types;
using Cartridge.Core.RPC.Core;
using Cartridge.Core.NFS.Cache;
using Cartridge.Core.NFS.Data.Attributes;
using Cartridge.Core.NFS.Exceptions;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Core
{
    public class NFSv3Client : INFSv3Operations
    {
        private readonly IRpcTransport _transport;
        private readonly HandleCache _handleCache;
        private readonly DirectoryCache _dirCache;
        private readonly AttributeCache _attrCache;
        private volatile NFSConnectionState _connectionState;
        private readonly SemaphoreSlim _stateLock = new(1, 1);
        private readonly int _defaultChunkSize;
        private NFSPermission _defaultPermission = NFSPermission.Default;

        public NFSPermission DefaultPermission
        {
            get => _defaultPermission;
            set => _defaultPermission = value;
        }

        public NFSv3Client(IRpcTransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _handleCache = new HandleCache();
            _dirCache = new DirectoryCache();
            _attrCache = new AttributeCache();
            _connectionState = NFSConnectionState.Disconnected;
            _defaultChunkSize = NFSv3Constants.DEFAULT_CHUNK_SIZE;
        }

        public NFSConnectionState ConnectionState => _connectionState;

        private async Task EnsureStateAsync(NFSConnectionState requiredState, CancellationToken ct)
        {
            await Task.CompletedTask;
            if (_connectionState != requiredState)
            {
                throw new NFSv3Exception($"Invalid connection state. Required: {requiredState}, Current: {_connectionState}");
            }
        }

        private async Task TransitionStateAsync(NFSConnectionState newState, CancellationToken ct)
        {
            await _stateLock.WaitAsync(ct);
            try
            {
                // Add validation logic for valid state transitions
                _connectionState = newState;
            }
            finally
            {
                _stateLock.Release();
            }
        }

        // Essential fast-path operations
        public async Task<LookupResult> LookupAsync(NFSv3Handle dirHandle, string name, CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Mounted, ct);

            var args = new LookupArgs(dirHandle, name);
            var result = await CallWithTimeoutAsync<LookupArgs, LookupResult>(
                NFSv3Constants.NFSPROC3_LOOKUP,
                args,
                isQuickOperation: true,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                if (result.Handle != null)
                {
                    _handleCache.Add(result.Handle);
                }
                if (result.Attributes != null)
                {
                    _attrCache.Update(result.Handle!, result.Attributes);
                }
            }

            return result;
        }

        public async Task<ReadDirPlusResult> ReadDirPlusAsync(
            NFSv3Handle dirHandle,
            ulong cookie,
            CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Mounted, ct);

            var args = new ReadDirPlusArgs(dirHandle, cookie);
            var result = await CallWithTimeoutAsync<ReadDirPlusArgs, ReadDirPlusResult>(
                NFSv3Constants.NFSPROC3_READDIRPLUS,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK && result.DirAttributes != null)
            {
                _dirCache.Update(
                    dirHandle,
                    result.Entries,
                    result.CookieVerf.Value.ToArray(),
                    result.DirAttributes,
                    result.Eof.Value
                );

                foreach (var entry in result.Entries.Where(e => e.Handle != null && e.Attributes != null))
                {
                    _handleCache.Add(entry.Handle!);
                    _attrCache.Update(entry.Handle!, entry.Attributes!);
                }
            }

            return result;
        }

        public async Task<ReadResult> ReadAsync(
            NFSv3Handle fileHandle,
            long offset,
            int count,
            CancellationToken ct = default)
        {
            var args = new ReadArgs(fileHandle, offset, count);
            var result = await CallWithTimeoutAsync<ReadArgs, ReadResult>(
                NFSv3Constants.NFSPROC3_READ,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK && result.Attributes != null)
            {
                _attrCache.Update(fileHandle, result.Attributes);
            }

            return result;
        }

        public async Task<GetAttrResult> GetAttrAsync(
            NFSv3Handle fileHandle,
            CancellationToken ct = default)
        {
            // Check cache first
            if (_attrCache.TryGet(fileHandle, out var cachedAttrs))
            {
                return new GetAttrResult
                {
                    Status = new XdrNFSv3Status(NFSv3Status.NFS3_OK),
                    Attributes = cachedAttrs
                };
            }

            var args = new GetAttrArgs(fileHandle);
            var result = await CallWithTimeoutAsync<GetAttrArgs, GetAttrResult>(
                NFSv3Constants.NFSPROC3_GETATTR,
                args,
                isQuickOperation: true,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK && result.Attributes != null)
            {
                _attrCache.Update(fileHandle, result.Attributes);
            }

            return result;
        }

        public async Task<CreateResult> CreateAsync(
            NFSv3Handle dirHandle,
            string name,
            NFSv3SetAttributes attributes,
            CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Mounted, ct);

            var args = new CreateArgs(dirHandle, name, attributes);
            var result = await CallWithTimeoutAsync<CreateArgs, CreateResult>(
                NFSv3Constants.NFSPROC3_CREATE,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                if (result.Handle != null)
                {
                    _handleCache.Add(result.Handle);
                }
                if (result.PostOpAttributes != null)
                {
                    _attrCache.Update(result.Handle!, result.PostOpAttributes);
                }
                // Invalidate directory cache since we modified its contents
                _dirCache.Invalidate(dirHandle);
            }

            return result;
        }

        public async Task<RemoveResult> RemoveAsync(
            NFSv3Handle dirHandle,
            string name,
            CancellationToken ct = default)
        {
            var args = new RemoveArgs(dirHandle, name);
            var result = await CallWithTimeoutAsync<RemoveArgs, RemoveResult>(
                NFSv3Constants.NFSPROC3_REMOVE,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                // Invalidate directory cache since we modified its contents
                _dirCache.Invalidate(dirHandle);
            }

            return result;
        }

        public async Task<MkDirResult> MkDirAsync(
            NFSv3Handle dirHandle,
            string name,
            NFSv3SetAttributes attributes,
            CancellationToken ct = default)
        {
            var args = new MkDirArgs(dirHandle, name, attributes);
            var result = await CallWithTimeoutAsync<MkDirArgs, MkDirResult>(
                NFSv3Constants.NFSPROC3_MKDIR,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                if (result.Handle != null)
                {
                    _handleCache.Add(result.Handle);
                }
                if (result.Attributes != null)
                {
                    _attrCache.Update(result.Handle!, result.Attributes);
                }
                // Invalidate directory cache since we modified its contents
                _dirCache.Invalidate(dirHandle);
            }

            return result;
        }

        public async Task<RmDirResult> RmDirAsync(
            NFSv3Handle dirHandle,
            string name,
            CancellationToken ct = default)
        {
            var args = new RmDirArgs(dirHandle, name);
            var result = await CallWithTimeoutAsync<RmDirArgs, RmDirResult>(
                NFSv3Constants.NFSPROC3_RMDIR,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                // Invalidate directory cache since we modified its contents
                _dirCache.Invalidate(dirHandle);
            }

            return result;
        }

        public async Task<FsInfoResult> GetFsInfoAsync(
            NFSv3Handle rootHandle,
            CancellationToken ct = default)
        {
            var args = new FsInfoArgs(rootHandle);
            var result = await CallWithTimeoutAsync<FsInfoArgs, FsInfoResult>(
                NFSv3Constants.NFSPROC3_FSINFO,
                args,
                isQuickOperation: true,
                ct
            );

            // No cache updates needed for FSINFO
            return result;
        }

        public async Task<FsStatResult> GetFsStatAsync(
            NFSv3Handle rootHandle,
            CancellationToken ct = default)
        {
            var args = new FsStatArgs(rootHandle);
            var result = await CallWithTimeoutAsync<FsStatArgs, FsStatResult>(
                NFSv3Constants.NFSPROC3_FSSTAT,
                args,
                isQuickOperation: true,
                ct
            );

            // No cache updates needed for FSSTAT
            return result;
        }

        public async Task<AccessResult> CheckAccessAsync(
            NFSv3Handle fileHandle,
            uint accessToCheck = NFSv3Constants.ACCESS_READ,
            CancellationToken ct = default)
        {
            var args = new AccessArgs(fileHandle, accessToCheck);
            var result = await CallWithTimeoutAsync<AccessArgs, AccessResult>(
                NFSv3Constants.NFSPROC3_ACCESS,
                args,
                isQuickOperation: true,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK && result.Attributes != null)
            {
                _attrCache.Update(fileHandle, result.Attributes);
            }

            return result;
        }

        public async Task<PathConfResult> GetPathConfAsync(
            NFSv3Handle fileHandle,
            CancellationToken ct = default)
        {
            var args = new PathConfArgs(fileHandle);
            var result = await CallWithTimeoutAsync<PathConfArgs, PathConfResult>(
                NFSv3Constants.NFSPROC3_PATHCONF,
                args,
                isQuickOperation: true,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK && result.Attributes != null)
            {
                _attrCache.Update(fileHandle, result.Attributes);
            }

            return result;
        }

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            if (_connectionState != NFSConnectionState.Disconnected)
            {
                throw new NFSv3Exception("Already connected or mounted");
            }

            // Assuming _transport has connect functionality
            await _transport.ConnectAsync(ct);
            await TransitionStateAsync(NFSConnectionState.Connected, ct);
        }

        public async Task MountAsync(string exportPath, CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Connected, ct);

            // Mount operation implementation
            // This would involve the actual NFS MOUNT protocol call
            await TransitionStateAsync(NFSConnectionState.Mounted, ct);
        }

        public async Task UnmountAsync(CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Mounted, ct);

            // Unmount operation implementation
            _handleCache.Clear();
            _dirCache.Clear();
            _attrCache.Clear();
            await TransitionStateAsync(NFSConnectionState.Connected, ct);
        }

        public async Task DisconnectAsync(CancellationToken ct = default)
        {
            if (_connectionState == NFSConnectionState.Mounted)
            {
                await UnmountAsync(ct);
            }

            if (_connectionState == NFSConnectionState.Connected)
            {
                await _transport.DisconnectAsync(ct);
                await TransitionStateAsync(NFSConnectionState.Disconnected, ct);
            }
        }

        public async Task<WriteResult> WriteAsync(
            NFSv3Handle fileHandle,
            long offset,
            ReadOnlyMemory<byte> data,
            CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Mounted, ct);

            var args = new WriteArgs(fileHandle, offset, data.ToArray());
            var result = await CallWithTimeoutAsync<WriteArgs, WriteResult>(
                NFSv3Constants.NFSPROC3_WRITE,
                args,
                isQuickOperation: false,
                ct
            );

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                // Update cached attributes since we modified the file
                _attrCache.Invalidate(fileHandle);
            }

            return result;
        }

        public async Task<CommitResult> CommitAsync(
            NFSv3Handle fileHandle,
            CancellationToken ct = default)
        {
            await EnsureStateAsync(NFSConnectionState.Mounted, ct);

            var args = new CommitArgs(fileHandle);
            var result = await _transport.CallAsync<CommitArgs, CommitResult>(
                NFSv3Constants.NFSPROC3_COMMIT,
                args,
                ct
            );

            if (result == null)
            {
                throw new NFSv3Exception("Null response from RPC call COMMIT");
            }

            if (result.Status.Value == NFSv3Status.NFS3_OK)
            {
                _attrCache.Invalidate(fileHandle);
            }

            return result;
        }

        private async Task<TResult> CallWithTimeoutAsync<TArgs, TResult>(
            int procedure,
            TArgs args,
            bool isQuickOperation = false,
            CancellationToken ct = default) 
            where TArgs : class, IXdrData
            where TResult : NFSv3Result, new()
        {
            using var timeoutCts = new CancellationTokenSource(
                TimeSpan.FromMilliseconds(isQuickOperation ? 
                    NFSv3Constants.QUICK_RPC_TIMEOUT : 
                    NFSv3Constants.DEFAULT_RPC_TIMEOUT));

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            var result = await _transport.CallAsync<TArgs, TResult>(
                procedure,
                args,
                linkedCts.Token
            );

            return result ?? throw new NFSv3Exception($"Null response from RPC call {procedure}");
        }
    }
}
