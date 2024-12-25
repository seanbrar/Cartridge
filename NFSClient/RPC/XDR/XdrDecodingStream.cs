using System;
using System.Net;

namespace NFSClient.RPC.XDR
{
    public abstract class XdrDecodingStream : IDisposable
    {
        public abstract bool xdrDecodeBoolean();
        public abstract byte xdrDecodeByte();
        public abstract short xdrDecodeShort();
        public abstract int xdrDecodeInt();
        public abstract long xdrDecodeLong();
        public abstract float xdrDecodeFloat();
        public abstract double xdrDecodeDouble();
        public abstract string xdrDecodeString();
        public abstract byte[] xdrDecodeOpaque();
        public abstract IPAddress xdrDecodeIPAddress();
        
        protected byte[] xdrDecodeFixedOpaque(int length)
        {
            byte[] value = xdrDecodeOpaque();
            if (value == null || value.Length != length)
                throw new ArgumentException($"Fixed opaque data must be exactly {length} bytes");
            return value;
        }
        
        protected byte[] xdrDecodeVariableOpaque(int maxLength)
        {
            byte[] value = xdrDecodeOpaque();
            if (value == null || value.Length > maxLength)
                throw new ArgumentException($"Variable opaque data must not exceed {maxLength} bytes");
            return value;
        }

        public abstract void Dispose();
    }
} 