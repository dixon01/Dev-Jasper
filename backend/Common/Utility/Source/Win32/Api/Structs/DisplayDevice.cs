// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayDevice.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Structs
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The display device structure which provides information about the display device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayDevice
    {
        /// <summary>
        /// The size of the DISPLAYDEVICE structure.
        /// </summary>
        public int Cb;

        /// <summary>
        /// The device name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        /// <summary>
        /// The device string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;

        /// <summary>
        /// The state flags indicating the state of the display device.
        /// </summary>
        public int StateFlags;

        /// <summary>
        /// The display device id.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;

        /// <summary>
        /// The display device key.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayDevice"/> struct.
        /// </summary>
        /// <param name="flags">
        /// The state of the display device.
        /// </param>
        public DisplayDevice(int flags)
        {
            this.Cb = 0;
            this.StateFlags = flags;
            this.DeviceName = new string((char)32, 32);
            this.DeviceString = new string((char)32, 128);
            this.DeviceID = new string((char)32, 128);
            this.DeviceKey = new string((char)32, 128);
            this.Cb = Marshal.SizeOf(this);
        }
    }
}
