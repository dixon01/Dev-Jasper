// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapVirtualKeyType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MapVirtualKeyType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Structs
{
    /// <summary>
    /// The type of mapping done with <see cref="Gorba.Common.Utility.Win32.Api.DLLs.User32.MapVirtualKey"/>
    /// </summary>
    public enum MapVirtualKeyType : uint
    {
        /// <summary>
        /// <c>MAPVK_VK_TO_VSC</c>:
        /// The code parameter is a virtual-key code and is translated into a scan code.
        /// If it is a virtual-key code that does not distinguish between left- and right-hand keys,
        /// the left-hand scan code is returned. If there is no translation, the function returns 0.
        /// </summary>
        VirtualKeyToScanCode = 0,

        /// <summary>
        /// <c>MAPVK_VSC_TO_VK</c>:
        /// The uCode parameter is a scan code and is translated into a virtual-key code that
        /// does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.
        /// </summary>
        ScanCodeToVirtualKey = 1,

        /// <summary>
        /// <c>MAPVK_VK_TO_CHAR</c>:
        /// The uCode parameter is a virtual-key code and is translated into an un-shifted character value
        /// in the low order word of the return value. Dead keys (diacritics) are indicated by setting
        /// the top bit of the return value. If there is no translation, the function returns 0.
        /// </summary>
        VirtualKeyToChar = 2,

        /// <summary>
        /// <c>MAPVK_VSC_TO_VK_EX</c>:
        /// The uCode parameter is a scan code and is translated into a virtual-key code that distinguishes
        /// between left- and right-hand keys. If there is no translation, the function returns 0.
        /// </summary>
        ScanCodeToVirtualKeyExtended = 3,
    }
}
