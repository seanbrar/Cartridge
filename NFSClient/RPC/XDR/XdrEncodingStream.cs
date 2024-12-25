using System;
using System.Net;

namespace NFSClient.RPC.XDR
{
    public abstract class XdrEncodingStream : IDisposable
    {
        public abstract void xdrEncodeBoolean(bool value);
        public abstract void xdrEncodeByte(byte value);
        public abstract void xdrEncodeShort(short value);
        public abstract void xdrEncodeInt(int value);
        public abstract void xdrEncodeLong(long value);
        public abstract void xdrEncodeFloat(float value);
        public abstract void xdrEncodeDouble(double value);
        public abstract void xdrEncodeString(string value);
        public abstract void xdrEncodeOpaque(byte[] value);
        public abstract void xdrEncodeIPAddress(IPAddress value);
        
        protected void xdrEncodeFixedOpaque(byte[] value, int length)
        {
            if (value == null || value.Length != length)
                throw new ArgumentException($"Fixed opaque data must be exactly {length} bytes");
            xdrEncodeOpaque(value);
        }
        
        protected void xdrEncodeVariableOpaque(byte[] value, int maxLength)
        {
            if (value == null || value.Length > maxLength)
                throw new ArgumentException($"Variable opaque data must not exceed {maxLength} bytes");
            xdrEncodeOpaque(value);
        }

        public abstract void Dispose();
    }
} 