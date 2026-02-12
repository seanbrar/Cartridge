namespace Cartridge.Core.NFS.Types
{
    public class NFSOperationProgress
    {
        public long BytesTransferred { get; }
        public long? TotalBytes { get; }
        public NFSOperationType OperationType { get; }

        public NFSOperationProgress(long bytesTransferred, long? totalBytes, NFSOperationType operationType)
        {
            BytesTransferred = bytesTransferred;
            TotalBytes = totalBytes;
            OperationType = operationType;
        }
    }

    public enum NFSOperationType
    {
        Read,
        Write,
        DirectoryListing
    }
}