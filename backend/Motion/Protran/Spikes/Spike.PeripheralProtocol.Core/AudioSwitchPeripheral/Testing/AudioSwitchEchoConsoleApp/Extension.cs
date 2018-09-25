

namespace AudioSwitchEchoConsoleApp
{
    using System;
    using System.Linq;

    internal static class Extension
    {

        public static int FindFramingPosition(this byte[] bytes, byte framingByte = Constants.PeripheralFramingByte)
        {
            if (bytes != null && bytes.Length > 0)
            {
                return Array.IndexOf(bytes, framingByte);
            }

            return -1;
        }

        public static byte[] GetStartOfMessage(this byte[] buffer, byte framingByte = Constants.PeripheralFramingByte)
        {
            int framingPosition = buffer.FindFramingPosition();
            if (framingPosition >= 0)
            {
                // Framing byte found in the stream, position or skip to the byte after this one
                buffer = buffer.Skip(framingPosition + 1).ToArray();
            }

            return buffer;
        }
    }
}
