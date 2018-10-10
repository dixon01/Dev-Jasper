// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralAudioGpioUpdate.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    /// <summary>The PeripheralAudioGpioUpdate interface. PCP_07_GPIO_UPDATE_PKT</summary>
    public interface IPeripheralAudioGpioStatus : IPeripheralBaseMessage
    {
        #region Public Properties

        /// <summary>Gets or sets the change mask, bits mask of changes in gpioStatus field.</summary>
        ushort ChangeMask { get; set; }

        /// <summary>Gets or sets the gpio status. See PeripheralGpioType</summary>
        ushort GpioStatus { get; set; }

        /// <summary>Gets or sets the raw pin status.  Bitmask of raw pins - no bit order translation</summary>
        ushort RawPinStatus { get; set; }

        #endregion
    }
}