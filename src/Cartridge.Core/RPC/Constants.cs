namespace Cartridge.Core.RPC
{
    public static class RpcConstants
    {
        public const int MAX_PACKET_SIZE = 1048576;    // 1MB max packet
        public const int HEADER_RESERVATION = 512;      // Space for headers
        public const int MAX_PAYLOAD_SIZE = MAX_PACKET_SIZE - HEADER_RESERVATION;
        public const int DEFAULT_BUFFER_SIZE = 8192;   // Current default
    }
}