namespace NFSLibrary.Protocols.Commons
{
    /// <summary>
    /// Represents the different types of items in an NFS filesystem.
    /// </summary>
    public enum NFSItemTypes
    {
        /// <summary>
        /// Regular file
        /// </summary>
        Regular = 1,

        /// <summary>
        /// Directory
        /// </summary>
        Directory = 2,

        /// <summary>
        /// Block special device
        /// </summary>
        Block = 3,

        /// <summary>
        /// Character special device
        /// </summary>
        Character = 4,

        /// <summary>
        /// Symbolic link
        /// </summary>
        SymbolicLink = 5,

        /// <summary>
        /// Socket
        /// </summary>
        Socket = 6,

        /// <summary>
        /// Named pipe (FIFO)
        /// </summary>
        NamedPipe = 7
    }
} 