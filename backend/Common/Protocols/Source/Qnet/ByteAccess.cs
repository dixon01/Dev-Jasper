using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorba.Common.Protocols.Qnet
{
    public class ByteAccess
    {
        public static UInt32 MakeLong(UInt16 high, UInt16 low)
        {
            return ((UInt32)low & 0xFFFF) | (((UInt32)high & 0xFFFF) << 16);
        }
        public static UInt16 MakeWord(byte high, byte low)
        {
            return (UInt16)(((UInt32)low & 0xFF) | ((UInt32)high & 0xFF) << 8);
        }
        public static UInt16 LoWord(UInt32 nValue)
        {
            return (UInt16)(nValue & 0xFFFF);
        }
        public static UInt16 HiWord(UInt32 nValue)
        {
            return (UInt16)(nValue >> 16);
        }
        public static Byte LoByte(UInt16 nValue)
        {
            return (Byte)(nValue & 0xFF);
        }
        public static Byte HiByte(UInt16 nValue)
        {
            return (Byte)(nValue >> 8);
        }
    }
}
