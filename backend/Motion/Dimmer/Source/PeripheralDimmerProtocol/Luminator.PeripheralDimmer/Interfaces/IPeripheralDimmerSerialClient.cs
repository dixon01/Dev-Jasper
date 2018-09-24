// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralDimmerSerialClient.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    using Luminator.PeripheralDimmer.Models;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The PeripheralDimmerSerialClient interface.</summary>
    public interface IPeripheralDimmerSerialClient : IPeripheralSerialClient
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether is connected.</summary>
        bool IsConnected { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The start background processing.</summary>
        /// <param name="msInterval">The timer interval on requesting sensor levels.</param>
        void StartBackgroundProcessing(int msInterval);

        /// <summary>The stop background processing.</summary>
        void StopBackgroundProcessing();

        /// <summary>The write brightness.</summary>
        /// <param name="brightnessLevel">The brightness level.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool WriteBrightness(byte brightnessLevel);

        /// <summary>The write brightness levels.</summary>
        /// <param name="brightnessLevels">The brightness levels to send to the peripheral.</param>
        /// <param name="msWriteDelay">The Write Delay in milliseconds between setting the brightness levels on the peripheral.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool WriteBrightnessLevels(DimmerBrightnessLevels brightnessLevels, int msWriteDelay);

        /// <summary>The write power on mode.</summary>
        /// <param name="powerOnMode">The power on mode.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool WritePowerOnMode(PowerOnMode powerOnMode);

        /// <summary>The write dimmer query request.</summary>
        /// <param name="msTimeout">The ms Timeout to wait for the response.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool WriteQueryRequest(int msTimeout = 1000);

        /// <summary>The write sensor scale.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool WriteSensorScale(byte value);

        /// <summary>The write version request.</summary>
        /// <returns>The <see cref="DimmerVersionInfo" />.</returns>
        VersionInfo WriteVersionRequest();

        #endregion
    }
}