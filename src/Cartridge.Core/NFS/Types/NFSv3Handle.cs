using System;
using System.Text;
using System.Threading.Tasks;
using Cartridge.Core.RPC.XDR;

namespace Cartridge.Core.NFS.Types
{
    /// <summary>
    /// Represents an NFSv3 file handle, which is an opaque identifier for a file or directory on the NFS server.
    /// </summary>
    public sealed class NFSv3Handle : IXdrData, IEquatable<NFSv3Handle>
    {
        private readonly byte[] _handleData;

        /// <summary>
        /// Creates a new NFSv3Handle from raw handle data.
        /// </summary>
        /// <param name="handleData">The raw handle data from the NFS server.</param>
        /// <exception cref="ArgumentNullException">If handleData is null.</exception>
        /// <exception cref="ArgumentException">If handleData length exceeds NFS v3 maximum.</exception>
        public NFSv3Handle(byte[] handleData)
        {
            if (handleData == null)
                throw new ArgumentNullException(nameof(handleData));
            
            if (handleData.Length > 64) // NFSv3 max handle size
                throw new ArgumentException("Handle data exceeds maximum size of 64 bytes", nameof(handleData));

            _handleData = (byte[])handleData.Clone(); // Create defensive copy
        }

        /// <summary>
        /// Creates a new NFSv3Handle from a path string.
        /// </summary>
        /// <param name="path">The file system path.</param>
        /// <exception cref="ArgumentNullException">If path is null.</exception>
        public NFSv3Handle(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            // Convert path to UTF-8 bytes
            _handleData = Encoding.UTF8.GetBytes(path);
            
            if (_handleData.Length > 64)
                throw new ArgumentException("Path exceeds maximum handle size of 64 bytes when encoded", nameof(path));
        }

        /// <summary>
        /// Creates a new NFSv3Handle for XDR decoding.
        /// </summary>
        /// <param name="forDecoding">Indicates whether this constructor is for XDR decoding.</param>
        /// <exception cref="ArgumentException">If forDecoding is false.</exception>
        internal NFSv3Handle(bool forDecoding)
        {
            if (!forDecoding)
                throw new ArgumentException("This constructor is only for XDR decoding");
            _handleData = Array.Empty<byte>();
        }

        public ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            return xdr.EncodeOpaqueAsync(_handleData);
        }

        [Obsolete("Use XdrEncodeAsync instead. Synchronous methods may cause deadlocks.")]
        public void XdrEncode(XdrEncodingStream xdr)
            => throw new NotImplementedException("Synchronous encoding not supported");

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            var decoded = await xdr.DecodeOpaqueAsync();
            
            if (decoded.Length > 64) // NFSv3 max handle size
                throw new XdrException("Handle data exceeds maximum size of 64 bytes");

            var newData = decoded.ToArray();
            Buffer.BlockCopy(newData, 0, _handleData, 0, newData.Length);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as NFSv3Handle);
        }

        public bool Equals(NFSv3Handle? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return _handleData.AsSpan().SequenceEqual(other._handleData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                BitConverter.ToInt32(_handleData.AsSpan()[..Math.Min(4, _handleData.Length)])
            );
        }

        public override string ToString()
        {
            return Convert.ToHexString(_handleData);
        }

        // Helper method to create from hex string (useful for debugging/testing)
        public static NFSv3Handle FromString(string hexString)
        {
            return new NFSv3Handle(Convert.FromHexString(hexString));
        }
    }
}
