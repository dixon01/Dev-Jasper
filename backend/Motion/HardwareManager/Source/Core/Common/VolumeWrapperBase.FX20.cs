// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VolumeWrapperBase.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VolumeWrapperBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;

    /// <summary>
    /// The volume wrapper.
    /// </summary>
    internal abstract partial class VolumeWrapperBase
    {
        /// <summary>
        /// Creates a new <see cref="VolumeWrapperBase"/> implementation
        /// valid for the current system.
        /// </summary>
        /// <returns>
        /// The <see cref="VolumeWrapperBase"/>.
        /// </returns>
        public static VolumeWrapperBase Create()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return new WinMmVolumeWrapper();
            }

            return new NAudioVolumeWrapper();
        }
    }
}
