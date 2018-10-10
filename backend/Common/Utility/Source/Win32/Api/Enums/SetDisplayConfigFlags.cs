// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetDisplayConfigFlags.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// A bitwise OR of flag values that indicates the behavior of the SetDisplayConfig. This parameter can be one the
    /// following values, or a combination of the following values; 0 is not valid.
    /// </summary>
    public enum SetDisplayConfigFlags
    {
        /// <summary>
        /// The caller requests the last internal configuration from the persistence database.
        /// </summary>
        SdcTopologyInternal = 0x00000001,

        /// <summary>
        /// The caller requests the last clone configuration from the persistence database.
        /// </summary>
        SdcTopologyClone = 0x00000002,

        /// <summary>
        /// The caller requests the last extended configuration from the persistence database.
        /// </summary>
        SdcTopologyExtend = 0x00000004,

        /// <summary>
        /// The caller requests the last external configuration from the persistence database.
        /// </summary>
        SdcTopologyExternal = 0x00000008,

        /// <summary>
        /// The resulting topology, source, and target mode is set.
        /// </summary>
        SdcApply = 0x00000080,
    }
}
