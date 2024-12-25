using System.IO.Pipes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
using NFSLibrary.Protocols.Commons;

namespace NFSLibrary.RPC
{
    /// <summary>
    /// Modern RPC client implementation for NFS operations.
    /// This replaces the legacy Java-ported implementation with a clean, C#-native approach.
    /// </summary>
    public class NFSRpcClient : INFSRpcClient
    {
        private readonly JsonRpc _rpc;
        private readonly NamedPipeClientStream _pipe;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly ILogger<NFSRpcClient> _logger;
        private bool _isConnected;

        public NFSRpcClient(string serverName, int port, ILogger<NFSRpcClient> logger)
        {
            _pipe = new NamedPipeClientStream(".", $"nfs-{serverName}-{port}", PipeDirection.InOut, PipeOptions.Asynchronous);
            _cancellationSource = new CancellationTokenSource();
            _rpc = JsonRpc.Attach(_pipe);
            _logger = logger;
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _pipe.ConnectAsync(_cancellationSource.Token);
                _isConnected = true;
                _logger.LogInformation("Connected to NFS server");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to NFS server");
                throw;
            }
        }

        public async Task<NFSAttributes> GetAttributesAsync(string path)
        {
            try
            {
                var result = await _rpc.InvokeAsync<NFSAttributesDTO>("GetAttributes", path);
                return new NFSAttributes(
                    result.CreateTime,
                    result.AccessTime,
                    result.ModifyTime,
                    result.Type,
                    result.Mode,
                    result.Size,
                    result.Handle
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attributes for {Path}", path);
                throw;
            }
        }

        public async Task<byte[]> ReadAsync(string path, long offset, int length)
        {
            try
            {
                return await _rpc.InvokeAsync<byte[]>("Read", new { path, offset, length });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read from {Path} at offset {Offset}", path, offset);
                throw;
            }
        }

        public async Task WriteAsync(string path, long offset, byte[] data)
        {
            try
            {
                await _rpc.InvokeAsync("Write", new { path, offset, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to {Path} at offset {Offset}", path, offset);
                throw;
            }
        }

        public async Task<string[]> ReadDirectoryAsync(string path)
        {
            try
            {
                return await _rpc.InvokeAsync<string[]>("ReadDirectory", path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read directory {Path}", path);
                throw;
            }
        }

        public async Task CreateDirectoryAsync(string path, NFSPermission mode)
        {
            try
            {
                await _rpc.InvokeAsync("CreateDirectory", new { path, mode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create directory {Path}", path);
                throw;
            }
        }

        public async Task RemoveAsync(string path)
        {
            try
            {
                await _rpc.InvokeAsync("Remove", path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove {Path}", path);
                throw;
            }
        }

        public async Task RenameAsync(string oldPath, string newPath)
        {
            try
            {
                await _rpc.InvokeAsync("Rename", new { oldPath, newPath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rename {OldPath} to {NewPath}", oldPath, newPath);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _cancellationSource.Cancel();
                _rpc.Dispose();
                _pipe.Dispose();
                _cancellationSource.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RPC client disposal");
            }
        }

        private class NFSAttributesDTO
        {
            public int CreateTime { get; set; }
            public int AccessTime { get; set; }
            public int ModifyTime { get; set; }
            public NFSItemTypes Type { get; set; }
            public NFSPermission Mode { get; set; }
            public long Size { get; set; }
            public byte[] Handle { get; set; } = Array.Empty<byte>();
        }
    }
} 