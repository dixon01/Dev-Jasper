namespace Luminator.DimmerPeripheral.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using NLog;
    
    using Luminator.DimmerPeripheral.Core.Interfaces;
    using Luminator.DimmerPeripheral.Core.Models;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;

    /// <summary>
    /// Extension methods for the dimmer peripheral
    /// </summary>

    [Obsolete("See Luminator.PeripheralDimmer")]
    public static class DimmerExtensions
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<DimmerPeripheralMessageType, Type> messageTypesReceivedDictionary = new Dictionary<DimmerPeripheralMessageType, Type>();

        private static ManualResetEvent dimmerPeripheralTypesDictionaryCreated = new ManualResetEvent(false);

        #endregion

        #region Public Methods and Operators

        /// <summary>Validate the buffer contains to determine if we have a valid header and the buffer length is valid</summary>
        /// <param name="buffer">Buffer containing a full message without the framing octet</param>
        /// <param name="headerInNetworkByteOrder">True if buffer contains header in Network Byte Order</param>
        /// <returns>The <see cref="bool"/>PeripheralHeader if valid else null.</returns>
        public static DimmerPeripheralHeader GetValidDimmerPeripheralHeader(this byte[] buffer, bool headerInNetworkByteOrder)
        {
            try
            {
                if (buffer == null || buffer.Length == 0)
                {
                    Logger.Warn("GetValidPeripheralHeader() Invalid Buffer, null or empty");
                    return null;
                }

                // the framing byte can be part of the buffer, read past this if present
                int framingPosition = buffer.FindFramingPosition();
                if (framingPosition == 0)// KSH Changed 7/27 from >= 0 buffer may have had the first byte as frame removed already and we have stacked messages
                {
                    // Framing byte found in the stream, position or skip to the byte after this one
                    Logger.Trace("GetValidPeripheralHeader() Frame position at Offset = {0}, removing frame byte from buffer", framingPosition);
                    buffer = buffer.Skip(framingPosition + 1).ToArray();
                }
                else if (framingPosition > 0)
                {
                    Logger.Warn("GetValidPeripheralHeader() Framing byte found at index {0} in bytes array and left in buffer. Buffer.Length = {1}", framingPosition, buffer.Length);
                }

                Logger.Info("GetValidPeripheralHeader() testing if new bytes buffer.Length = {0} >= Expected PeripheralSize {1}", buffer.Length, PeripheralHeader.Size);

                // get the header, validate the message and system types (enums), enough data to consider ?
                if (buffer.Length >= PeripheralHeader.Size)
                {
                    // attempt to create header from the buffer
                    var dimmerPeripheralHeader = buffer.FromBytes<DimmerPeripheralHeader>();
                    if (dimmerPeripheralHeader == null)
                    {
                        throw new InvalidDataException("FromBytes<PeripheralHeader> data buffer cannot be converted to a know valid header");
                    }

                    // flip the bytes for network byte order if told to and the length does not look valid
                    if (headerInNetworkByteOrder)
                    {
                        // a case could exists where the header/bytes is already in the correct order
                        // Flipping it when told we expect it and it has already been done would create a bug
                        var messageType = dimmerPeripheralHeader.MessageType.FindPeripheralMessageType();

                        Logger.Trace("CreateInstance of messageType = {0} to validate and flip the header bytes for network byte order", messageType);
                        var tempModel = Activator.CreateInstance(messageType) as IPeripheralBaseMessage;
                        if (tempModel != null && tempModel.Header.Length != dimmerPeripheralHeader.Length)
                        {
                            Logger.Trace("Set Peripheral Header to NetworkToHostByteOrder tempModel.Header.Length {0} != header.Length {1}", tempModel.Header.Length, dimmerPeripheralHeader.Length);
                            dimmerPeripheralHeader.NetworkToHostByteOrder();
                        }
                    }

                    Logger.Trace("Current Peripheral Header Length = {0}, for MessageType {1}", dimmerPeripheralHeader.Length, dimmerPeripheralHeader.MessageType);
                    if (dimmerPeripheralHeader.Length >= PeripheralHeader.Size)
                    {
                        // does the buffer have at least the required bytes for the full message ?
                        // KSH REVIEWED 10/13/16, return the header always if we have it and account for data shortage
                        if (buffer.Length >= dimmerPeripheralHeader.Length)
                        {
                            Logger.Info("GetValidPeripheralHeader() found Valid Header = {0}", dimmerPeripheralHeader);
                        }
                        else
                        {
                            Logger.Warn("Buffer is Short Data, Read Again  buffer.Length = {0} < header.Length = {1}", buffer.Length, dimmerPeripheralHeader.Length);
                        }

                        return dimmerPeripheralHeader;
                    }
                    else
                    {
                        Logger.Warn("GetValidPeripheralHeader() Invalid Length in Header Value={0}", dimmerPeripheralHeader.Length);
                    }
                }
                else
                {
                    Logger.Warn("GetValidPeripheralHeader() current buffer is to small for valid header, Length = {0}", buffer.Length);
                }
            }
            catch (InvalidDataException invalidDataException)
            {
                Logger.Error("GetValidPeripheralHeader() {0}, re-throwing exception InvalidDataException", invalidDataException.Message);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetValidPeripheralHeader() general exception Invalid message buffer {0}", ex.Message);
            }

            return null;
        }

        /// <summary>Find peripheral message class type from the enum message type.</summary>
        /// <param name="peripheralMessageType">The peripheral message type.</param>
        /// <returns>The <see cref="Type"/>.</returns>
        public static Type FindPeripheralMessageType(this DimmerPeripheralMessageType peripheralMessageType)
        {
            Type t;
            lock (messageTypesReceivedDictionary)
            {
                if (messageTypesReceivedDictionary.Count == 0)
                {
                    InitDimmerPeripheralTypesDictionary();
                }

                if (messageTypesReceivedDictionary.TryGetValue(peripheralMessageType, out t) == false)
                {
                    Logger.Error("Failed to find Message type {0} in dictionary!", peripheralMessageType);
                }

            }

            return t;
        }
        
        private static void InitDimmerPeripheralTypesDictionary()
        {
            Debug.WriteLine("InitDimmerPeripheralTypesDictionary() Enter");
            var assembly = typeof(DimmerPeripheralBaseMessage).Assembly;
            var enums = Enum.GetValues(typeof(DimmerPeripheralMessageType)) as IEnumerable<DimmerPeripheralMessageType>;
            var allMessageTypes = new List<DimmerPeripheralMessageType>(enums);
            lock (messageTypesReceivedDictionary)
            {
                try
                {
                    Logger.Trace("Init dimmer peripheral types....");
                    foreach (Type classType in assembly.GetTypes().Where(m => m.IsClass))
                    {
                        if (classType.GetProperty("Header") != null)
                        {
                            if (classType.IsAbstract)
                            {
                                continue;
                            }

                            var model = Activator.CreateInstance(classType) as IDimmerPeripheralBaseMessage;
                            if (model != null)
                            {
                                if (allMessageTypes.Contains(model.Header.MessageType)
                                    && messageTypesReceivedDictionary.ContainsKey(model.Header.MessageType) == false)
                                {
                                    messageTypesReceivedDictionary.Add(model.Header.MessageType, classType);
                                    Debug.WriteLine("Found Type = " + classType.FullName);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Debug.WriteLine("InitDimmerPeripheralTypesDictionary() Exit");
                    dimmerPeripheralTypesDictionaryCreated.Set();
                }
            }
        }
        
        /// <summary>The remove framing bytes.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] RemoveFramingBytes(this byte[] bytes)
        {
            return bytes.SkipWhile(m => m.Equals(DimmerConstants.PeripheralFramingByte)).ToArray();
        }

        /// <summary>The host to network byte order.</summary>
        /// <param name="header">The header.</param>
        /// <returns>The <see cref="PeripheralHeader"/>.</returns>
        public static PeripheralHeader HostToNetworkByteOrder(this PeripheralHeader header)
        {
            header.Length = HostToNetworkOrder(header.Length);
            header.Address = HostToNetworkOrder(header.Address);
            return header;
        }

        /// <summary>The network byte order to host.</summary>
        /// <param name="header">The header.</param>
        /// <returns>The <see cref="PeripheralHeader"/>.</returns>
        public static PeripheralHeader NetworkByteOrderToHost(this PeripheralHeader header)
        {
            header.Length = NetworkToHostByteOrder(header.Length);
            header.Address = NetworkToHostByteOrder(header.Address);
            return header;
        }
        
        #endregion

        #region Methods

        private static ushort HostToNetworkOrder(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        private static ushort NetworkToHostByteOrder(ushort value)
        {
            return (ushort)((value & 0xFF00U) >> 8 | (value & 0xFFU) << 8);
        }

        #endregion
    }
}
