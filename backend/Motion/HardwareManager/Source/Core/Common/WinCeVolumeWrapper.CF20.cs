// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WinCEVolumeWrapper.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WinMmVolumeWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    /// <summary>
    /// <see cref="VolumeWrapperBase"/> implementation that uses <code>winmm.dll</code>.
    /// </summary>
    internal partial class WinCeVolumeWrapper : VolumeWrapperBase
    {
        /// <summary>
        /// Gets the current overall system volume.
        /// </summary>
        /// <returns>
        /// The volume between 0 and 100.
        /// </returns>
        public override int GetVolume()
        {
            // TODO: implement, if possible
            return 0;
        }

        /// <summary>
        /// Sets the current overall system volume.
        /// </summary>
        /// <param name="volume">
        /// The volume between 0 and 100.
        /// </param>
        public override void SetVolume(int volume)
        {
            // TODO: implement, if possible
        }
    }
}