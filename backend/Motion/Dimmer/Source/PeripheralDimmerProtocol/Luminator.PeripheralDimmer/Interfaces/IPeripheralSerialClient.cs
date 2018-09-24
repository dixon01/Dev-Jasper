// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralSerialClient.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    using System;
    using System.IO;
    using System.IO.Ports;

    /// <summary>The PeripheralSerialClient interface.</summary>
    public interface IPeripheralSerialClient : IDisposable
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether is open.</summary>
        bool IsOpen { get; }

        /// <summary>Gets or sets the serial port.</summary>
        SerialPort SerialPort { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close.</summary>
        void Close();

        /// <summary>The open.</summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        /// <returns>The <see cref="Stream"/>.</returns>
        Stream Open(ISerialPortSettings serialPortSettings);

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="userNetworkToHostByteOrder">The user network to host byte order.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="object"/>.</returns>
        T Read<T>(byte[] bytes, bool userNetworkToHostByteOrder) where T : class, IPeripheralBaseMessage;

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="networkByteOrderToHost">The network byte order to host.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        IPeripheralBaseMessage Read(byte[] bytes, bool networkByteOrderToHost = true);

        /// <summary>The read.</summary>
        /// <param name="msTimeout">The ms timeout.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        byte[] Read(int msTimeout);

        /// <summary>The serial port on data received.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="serialDataReceivedEventArgs">The serial data received event args.</param>
        void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs);

        /// <summary>The serial port on error received.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="serialErrorReceivedEventArgs">The serial error received event args.</param>
        void SerialPortOnErrorReceived(object sender, SerialErrorReceivedEventArgs serialErrorReceivedEventArgs);

        /// <summary>The write.</summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>.</returns>
        int Write<T>(T model, bool waitForAck, Stream stream) where T : class, IPeripheralBaseMessage;

        #endregion
    }
}