// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightSensorTestor
{
    public static class Extensions
    {
        public static ushort HostToNetworkOrder(this ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static ushort NetworkToHostByteOrder(this ushort value)
        {
            return (ushort)((value & 0xFF00U) >> 8 | (value & 0xFFU) << 8);
        }
    }
}
