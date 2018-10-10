// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NAudioVolumeWrapper.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NAudioVolumeWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;

    using NAudio.CoreAudioApi;

    using NLog;

    /// <summary>
    /// <see cref="VolumeWrapperBase"/> implementation that uses <see cref="NAudio"/>.
    /// </summary>
    internal partial class NAudioVolumeWrapper : VolumeWrapperBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the current overall system volume.
        /// </summary>
        /// <returns>
        /// The volume between 0 and 100.
        /// </returns>
        public override int GetVolume()
        {
            // Instantiate an Enumerator to find audio devices
            var enumerator = new MMDeviceEnumerator();

            // Get all the devices, no matter what condition or status
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            double mappedVolume = 0;

            // Loop through all devices
            foreach (var device in devices)
            {
                if (device.State == DeviceState.Unplugged)
                {
                    continue;
                }

                var friendlyName = string.Empty;
                try
                {
                    friendlyName = device.DeviceFriendlyName;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't read master volume of " + friendlyName);
                }

                var value = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                mappedVolume = Math.Max(mappedVolume, value);
            }

            return (int)mappedVolume;
        }

        /// <summary>
        /// Sets the current overall system volume.
        /// </summary>
        /// <param name="volume">
        /// The volume between 0 and 100.
        /// </param>
        public override void SetVolume(int volume)
        {
            // Instantiate an Enumerator to find audio devices
            var enumerator = new MMDeviceEnumerator();

            // Get all the devices, no matter what condition or status
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            // Loop through all devices
            foreach (var device in devices)
            {
                if (device.State == DeviceState.Unplugged)
                {
                    continue;
                }

                string friendlyName = string.Empty;
                try
                {
                    friendlyName = device.DeviceFriendlyName;
                    var mappedVolume = (float)volume / 100;
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = mappedVolume;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't set master volume of " + friendlyName + " to " + volume);
                }
            }
        }
    }
}