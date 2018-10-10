// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IAudioSwitchSerialClient.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System;
    using System.IO;
    using System.IO.Ports;

    using Gorba.Common.Medi.Core.Messages;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The AudioSwitchSerialClient interface.</summary>
    public interface IAudioSwitchSerialClient : IPeripheralSerialClient<PeripheralAudioSwitchMessageType>
    {
        #region Public Events

        /// <summary>Peripheral GPIO changed event</summary>
        event EventHandler<PeripheralGpioEventArg> GpioChanged;

        /// <summary>The audio status changed.</summary>
        event EventHandler<AudioStatusMessage> AudioStatusChanged;

        event EventHandler<PeripheralAudioSwitchDataReceivedEventArgs> PeripheralAudioSwitchDataReceived;

        #endregion

        #region Public Methods and Operators

        /// <summary>Read peripheral audio config file from xml.</summary>
        /// <param name="audioConfigFile">The audio config file.</param>
        /// <returns>The <see cref="PeripheralAudioConfig"/>.</returns>
        PeripheralAudioConfig ReadPeripheralAudioConfigFile(string audioConfigFile);

        /// <summary>The write audio config.</summary>
        /// <param name="audioConfig">The audio config.</param>
        /// <returns>The <see cref="int"/>.</returns>
        int WriteAudioConfig(PeripheralAudioConfig audioConfig);

        /// <summary>Write the audio config xml settings to the serial port stream.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="audioStatusDelay">The optional audio Status Delay that cause the hardware to send(TX) Audio Status to the
        ///     client at this interval. Use 0 - 30000</param>
        /// <returns>The <see cref="int"/>Number of bytes written.</returns>
        int WriteAudioConfig(string fileName = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile, ushort audioStatusDelay = 0);

        /// <summary>Write audio enabled message.</summary>
        /// <param name="activeSpeakerZone">The active speaker zone. see enum ActiveSpeakerZone</param>
        /// <returns>The <see cref="int"/>Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        int WriteAudioEnabled(ActiveSpeakerZone activeSpeakerZone);

        /// <summary>The write audio status interval.</summary>
        /// <param name="audioStatusDelay">The audio status delay.</param>
        /// <returns>The <see cref="int"/>.</returns>
        int WriteAudioStatusInterval(ushort audioStatusDelay);

        /// <summary>Write a request to receive audio status from the peripheral</summary>
        /// <returns>The <see cref="int" />.</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        int WriteAudioStausRequest();

        /// <summary>Write message to set the volume.</summary>
        /// <param name="interiorVolume">The interior Volume.</param>
        /// <param name="exteriorVolume">The exterior Volume.</param>
        /// <returns>The <see cref="int"/>Total Bytes written</returns>
        int WriteSetVolume(byte interiorVolume, byte exteriorVolume);

        bool UpdateImage(string fileName);

        #endregion
    }
}