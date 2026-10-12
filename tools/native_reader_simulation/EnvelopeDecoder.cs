using System;

namespace EagleAuto.NativeReaderSimulation
{
    // Test-only proposed envelope. Never wired into the legacy WM_COPYDATA parser.
    public static class EnvelopeDecoder
    {
        public const int HeaderSize = 24, MaxPayload = 65536;
        private static uint Read32(byte[] bytes, int offset)
        { return (uint)(bytes[offset] | bytes[offset + 1] << 8 | bytes[offset + 2] << 16 | bytes[offset + 3] << 24); }

        public static bool TryDecode(byte[] envelope, int expectedPid, out uint sequence, out byte[] payload)
        {
            sequence = 0; payload = null;
            if (envelope == null || envelope.Length < HeaderSize || expectedPid <= 0) return false;
            if (envelope[0] != 'E' || envelope[1] != 'G' || envelope[2] != 'R' || envelope[3] != 'D'
                || envelope[4] != 1 || envelope[5] != 0 || envelope[6] != HeaderSize || envelope[7] != 0) return false;
            uint length = Read32(envelope, 16);
            if (Read32(envelope, 8) != (uint)expectedPid || Read32(envelope, 20) != 0 || length == 0
                || length > MaxPayload || envelope.Length != HeaderSize + (long)length) return false;
            sequence = Read32(envelope, 12);
            if (sequence == 0) return false;
            payload = new byte[(int)length];
            Buffer.BlockCopy(envelope, HeaderSize, payload, 0, (int)length);
            return true;
        }
    }
}
