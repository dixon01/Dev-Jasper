// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VolumeWrapperBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
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
        /// Gets the current overall system volume.
        /// </summary>
        /// <returns>
        /// The volume between 0 and 100.
        /// </returns>
        public abstract int GetVolume();

        /// <summary>
        /// Sets the current overall system volume.
        /// </summary>
        /// <param name="volume">
        /// The volume between 0 and 100.
        /// </param>
        public abstract void SetVolume(int volume);
    }
}
