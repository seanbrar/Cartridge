using System;
using System.Text;

namespace NFSLibrary.Protocols.Commons
{
    public class NFSAttributes
    {
        public DateTime CreateDateTime { get; }
        public DateTime LastAccessedDateTime { get; }
        public DateTime ModifiedDateTime { get; }
        public NFSItemTypes NFSType { get; }
        public NFSPermission Mode { get; }
        public long Size { get; }
        public byte[] Handle { get; }

        public NFSAttributes(int cdateTime, int adateTime, int mdateTime, NFSItemTypes type, NFSPermission mode, long size, byte[] handle)
        {
            CreateDateTime = DateTimeOffset.FromUnixTimeSeconds(cdateTime).DateTime;
            LastAccessedDateTime = DateTimeOffset.FromUnixTimeSeconds(adateTime).DateTime;
            ModifiedDateTime = DateTimeOffset.FromUnixTimeSeconds(mdateTime).DateTime;
            NFSType = type;
            Size = size;
            Mode = mode;
            Handle = (byte[])handle.Clone();
        }

        public override string ToString()
        {
            var handleString = new StringBuilder();
            foreach (byte b in Handle)
            {
                handleString.Append(b.ToString("X2"));
            }

            return $"CDateTime: {CreateDateTime}, ADateTime: {LastAccessedDateTime}, MDateTime: {ModifiedDateTime}, " +
                   $"Type: {NFSType}, Mode: {Mode.UserAccess}{Mode.GroupAccess}{Mode.OtherAccess}, " +
                   $"Size: {Size}, Handle: {handleString}";
        }
    }
}