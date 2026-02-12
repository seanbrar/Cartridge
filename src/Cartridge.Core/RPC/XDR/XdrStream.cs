using System.Buffers;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Base class for XDR streams providing common functionality.
    /// </summary>
    public abstract class XdrStream : IAsyncDisposable
    {
        protected Memory<byte> Buffer { get; private set; }
        protected int Position { get; set; }
        private readonly ArrayPool<byte>? _arrayPool;

        protected XdrStream(int initialCapacity = 8192, bool useArrayPool = true)
        {
            _arrayPool = useArrayPool ? ArrayPool<byte>.Shared : null;
            Buffer = _arrayPool != null 
                ? _arrayPool.Rent(initialCapacity) 
                : new byte[initialCapacity];
            Position = 0;
        }

        protected virtual ValueTask EnsureBufferSizeAsync(int required)
        {
            if (Buffer.Length - Position < required)
            {
                var newSize = Math.Max(Buffer.Length * 2, Buffer.Length + required);
                var newBuffer = _arrayPool != null 
                    ? _arrayPool.Rent(newSize) 
                    : new byte[newSize];
                
                Buffer.CopyTo(newBuffer);
                
                if (_arrayPool != null)
                {
                    _arrayPool.Return(Buffer.ToArray());
                }
                
                Buffer = newBuffer;
            }
            return default;
        }

        protected void ValidateBufferBounds(int required)
        {
            if (Position + required > Buffer.Length)
            {
                throw new InvalidOperationException($"Buffer overflow: required {required} bytes but only {Buffer.Length - Position} available");
            }
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_arrayPool != null && !Buffer.IsEmpty)
            {
                _arrayPool.Return(Buffer.ToArray());
                Buffer = Memory<byte>.Empty;
            }
            Position = 0;
            await ValueTask.CompletedTask;
        }

        /// <summary>
        /// Aligns the buffer position to a 4-byte boundary as required by XDR.
        /// </summary>
        protected void AlignBuffer()
        {
            var padding = (4 - (Position & 3)) & 3;
            Position += padding;
        }

        /// <summary>
        /// Gets the number of bytes needed to pad to a 4-byte boundary.
        /// </summary>
        protected static int GetPadding(int length)
        {
            return (4 - (length & 3)) & 3;
        }
    }
}