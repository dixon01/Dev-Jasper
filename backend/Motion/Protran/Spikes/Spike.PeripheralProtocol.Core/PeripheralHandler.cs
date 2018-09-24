// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHandler.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Config;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    /// <summary>The base Peripheral Handler.</summary>
    /// <typeparam name="TMessageType"></typeparam>
    public abstract class PeripheralHandler<TMessageType> : IPeripheralHandler<TMessageType>
    {
        #region Static Fields

        /// <summary>The logger.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>The disposed.</summary>
        protected bool disposed;

        /// <summary>The file configuration.</summary>
        protected FileConfigurator fileConfigurator;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralHandler{TMessageType}"/> class. 
        ///     Initializes a new instance of the <see cref="PeripheralHandler"/> class.
        ///     Gets or sets Host.
        ///     The "host" object that will receive (eventually) data from this protocol
        ///     and the will send (eventually) data to this protocol.</summary>
        /// <summary>Initializes a new instance of the <see cref="PeripheralHandler"/> class.</summary>
        protected PeripheralHandler()
        {
            this.IsMediInitialized = false;
        }

        ///// <summary>Initializes a new instance of the <see cref="PeripherialHandler"/> class.</summary>
        ///// <param name="context">The context.</param>
        /// <summary>Initializes a new instance of the <see cref="PeripheralHandler{TMessageType}"/> class. Initializes a new instance of the <see cref="PeripheralHandler"/> class.</summary>
        /// <param name="peripheralContext">The peripheral context.</param>
        protected PeripheralHandler(PeripheralContext<TMessageType> peripheralContext)
            : this()
        {
            this.PeripheralContext = peripheralContext;
        }

        #endregion

        #region Public Events

        /// <summary>The audio status changed event.</summary>
        public event EventHandler<PeripheralVersionsInfo> PeripherialVersionChangedEvent;

        #endregion

        #region Public Properties

        /// <summary>The ximple created.</summary>
        /// <summary>Gets the audio switch context stream.</summary>
        public Stream ContextStream
        {
            get
            {
                return this.PeripheralContext?.Stream;
            }
        }

        /// <summary>Gets or sets a flag indicating whether the header is Read or Written in network byte order.</summary>
        public bool IsHeaderNetworkByteOrder
        {
            get
            {
                return this.PeripheralContext?.Config != null && this.PeripheralContext.Config.PeripheralHeaderInNetworkByteOrder;
            }

            set
            {
                if (this.PeripheralContext?.Config != null)
                {
                    this.PeripheralContext.Config.PeripheralHeaderInNetworkByteOrder = value;
                }
            }
        }

        /// <summary>Flag to test if medi is initialized.</summary>
        public bool IsMediInitialized { get; set; }

        /// <summary>Gets or sets the audio switch context.</summary>
        public PeripheralContext<TMessageType> PeripheralContext { get; set; }

        /// <summary>
        ///     Gets or sets the number of frame bytes to include in the Write buffer stream. Defaults to One. See
        ///     Constants.Fram
        /// </summary>
        public int PeripheralFramingBytesCount
        {
            get
            {
                return this.PeripheralContext?.Config != null
                           ? this.PeripheralContext.Config.DefaultMaxPeripheralFramingBytes
                           : Constants.DefaultMaxPeripheralFramingBytes;
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets a true flag if the Stream is a SerialStream</summary>
        private bool IsSerialStream
        {
            get
            {
                var streamType = this.ContextStream?.GetType();
                return streamType?.Name.Equals("SerialStream") ?? false;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets or sets a value indicating whether running.</summary>
        /// <summary>De-serialize using the MemoryStream and standard BinaryFormatter for .Net source and destinations</summary>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null. </exception>
        /// <returns>The <see cref="T"/>.</returns>
        /// <summary>The check sum.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="byte"/>.</returns>
        public static byte CalculateCheckSum<T>(T model) where T : class, IChecksum
        {
            return CheckSumUtil.CalculateCheckSum(model);
        }

        /// <summary>Find peripheral message class type from the enum message type.</summary>
        /// <returns>The <see cref="Type"/>.</returns>
        /// / KSH TODO
        /// <summary>Get bytes buffer ready to write by including the framing byte and setting the header to network byte order.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="headerInNetworkByteOrder">True to set the header In Network Byte Order.</param>
        /// <param name="totalFrameBytesToInclude">Total number of Frame Bytes to prefix the buffer with. Default is One</param>
        /// <exception cref="ArgumentException">Invalid or empty buffer, or if the bytes does not contain a valid peripheral header</exception>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] GetPeripheralBytesWriteBuffer(byte[] buffer, bool headerInNetworkByteOrder = true, int totalFrameBytesToInclude = 1)
        {
            if (buffer == null || buffer.Length == 0)
            {
                throw new ArgumentException("Invalid or empty buffer", nameof(buffer));
            }

            // The extra frame bytes are not necessary if the Stream is Not a SerialPort ie MemoryStream            
            if (totalFrameBytesToInclude < 1 || totalFrameBytesToInclude > 2)
            {
                totalFrameBytesToInclude = 1;
            }

            var writeBuffer = new byte[totalFrameBytesToInclude + buffer.Length];
            if (headerInNetworkByteOrder)
            {
                // convert the header to Network Byte order and replace it to the buffer
                var header = buffer.FromBytes<PeripheralHeader<TMessageType>>();
                if (header != null)
                {
                    if (headerInNetworkByteOrder)
                    {
                        header.UpdateHeaderToNetworkByteOrder();
                    }

                    var headerBytes = header.ToBytes();
                    headerBytes.CopyTo(buffer, 0);
                }
            }

            // Add the Frame Byte as the first part of a write buffer message. Normally this would be one byte but we allow up to two.
            for (int i = 0; i < totalFrameBytesToInclude; i++)
            {
                writeBuffer[i] = Constants.PeripheralFramingByte;
            }

            buffer.CopyTo(writeBuffer, totalFrameBytesToInclude);
            return writeBuffer;
        }

        /// <summary>Find peripheral message class type form the given header using the MEssageType as a lookup.</summary>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        /// <summary>Find peripheral message class type from the bytes buffer.</summary>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader!</exception>
        /// <returns>The <see cref="Type"/>Class Type</returns>
        /// <param name="peripheralHeader">The header.</param>
        /// <returns>The <see cref="Type"/>.</returns>
        public static Type PeripheralMessageTypeToClassType<T>(PeripheralHeader<T> peripheralHeader)
        {
            return peripheralHeader.MessageType.FindPeripheralMessageClassType(peripheralHeader.SystemType);
        }

        /// <summary>The find peripheral message type.</summary>
        /// <param name="handler">The handler.</param>
        /// <param name="bytes">The bytes.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
        /// <returns>The <see cref="Type"/>.</returns>
        public static Type PeripheralMessageTypeToClassType(IPeripheralHandler<TMessageType> handler, byte[] bytes)
        {
            // read past the framing byte if present in the array
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Invalid Handler in FindPeripheralMessageType()");
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                var model = handler.Read(memoryStream);
                if (model == null)
                {
                    Logger.Warn("FindPeripheralMessageType() unknown Message type from bytes");
                    return null;
                }

                Type messageType = model.GetType(); // class or object type expected to be our known models.
                return messageType;
            }
        }

        /// <summary>The dispose.</summary>
        public virtual void Dispose()
        {
            try
            {
                Debug.WriteLine("PeripherialHandler<" + typeof(TMessageType) + ">.Dispose");
                if (!this.disposed)
                {
                    if (this.IsMediInitialized)
                    {
                        Info("PeripheralHandler UnSubscribed Medi Messages");
                        this.UnInitMedi();
                        this.IsMediInitialized = false;
                    }
                }
            }
            finally
            {
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>The find peripheral message model.</summary>
        /// <param name="handler">The handler.</param>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IPeripheralBaseMessageType<TMessageType> FindPeripheralMessageModel<T>(IPeripheralHandler<TMessageType> handler, byte[] bytes)
        {
            // read past the framing byte if present in the array and find a base model of type 
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Invalid Handler in FindPeripheralMessageModel()");
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                return handler.Read(memoryStream) as IPeripheralBaseMessageType<TMessageType>;
            }
        }

        /// <summary>The process received messages.</summary>
        /// <param name="peripheralState">The state.</param>
        /// <param name="message">The message.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        public abstract IPeripheralBaseMessageType<TMessageType> ProcessReceivedMessages(
            PeripheralState<TMessageType> peripheralState, 
            object message);

        /// <summary>The read.</summary>
        /// <param name="readTimeout">The read timeout.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public object Read(int readTimeout = 0)
        {
            return this.Read(this.ContextStream);
        }

        /// <summary>The read.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <exception cref="ApplicationException">Failed to find the FromBytes method in assembly!!!</exception>
        /// <exception cref="AmbiguousMatchException">More than one method is found with the specified name. </exception>
        /// <returns>The <see cref="object"/>.</returns>
        public object Read(byte[] buffer)
        {
            object message = null;
            try
            {
                buffer.NLogTrace("Read Enter");

                // 1. A Framing octet in the protocol is used to frame the peripheral messages - 0x7E, find this to begin reading the header.
                // 2. We need to find the start of a message and attempt to read in a valid header where we can get it's message length and determine the expected message size.
                // 3. The message length will not include the one byte for the checksum, don't forget that extra byte to be read in where it will be the last byte.
                // 4. If we cannot de-serialize/read in a valid header we will send a Nak and clear the stream.
                int length = buffer?.Length ?? 0;

                // test if we have at least the bytes that represent the header's size in bytes in the stream
                if (length >= PeripheralHeader<TMessageType>.HeaderSize)
                {
                    Info("Read() Original buffer length = {0}, finding framing position in buffer to exclude...", length);
                    int framingPosition = buffer.FindFramingPosition();
                    if (framingPosition >= 0)
                    {
                        // Framing byte found in the stream, position or skip to the byte after this one, alter the buffer here to exclude frame byte
                        Logger.Trace(
                            "Read() Found the Frame Byte 0x{0:X} at position {1} in the buffer and removed from buffer", 
                            Constants.PeripheralFramingByte, 
                            framingPosition);
                        buffer = buffer.Skip(framingPosition + 1).ToArray();
                        length = buffer.Length; // This should be the full message with the one byte for checksum                          
                    }

                    Info("Read() Find valid PeripheralHeader. Buffer.Length = {0}, length = {1}", buffer.Length, length);

                    // Validate the buffer and a valid header can be created. Then test if we have the full message in the buffer.
                    // Protocol dictates the header will be in Network byte Order !!!! see HostToNetworkOrder Method
                    // Set appropriately if the bytes buffer given is from a true read operations vs unit testing!
                    var peripheralHeader = buffer.GetValidPeripheralHeader<TMessageType>(this.IsHeaderNetworkByteOrder);
                    if (peripheralHeader != null)
                    {
                        Info("Success, valid PeripheralHeader found {0}", peripheralHeader);

                        // do we have enough data read that is at least the size of the message ?
                        // recall the value of Length does not contain the Checksum byte
                        var messageType = peripheralHeader.MessageType;
                        if (length >= peripheralHeader.Length && peripheralHeader.IsUnknown == false)
                        {
                            // we have at least the full number of bytes                             
                            var currentClassType = messageType.FindPeripheralMessageClassType(peripheralHeader.SystemType); // use the system type and message type to determine the class type
                            if (currentClassType != null)
                            {
                                Debug.WriteLine("PeripheralMessageToType Type = " + currentClassType);
                                var methodInfo = typeof(Extensions).GetMethod("FromBytes");
                                if (methodInfo != null)
                                {
                                    var genericMethod = methodInfo.MakeGenericMethod(currentClassType);

                                    // Update the header with what we determined
                                    Array.Copy(peripheralHeader.ToBytes(), buffer, PeripheralHeader<TMessageType>.HeaderSize);
                                    message = genericMethod.Invoke(null, new[] { buffer });
                                }
                                else
                                {
                                    throw new ApplicationException("Failed to find the FromBytes method in assembly!!!");
                                }
                            }
                            else
                            {
                                throw new InvalidDataException("Failed to find Peripheral Entity type!");
                            }
                        }
                        else
                        {
                            Info(
                                "Peripheral Header found for type {0} but not expected, header Length {1} != bytes read {2}", 
                                peripheralHeader.MessageType, 
                                peripheralHeader.Length, 
                                length);
                        }
                    }
                }
                else
                {
                    Info("Read() Nothing in buffer to process exiting. Length = {0}", length);
                }
            }
            catch (InvalidDataException ex)
            {
                Logger.Error("Read() Invalid data exception {0}", ex.Message);
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                Logger.Error("Read() TimeoutException {0}", timeoutException.Message);
            }
            finally
            {
                // debug if no message dump the buffer content
                if (message == null)
                {
                    Warn("Did not find the Peripheral Header in the stream!");

                    // for debug dump the buffer that exists to review causes
                    buffer.NLogWarn("Read()");
                }
            }

            return message;
        }

        /// <summary>Read from the Stream a peripheral message object</summary>
        /// <param name="stream">The source stream.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        /// <exception cref="NotSupportedException">Stream does not support Length property(SerialPort.BaseStream) ie SerialPort
        ///     Stream</exception>
        /// <exception cref="ApplicationException">Failed to find the FromBytes method in assembly!!!</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="TargetException">NoteIn the .NET for Windows Store apps or the Portable Class Library, catch<see cref="T:System.Exception"/>
        ///     instead.The <paramref name="obj"/> parameter is null and the method is not
        ///     static.-or- The method is not declared or inherited by the class of <paramref name="obj"/>. -or-A static
        ///     constructor is invoked, and <paramref name="obj"/> is neither null nor an instance of the class that declared the
        ///     constructor.</exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public object Read(Stream stream)
        {
            Info("Read() from Stream Enter");
            if (stream == null)
            {
                stream = this.PeripheralContext.Stream;
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "Invalid null Stream in Read()");
            }

            // Note if the stream is serial Port base stream:
            // Due to the inaccessibility of the wrapped file handle, the Length and Position properties are not supported, and the Seek and SetLength methods are not supported.
            long bytesInStream = 0;
            try
            {
                // the total bytes in the stream, this is Framing byte, Data(The Message) and the checksum byte.
                // Note the Header.Length does not include the checksum byte in the length!
                bytesInStream = stream.Length;
            }
            catch (NotSupportedException notSupportedException)
            {
                Debug.WriteLine(notSupportedException.Message);
                Logger.Error("Read() failed NotSupported {0}", notSupportedException);
                throw;
            }

            object message = null;
            var streamPosition = stream.Position;
            Logger.Trace("Read() from stream bytes={0}, streamPosition={1}", bytesInStream, streamPosition);
            var buffer = new byte[bytesInStream];

            if (stream.CanRead == false || bytesInStream <= 0)
            {
                Logger.Warn("Read() Cannot read from stream or it is empty");
                return null;
            }

            try
            {
                // 1. A Framing octet in the protocol is used to frame the peripheral messages - 0x7E, find this to begin reading the header.
                // 2. We need to find the start of a message and attempt to read in a valid header where we can get it's message length and determine the expected message size.
                // 3. The message length will not include the one byte for the checksum, don't forget that extra byte to be read in where it will be the last byte.
                // 4. If we cannot de-serialize/read in a valid header we will send a Nak and clear the stream.

                // test if we have at least the bytes that represent the header's size in bytes in the stream
                if (bytesInStream >= PeripheralHeader<TMessageType>.HeaderSize)
                {
                    Logger.Trace("Reading from stream...buffer size {0}", buffer.Length);

                    // read up to our buffer length
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        message = this.Read(buffer);
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                Logger.Error("Read() Invalid data exception {0}", ex.Message);
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                Logger.Error("Read() TimeoutException {0}", timeoutException.Message);
            }
            finally
            {
                // valid header indicates we have partial message
                // why not rewind if we failed in any case ? KSH
                if (message == null /*&& validHeader*/)
                {
                    // we failed to find a message, rewind the file stream to the position we found it
                    stream.Position = streamPosition;
                }
            }

            return message;
        }

        /// <summary>Read from the stream as a series of bytes.</summary>
        /// <param name="stream">The stream or null to use the default context stream</param>
        /// <param name="readTimeout">Stream read timeout or Zero to use the default on the stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>The entity of type T.</returns>
        /// <exception cref="ArgumentNullException"><paramref name=""/> is <see langword="null"/>.</exception>
        public T Read<T>(Stream stream = null, int readTimeout = 0) where T : class, IPeripheralBaseMessageType<TMessageType>
        {
            var message = this.Read(stream);
            return message as T;
        }

        /// <summary>The remove next message.</summary>
        /// <param name="state">The state.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public object RemoveNextMessage(PeripheralState<TMessageType> state)
        {
            object peripheralMessage = null;

            try
            {
                state.LockStream();
                var streamBytes = state.MemoryStreamBytes;
                var bytesInStream = streamBytes.Length;

                if (bytesInStream > 0)
                {
                    int totalMessagesInStream = state.MemoryStreamBytes.Count(x => x == Constants.PeripheralFramingByte);
                    Logger.Info(
                        "RemoveNextMessage() Enter reading {0} bytes from stream. Total Peripheral Messages in stream = {1}", 
                        bytesInStream, 
                        totalMessagesInStream);

                    // Convert the bytes in the stream to a valid peripheral message or null if unknown
                    peripheralMessage = this.Read(streamBytes);

                    if (peripheralMessage != null)
                    {
                        // spot check - remove from the stream buffer the message length in bytes plus one byte for checksum
                        // TODO remove if accepted var baseMessage = peripheralMessage as IPeripheralBaseMessage<TMessageType>;
                        var baseMessage = peripheralMessage as PeripheralBaseMessage<TMessageType>;
                        if (baseMessage != null)
                        {
                            try
                            {
                                // remove the message from the stream, include one byte for checksum which is not include in the length by design
                                // extra byte for the Frame position octet as the first byte
                                // Note The Length property does not include the byte for the checksum or the Framing octet 0XFE
                                // the constant Size field of the type (T) however does.
                                int bytesToRemove = baseMessage.Header.Length;
                                var fieldInfo = peripheralMessage.GetType().GetField("Size");
                                if (fieldInfo != null)
                                {
                                    object sizeValue = fieldInfo.GetValue(peripheralMessage);
                                    if (baseMessage.Header.Length < Convert.ToUInt16(sizeValue))
                                    {
                                        // One byte for the Checksum that is NOT part of Length but is included in the total Model Size constant or sizeof(object)
                                        bytesToRemove++;
                                    }
                                }

                                // include at least one byte for the Framing byte, usually the first byte, we want to remove this from the buffer
                                var index = streamBytes.FindFramingPosition() + 1;
                                if (index > 0)
                                {
                                    bytesToRemove += index;
                                }

                                Logger.Info(
                                    "RemoveNextMessage() found valid peripheral message Header.Length={0}. Modify Stream, removing {1} bytes from stream for message type {2}", 
                                    baseMessage.Header.Length, 
                                    bytesToRemove, 
                                    baseMessage.Header.MessageType);
                                state.RemoveBytesFromStream(bytesToRemove);

#if DEBUG_BYTES_LEFT
                                var remainingBytes = state.StreamLength;
                                if (remainingBytes.Length == 1)
                                {
                                    Debugger.Launch();
                                }

                                if (totalMessagesInStream == 1 && (state.StreamLength != 0 && state.MemoryStreamPosition != 0))
                                {
                                    Logger.Error("Invalid Memory stream length - Bug! One Message in Stream and Buffer length expected to be Zero");
                                    Debugger.Launch();
                                }

#endif
                            }
                            catch (ArgumentException argumentException)
                            {
                                Logger.Error("RemoveNextMessage() Exception {0}", argumentException.Message);
                            }
                        }
                    }
                    else
                    {
                        Logger.Warn("RemoveNextMessage() Exit, no peripheral message found to remove, message = null");
                        state.MemoryStreamBytes.NLogTrace("Current");
                    }
                }
            }
            finally
            {
                Logger.Info("RemoveNextMessage() Exit Byte in stream buffer = {0}", state.StreamLength);
                state.UnLockStream();
            }

            return peripheralMessage;
        }

        /// <summary>The save version info.</summary>
        /// <param name="peripheralVersions">The peripheral versions.</param>
        /// <param name="fileName">The file name.</param>
        public void SaveVersionInfo(PeripheralVersionsInfo peripheralVersions, string fileName = "PeripherialVersions.xml")
        {
            Info("{0} Enter", nameof(this.SaveVersionInfo));
            if (peripheralVersions != null)
            {
                var serializer = new XmlSerializer(typeof(PeripheralVersionsInfo));
                using (var file = File.Create(fileName))
                {
                    Debug.WriteLine("Created Audio Switch Version file " + fileName);
                    serializer.Serialize(file, peripheralVersions);
                }

                this.PeripheralContext.PeripheralVersions = peripheralVersions;

                // Fire event the version has changed
                var handler = this.PeripherialVersionChangedEvent;
                handler?.Invoke(this, peripheralVersions);
            }
        }

        /// <summary>Write the framing octet(first byte in stream) and the model as series of bytes to the peripheral stream,
        ///     calculating the checksum.</summary>
        /// <param name="model">The peripheral model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>.</returns>
        public virtual int Write<T>(T model) where T : IPeripheralBaseMessageType<TMessageType>
        {
            Logger.Info("Write<{0}> to stream", typeof(T));
            var m = model as IPeripheralBaseMessageType<TMessageType>;
            return m != null ? this.Write(m, this.PeripheralContext.Stream) : 0;
        }

        /// <summary>The write.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public virtual int Write(byte[] buffer, Stream stream = null)
        {
            var headerSize = PeripheralHeader<TMessageType>.HeaderSize;
            if (buffer.Length < headerSize)
            {
                // bogus model
                Logger.Error("Write() Ignored buffer Length < {0}", headerSize);
                return 0;
            }

            if (stream == null)
            {
                stream = this.PeripheralContext.Stream;
            }

            Logger.Trace("Writing {0} bytes to stream", buffer.Length);
            stream.Write(buffer, 0, buffer.Length);

            return buffer.Length;
        }

        /// <summary>Write the framing octet, model as series of bytes to the peripheral stream, calculating the checksum.</summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream or null to use the existing AudioSwitchContext.Stream</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>Number of bytes written to stream</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        public virtual int Write<T>(T model, Stream stream) where T : IPeripheralBaseMessageType<TMessageType>
        {
            if (stream == null)
            {
                stream = this.PeripheralContext.Stream;
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "Invalid Stream in Write");
            }

            var bytesWritten = 0;
            if (stream.CanWrite)
            {
                // 1. Set the Length in the header correctly less one octet for the checksum field.
                // 2. Convert the model to series of bytes to be written to the stream. This is the header + payload and checksum (the model)
                // 3. Calculate the checksum and set it's value on the last byte which must be the checksum byte by the sequential class layout.
                // 4. Prepare the header if Network Byte order flip it.
                // 5. Write the Framing octet to the stream.
                // 6. Write the bytes buffer to the stream.

                // Set the correct length as Header(6 bytes) + Payload(if any) less the checksum byte
                if (model.Header.Length > 0)
                {
                    model.Header.Length--;  // Less One byte for the Checksum that is NOT in the Length field but should be set prio to calculating a CheckSum on TX
                }
                else
                {
                    // Note Marshal.SizeOf() will fail on a Generic Class!
                    model.Header.Length = (ushort)(Marshal.SizeOf<T>() - sizeof(byte));
                }

                // HACK for the audio switch hardware for use of RS485 emulator on PC where sometimes the first TX byte (0Xfe) is missed so we send two.
                // Note: 10/2016 this case may be related to the RS485 on the PC. 
                // Set PeripheralFramingBytesFramingByteCount > 1 if we need the peripheral to correctly respond to our writes.
                // The number of extra frame bytes can be 1 or 2.
                int extraFrameBytes = 1;
                if (this.IsSerialStream)
                {
                    extraFrameBytes = this.PeripheralFramingBytesCount;
                }

                var m = model as IPeripheralBaseMessageType<TMessageType>;
                var writeBuffer = GetPeripheralBytesWriteBuffer(m, this.IsHeaderNetworkByteOrder, extraFrameBytes);
                Debug.Assert(writeBuffer[0] == Constants.PeripheralFramingByte, "Missing Framing byte as first byte in buffer");

                // note first byte should be the Framing Byte 0x7E
                writeBuffer.DebugDump("Tx");
                stream.Write(writeBuffer, 0, writeBuffer.Length);
                stream.Flush();
                bytesWritten = writeBuffer.Length;
            }

            return bytesWritten;
        }

        /// <summary>The write ack message to context stream.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public virtual int WriteAck(ushort address = Constants.DefaultPeripheralAudioSwitchAddress)
        {
            Info("WriteAck");
            var bytes = new PeripheralAck(address).ToBytes();
            return this.Write(bytes, this.PeripheralContext.Stream);
        }

        /// <summary>Write the framing  byte to the stream. This is the first byte that starts outgoing messages in the protocol</summary>
        /// <param name="framing">The framing. Default see Constants.FramingByte</param>
        /// <returns>Number of bytes written to stream.</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public virtual int WriteFraming(byte framing = Constants.PeripheralFramingByte)
        {
            var stream = this.PeripheralContext.Stream;
            if (stream != null && stream.CanWrite)
            {
                stream.WriteByte(framing);
                return sizeof(byte);
            }

            return 0;
        }

        /// <summary>The write nak message to context stream.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public virtual int WriteNak(ushort address = Constants.DefaultPeripheralAudioSwitchAddress)
        {
            Info("WriteNak");
            var bytes = new PeripheralNak(address).ToBytes();
            return this.Write(bytes, this.PeripheralContext.Stream);
        }

        /// <summary>The write response.</summary>
        /// <param name="validChecksum">The valid checksum.</param>
        public void WriteResponse(bool validChecksum)
        {
            if (validChecksum)
            {
                this.WriteAck();
            }
            else
            {
                this.WriteNak();
            }
        }

        #endregion

        #region Methods

        /// <summary>The info.</summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected static void Info(string format, params object[] args)
        {
            Logger.Info(format, args);
            Debug.WriteLine(format, args);
        }

        /// <summary>The warn.</summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected static void Warn(string format, params object[] args)
        {
            Logger.Warn(format, args);
            Debug.WriteLine(format, args);
        }

        /// <summary>The init medi.</summary>
        /// <param name="mediFileName">The medi file name.</param>
        protected abstract void InitMedi(string mediFileName);

        /// <summary>The un-initialize medi called on Dispose.</summary>
        protected abstract void UnInitMedi();

        /// <summary>The get bytes to write to the Peripheral.</summary>
        /// <param name="model">The model.</param>
        /// <param name="headerInNetworkByteOrder">The header in network byte order.</param>
        /// <param name="totalFrameBytesToInclude">Total number of Frame Bytes to prefix the buffer with. Default is One</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte[]"/>.</returns>
        private static byte[] GetPeripheralBytesWriteBuffer<T>(T model, bool headerInNetworkByteOrder = true, int totalFrameBytesToInclude = 1)
            where T : class, IPeripheralBaseMessageType<TMessageType>
        {
            // The Length byte in the Header does not include the byte for the Checksum (Not my Idea KSH)
            // So we expect the Header.Length here to be correct. Sizeof(Model) - One Byte for checksum
            // Convert to byte array and re-calculate the checksum and set as the last byte            
            var buffer = model.ToBytes();

            // update the checksum which must be the last byte
            var checksum = CheckSumUtil.CheckSum(buffer);

            // Last byte in array is the checksum by definition            
            buffer[buffer.Length - 1] = checksum;

            return GetPeripheralBytesWriteBuffer(buffer, headerInNetworkByteOrder, totalFrameBytesToInclude);
        }

        #endregion

        // public event EventHandler<XimpleEventArgs> XimpleCreated;

        // : IDisposable
    }
}