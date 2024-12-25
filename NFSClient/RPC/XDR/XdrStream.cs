namespace NFSLibrary.RPC.XDR
{
    /// <summary>
    /// Base class for XDR encoding streams.
    /// </summary>
    public abstract class XdrEncodingStream : IDisposable
    {
        public abstract void XdrEncodeInt(int value);
        public abstract void XdrEncodeLong(long value);
        public abstract void XdrEncodeFloat(float value);
        public abstract void XdrEncodeDouble(double value);
        public abstract void XdrEncodeBoolean(bool value);
        public abstract void XdrEncodeString(string value);
        public abstract void XdrEncodeOpaque(byte[] value);
        public abstract void Dispose();
    }

    /// <summary>
    /// Base class for XDR decoding streams.
    /// </summary>
    public abstract class XdrDecodingStream : IDisposable
    {
        public abstract int XdrDecodeInt();
        public abstract long XdrDecodeLong();
        public abstract float XdrDecodeFloat();
        public abstract double XdrDecodeDouble();
        public abstract bool XdrDecodeBoolean();
        public abstract string XdrDecodeString();
        public abstract byte[] XdrDecodeOpaque();
        public abstract void Dispose();
    }
} 