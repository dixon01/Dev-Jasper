// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralSerialClient.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Threading;

    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralSerialClient interface.</summary>
    public interface IPeripheralSerialClient : IDisposable
    {
        #region Public Events

        /// <summary>Peripheral GPIO changed event</summary>
        /// <summary>The peripheral data received.</summary>
        event EventHandler<PeripheralDataReceivedEventArgs> PeripheralDataReceived;

        /// <summary>The serial data received.</summary>
        event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;

        /// <summary>The serial error received.</summary>
        event EventHandler<SerialErrorReceivedEventArgs> SerialErrorReceived;

        /// <summary>The version info changed.</summary>
        event EventHandler<PeripheralVersionsInfo> VersionInfoChanged;

        /// <summary>The peripheral image update info changed.</summary>
        event EventHandler<PeripheralImageUpdateInfo> PeripheralImageUpdateInfoChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether is valid com port and opened.
        ///     <see>IsAudioSwitchConnected</see>
        /// </summary>
        bool IsComPortOpen { get; }

        /// <summary>Gets a value indicating whether is peripheral has connected and exchanged it's verison information.</summary>
        bool IsPeripheralConnected { get; }

        /// <summary>Gets or sets a value indicating whether the peripheral has been configured.</summary>
        bool IsVersionInfoReceived { get; set; }

        /// <summary>Gets or sets the peripheral handler.</summary>
        IPeripheralHandler PeripheralHandler { get; set; }

        /// <summary>Gets or sets the peripheral version info.</summary>
        PeripheralVersionsInfo PeripheralVersionInfo { get; set; }

        /// <summary>Gets the port name.</summary>
        string PortName { get; }

        /// <summary>Gets the serial port.</summary>
        SerialPort SerialPort { get; set; }

        /// <summary>Gets or sets the state.</summary>
        PeripheralState State { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Close the Serial Port
        /// </summary>
        void Close();

        /// <summary>The create peripheral handler.</summary>
        /// <param name="config">The config.</param>
        /// <returns>The <see cref="THandler"/>.</returns>
        /// <exception cref="MissingMethodException">NoteIn the .NET for Windows Store applications or the Portable Class Library,
        ///     catch the base class exception, <see cref="T:System.MissingMemberException"/>, instead.The type that is specified
        ///     for <paramref name="T"/> does not have a parameterless constructor.</exception>
        /// <exception cref="ApplicationException">Reader thread already started</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <exception cref="ThreadStateException">Thead failed to start.</exception>
        IPeripheralHandler CreatePeripheralHandler(PeripheralConfig config);

        /// <summary>The initialize peripheral.</summary>
        /// <param name="stateObject">The state object.</param>
        void InitializePeripheral(object stateObject);

        /// <summary>Opens the Serial Port creating a handler.</summary>
        /// <param name="peripheralConfig">The Peripheral Configuration</param>
        /// <exception cref="ApplicationException">Audio Switch Already created and Opened</exception>
        /// <returns>The <see cref="bool"/>.</returns>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <exception cref="ThreadStateException">Thead failed to start.</exception>
        bool Open(PeripheralConfig peripheralConfig);

        /// <summary>Remove next message from the stream.</summary>
        /// <param name="state">The state.</param>
        /// <returns>The <see cref="object"/>Message object or null if nothing.</returns>
        object RemoveNextMessage(PeripheralState state);

        /// <summary>Write the peripheral model.</summary>
        /// <param name="model">The model.</param>
        /// <param name="ackReceived">The ack Received out parameter.</param>
        /// <param name="waitForAck">The wait For Ack.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        int Write<T>(T model, out bool ackReceived, bool waitForAck) where T : class, IPeripheralBaseMessage;

        /// <summary>Write ack message.</summary>
        /// <returns>The <see cref="int" />Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        int WriteAck();

        /// <summary>Write nak message.</summary>
        /// <returns>The <see cref="int" />Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        int WriteNak();

        /// <summary>Write poll message.</summary>
        /// <returns>The <see cref="int"/>.</returns>
        int WritePoll();

        /// <summary>Write the raw bytes out to the Stream with the header written in network byte order and the lead framing byte
        ///     written as the first byte to the stream.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader! Bytes does not contain a valid header</exception>
        /// <returns>The <see cref="int"/>Total Bytes written</returns>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader!</exception>
        /// <exception cref="OverflowException">The array is multidimensional and contains more than<see cref="F:System.Int32.MaxValue"/> elements.</exception>
        int WriteRaw(byte[] bytes);

        /// <summary>Write request for version message.</summary>
        /// <param name="msTimeout">The Timeout in milliseconds to wait for the reply.</param>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="bool"/>True if Acknowledged successfully else false</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        bool WriteVersionRequest(int msTimeout, ushort address);

        /// <summary>Request an image update.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="imageUpdateType">The image update type.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool UpdateImage(string fileName, PeripheralImageUpdateType imageUpdateType);

        /// <summary>The get update image total records.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="imageUpdateType">The image update type.</param>
        /// <returns>The <see cref="int"/>.</returns>
        int GetUpdateImageTotalRecords(string fileName, PeripheralImageUpdateType imageUpdateType);

        #endregion
    }
}