// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crc.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CRC type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    /// <summary>
    /// Manages calculation of CRC
    /// </summary>
    public class Crc
    {
        /// <summary>
        /// Calculates the CRC to the incoming buffer.
        /// </summary>
        /// <param name="buffer">The buffer to which calculate the CRC.</param>
        /// <param name="bufLen">The buffer's length.</param>
        /// <returns>CRC code in unicode</returns>
        public ushort CalculateCrc(byte[] buffer, int bufLen)
        {
            ushort curCrc = 0xffff;
            for (uint i = 0; i < bufLen; i++)
            {
                ushort ch = buffer[i];
                this.CurrentCrc(ref curCrc, ch);
            }

            this.CurrentCrc(ref curCrc, 0);
            this.CurrentCrc(ref curCrc, 0);

            return curCrc;
        }

        /// <summary>
        /// Calculate the CRC.
        /// </summary>
        /// <param name="crc">CRC value</param>
        /// <param name="ch">offset value</param>
        public void CurrentCrc(ref ushort crc, ushort ch)
        {
            // #define poly 0x1021          /* crc-ccitt mask */
            // Align test bit with leftmost bit of the message byte.
            byte bitPos = 0x0080;
            for (int i = 0; i < 8; i++)
            {
                bool xorFlag = (crc & 0x8000) != 0;
                crc = (ushort)(crc << 1);

                if ((ch & bitPos) != 0)
                {
                    crc = (ushort)(crc + 1);
                }

                if (xorFlag)
                {
                    crc = (ushort)(crc ^ 0x1021);
                }

                bitPos = (byte)(bitPos >> 1);
            }
        }
    }
}
