// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralGpioEventArg.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Collections.Generic;

    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral gpio event arg.</summary>
    public class PeripheralGpioEventArg : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralGpioEventArg"/> class. The peripheral gpio event arg.</summary>
        /// <param name="peripheralAudioGpioStatus">The status.</param>
        /// <param name="peripheralAudioConfig">The peripheral Audio Config.</param>
        /// <exception cref="ArgumentNullException"><paramref name=""/> is <see langword="null"/>.</exception>
        public PeripheralGpioEventArg(PeripheralAudioGpioStatus peripheralAudioGpioStatus, PeripheralAudioConfig peripheralAudioConfig)
        {
            this.GpioInfo = new List<GpioInfo>();
            if (peripheralAudioGpioStatus == null)
            {
                throw new ArgumentNullException(nameof(peripheralAudioGpioStatus), "Invalid PeripheralAudioGpioStatus");
            }

            if (peripheralAudioConfig == null)
            {
                // TODO Cache the data
                peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig();
                if (peripheralAudioConfig == null)
                {
                    throw new ArgumentNullException(nameof(peripheralAudioConfig), "Invalid PeripheralAudioConfig");
                }
            }

            this.RawPinStatus = peripheralAudioGpioStatus.RawPinStatus;
            this.GpioInfo = CreatePinInfoList(peripheralAudioConfig, peripheralAudioGpioStatus.ChangeMask, peripheralAudioGpioStatus.GpioStatus);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralGpioEventArg"/> class.</summary>
        public PeripheralGpioEventArg()
        {
            this.GpioInfo = new List<GpioInfo>();
            this.RawPinStatus = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the gpio info.</summary>
        public List<GpioInfo> GpioInfo { get; set; }

        /// <summary>Gets or sets the raw pin status.</summary>
        public ushort RawPinStatus { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Create the pin status collection of changed Gpio.</summary>
        /// <param name="peripheralAudioConfig">The peripheral audio config.</param>
        /// <param name="changeMask">The change mask.</param>
        /// <param name="gpioStatus">The gpio status.</param>
        /// <returns>The <see cref="List"/>.</returns>
        public static List<GpioInfo> CreatePinInfoList(PeripheralAudioConfig peripheralAudioConfig, ushort changeMask, ushort gpioStatus)
        {
            var gpioInfoList = new List<GpioInfo>();
            var pinMeaning = peripheralAudioConfig.PinMeaning;
            ushort mask = 0x1;
            for (ushort i = 1; i < 16; i++)
            {
                if ((changeMask & mask) != 0)
                {
                    PeripheralGpioType gpio;
                    var idx = i - 1;
                    if (Enum.TryParse(pinMeaning.GetValue(idx).ToString(), out gpio))
                    {
                        var pinActive = (gpioStatus & mask) != 0;
                        gpioInfoList.Add(new GpioInfo(gpio, pinActive, changeMask, gpioStatus));
                    }
                }

                mask = (ushort)(mask << 1);
            }

            return gpioInfoList;
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            if (this.GpioInfo == null)
            {
                return "GpioInfo Undefined";
            }

            var s = string.Empty;
            foreach (var gpioInfo in this.GpioInfo)
            {
                s += gpioInfo.ToString() + ",";
            }

            return s;
        }

        #endregion
    }
}