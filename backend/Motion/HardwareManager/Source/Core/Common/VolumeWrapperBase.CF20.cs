// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VolumeWrapperBase.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VolumeWrapperBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
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
            return new WinCeVolumeWrapper();
        }
    }
}
