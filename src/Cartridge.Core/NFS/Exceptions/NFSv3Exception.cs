using System;

namespace Cartridge.Core.NFS.Exceptions
{
    public class NFSv3Exception : Exception
    {
        public NFSv3Exception(string message) : base(message)
        {
        }

        public NFSv3Exception(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
