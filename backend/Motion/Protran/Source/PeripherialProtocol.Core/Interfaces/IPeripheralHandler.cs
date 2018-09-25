// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralHandler.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System;
    using System.IO;

    using Luminator.PeripheralProtocol.Core.Models;

    /// <summary>The PeripheralHandler interface.</summary>
    public interface IPeripheralHandler : IDisposable
    {
        #region Public Events

        #endregion

        #region Public Properties

        /// <summary>Gets the audio switch context stream.</summary>
        Stream ContextStream { get; }

        /// <summary>Gets or sets a flag indicating whether the header is Read or Written in network byte order.</summary>
        bool IsHeaderNetworkByteOrder { get; set; }

        /// <summary>Gets or sets a flag if medi is initialized.</summary>
        bool IsMediInitialized { get; set; }

        int PeripheralFramingBytesCount { get; }

        /// <summary>Gets or sets the audio switch context.</summary>
        PeripheralContext PeripheralContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        void Dispose(bool disposing);

        /// <summary>The process received messages.</summary>
        /// <param name="state">The state.</param>
        /// <param name="message">The message.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        IPeripheralBaseMessage ProcessReceivedMessages(PeripheralState state, object message);

        /// <summary>The read.</summary>
        /// <param name="readTimeout">The read timeout.</param>
        /// <returns>The <see cref="object"/>.</returns>
        object Read(int readTimeout = 0);

        /// <summary>Read from the Stream a peripheral message object</summary>
        /// <param name="stream">The source stream.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        /// <exception cref="NotSupportedException">Stream does not support Length property(SerialPort.BaseStream) ie SerialPort
        ///     Stream</exception>
        /// <exception cref="ApplicationException">Failed to find the FromBytes method in assembly!!!</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        object Read(Stream stream);

        /// <summary>Read from the stream as a series of bytes.</summary>
        /// <param name="stream">The stream or null to use the default context stream</param>
        /// <param name="readTimeout">Stream read timeout or Zero to use the default on the stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>The entity of type T.</returns>
        /// <exception cref="ArgumentNullException"><paramref name=""/> is <see langword="null"/>.</exception>
        T Read<T>(Stream stream = null, int readTimeout = 0) where T : class, IPeripheralBaseMessage;

        /// <summary>The remove next message.</summary>
        /// <param name="state">The state.</param>
        /// <returns>The <see cref="object"/>.</returns>
        object RemoveNextMessage(PeripheralState state);

        /// <summary>Write the framing octet(first byte in stream) and the model as series of bytes to the peripheral stream,
        ///     calculating the checksum.</summary>
        /// <param name="model">The peripheral model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>.</returns>
        int Write<T>(T model) where T : class, IPeripheralBaseMessage;

        /// <summary>The write.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="stream">The stream.</param>
        int Write(byte[] buffer, Stream stream = null);

        /// <summary>Write the framing octet, model as series of bytes to the peripheral stream, calculating the checksum.</summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream or null to use the existing AudioSwitchContext.Stream</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>Number of bytes written to stream</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        int Write<T>(T model, Stream stream) where T : class, IPeripheralBaseMessage;

        /// <summary>The write ack message to context stream.</summary>
        /// <returns>The <see cref="int" />.</returns>
        int WriteAck(ushort address);

        /// <summary>Write the framing  byte to the stream. This is the first byte that starts outgoing messages in the protocol</summary>
        /// <param name="framing">The framing. Default see Constants.FramingByte</param>
        /// <returns>Number of bytes written to stream.</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        int WriteFraming(byte framing = Constants.PeripheralFramingByte);

        /// <summary>The write nak message to context stream.</summary>
        /// <returns>The <see cref="int" />.</returns>
        int WriteNak(ushort address);

        /// <summary>The write response.</summary>
        /// <param name="validChecksum">The valid checksum.</param>
        void WriteResponse(bool validChecksum);

        #endregion
    }
}