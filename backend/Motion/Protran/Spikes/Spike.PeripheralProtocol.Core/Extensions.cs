// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="Extensions.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    /// <summary>The reflections helper.</summary>
    public static class Extensions
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<PeripheralHeader, System.Type> messagesToTypeDictionary = new Dictionary<PeripheralHeader, Type>();

        private static readonly Dictionary<Type, IList<MemberInfo>> PropsCache = new Dictionary<Type, IList<MemberInfo>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>The debug dump.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="prefix">The prefix.</param>
        public static void DebugDump(this byte[] buffer, string prefix)
        {
#if DEBUG
            Debug.Write(prefix + " | ");
            var s = buffer.Aggregate(string.Empty, (current, b) => current + string.Format("0x{0:X},", b));
            Debug.WriteLine(s);
#endif
        }

        public static void NLogTrace(this byte[] buffer, string prefix)
        {
            var s = buffer.Aggregate(string.Empty, (current, b) => current + string.Format("0x{0:X},", b));
            Logger.Trace("{0} | {1}", prefix, s);
        }
        public static void NLogWarn(this byte[] buffer, string prefix)
        {
            var s = buffer.Aggregate(string.Empty, (current, b) => current + string.Format("0x{0:X},", b));
            Logger.Warn("{0} | {1}", prefix, s);
        }

        /// <summary>The object fields to bytes.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte[]"/>.</returns>
        /// <exception cref="AmbiguousMatchException">More than one method is found with the specified name and specified
        ///     parameters.</exception>
        public static byte[] FieldsToBytes<T>(this T model)
        {
            var bytesList = new List<byte>();
            Debug.WriteLine("Start of FieldsToBytes for " + typeof(T));

            // ignore constant literals
            foreach (var field in model.GetType().GetFields().Where(m => m.IsLiteral == false))
            {
                object value = field.GetValue(model);
                if (field.Name.ToLower() == "checksum")
                {
                    // the checksum should be the last byte in the class model by definition of the protocol
                    var lastProp = model.GetType().GetProperties().Last().Name.ToLower() == "checksum";
                    if (!lastProp)
                    {
                        Logger.Warn("Last Prop in model {0} is not Checksum", typeof(T));
                    }
                }

                var sizeConst = 0;
                var marshalAsAttribute = field.GetCustomAttributes(typeof(MarshalAsAttribute), false).FirstOrDefault() as MarshalAsAttribute;
                if (marshalAsAttribute != null)
                {
                    sizeConst = marshalAsAttribute.SizeConst;
                    Logger.Trace("Using Size={0} for property {1}", sizeConst, field.Name);
                }

                if (field.FieldType.IsClass && field.FieldType == typeof(string))
                {
                    if (sizeConst > 0)
                    {
                        Debug.WriteLine(" SizeConst = {0} for Field {1}", sizeConst, field.Name);
                        var s = value as string;
                        if (s != null)
                        {
                            var bytes = s.GetBytes(sizeConst);
                            bytesList.AddRange(bytes);
                        }

                        continue;
                    }
                }

                var isByte = field.FieldType.IsEnum ? Enum.GetUnderlyingType(field.FieldType).Name == "Byte" : field.FieldType.Name == "Byte";

                if (isByte)
                {
                    Debug.WriteLine("  Byte Field {0} = {1} Bytes[ {2} ]", field.Name, value, (byte)value);
                    bytesList.Add((byte)value);
                }
                else
                {
                    var propType = field.FieldType.IsEnum ? typeof(int) : field.FieldType;
                    var method = typeof(BitConverter).GetMethod("GetBytes", new[] { propType });
                    if (method != null)
                    {
                        byte[] bytes = (byte[])method.Invoke(null, new[] { value });
                        Debug.WriteLine("  Field {0} = {1} Bytes[ {2} ]", field.Name, value, string.Join(",", bytes));
                        bytesList.AddRange(bytes);
                    }
                    else if (field.FieldType.Name == "Byte[]")
                    {
                        byte[] bytes = value as byte[];
                        if (bytes != null && bytes.Length < sizeConst)
                        {
                            Array.Resize(ref bytes, sizeConst);
                        }

                        if (bytes != null)
                        {
                            Debug.WriteLine("  Byte[] Field {0} = {1} Bytes[ {2} ]", field.Name, value, string.Join(",", bytes));
                            bytesList.AddRange(bytes);
                        }
                    }
                }
            }

#if DEBUG
            var debugString = bytesList.ToArray().Aggregate(string.Empty, (current, x) => current + string.Format("0x{0:X},", x));
            Debug.Write(debugString);
            Debug.WriteLine("FieldsToBytes End");
#endif
            return bytesList.ToArray();
        }

        public static int CountConsecutiveFramingPosition(this byte[] bytes)
        {
            var frameBytes = 0;
            if (bytes!= null && bytes.Length > 0)
            {
                int i = 0;
                while (i < bytes.Length && bytes[i++] == Constants.PeripheralFramingByte)
                {
                    frameBytes++;
                }
            }

            return frameBytes;
        }

        /// <summary>The find framing position.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="framingByte">The framing byte.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int FindFramingPosition(this byte[] bytes, byte framingByte = Constants.PeripheralFramingByte)
        {
            if (bytes != null && bytes.Length > 0)
            {
                return Array.IndexOf(bytes, framingByte);
            }

            return -1;
        }

        /// <summary>Find peripheral message class type from the enum message type.</summary>
        /// <param name="peripheralMessageType">The peripheral message type.</param>
        /// <param name="systemMessageType"></param>
        /// <returns>The <see cref="Type"/>.</returns>
        public static Type FindPeripheralMessageClassType<TMessagType>(this TMessagType peripheralMessageType, PeripheralSystemMessageType systemMessageType)
        {
            Type classType = null;
            lock (messagesToTypeDictionary)
            {
                if (messagesToTypeDictionary.Count == 0)
                {
                    InitPeripheralClassTypesDictionary<PeripheralMessageType>();
                    InitPeripheralClassTypesDictionary<PeripheralAudioSwitchMessageType>();
#if DEBUG
                    foreach (var item in messagesToTypeDictionary)
                    {
                        Debug.WriteLine(string.Format("Key={0}, Value={1}", item.Key, item.Value));
                    }
#endif
                }

                byte msgTypeValue = 0;
                try
                {
                    msgTypeValue = Convert.ToByte(peripheralMessageType);
                }
                catch (InvalidCastException)
                {
                }
                
                var key = messagesToTypeDictionary.Keys.Where(m => m.SystemType == systemMessageType).FirstOrDefault(n => n.MessageType == msgTypeValue);
                if (key != null)
                {
                    // class Type found using the key lookup
                    classType = messagesToTypeDictionary[key];
                }
                else
                {
                    Logger.Error("Failed to find Message type {0} in dictionary!", peripheralMessageType);
                }
            }

            return classType;
        }

        /// <summary>The from byte array.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <typeparam name="T"></typeparam>
        ///// <returns>The <see cref="T"/>.</returns>
        // [Obsolete("Not Used")]
        //public static T FromByteArray<T>(this byte[] bytes) where T : class
        //{
        //    T obj = default(T);
        //    int sizeOf = Marshal.SizeOf(obj);
        //    var ptr = Marshal.AllocHGlobal(sizeOf);
        //    Marshal.Copy(bytes, 0, ptr, sizeOf);
        //    obj = (T)Marshal.PtrToStructure(ptr, obj.GetType());
        //    Marshal.FreeHGlobal(ptr);
        //    return obj;
        //}

        /// <summary>The from bytes.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        /// <exception cref="NotImplementedException">Types other than Property or Field</exception>
        /// <exception cref="InvalidDataException">Invalid enumerations values.</exception>
        /// <exception cref="NotSupportedException">Invalid runtime values.</exception>
        /// <exception cref="MissingMethodException">T is an interface not a class.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded. </exception>
        public static T FromBytes<T>(this byte[] bytes) where T : class
        {
            if (typeof(T).IsInterface)
            {
                throw new MissingMethodException("FromBytes<T>() Cannot create an instance of an interface for Type " + typeof(T));
            }

            if (typeof(T).IsAbstract)
            {
                throw new MissingMethodException("FromBytes<T>() Cannot create an instance of an abstract class for Type " + typeof(T));
            }

            T model = Activator.CreateInstance<T>();
            var idx = 0;
            var properties = model.GetOrderedPropertiesForObject();
            foreach (var prop in properties)
            {
                if (idx >= bytes.Length)
                {
                    // we have more props than bytes to work with
                    break;
                }

                var ignoreAttribute = prop.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false).FirstOrDefault() as IgnoreDataMemberAttribute;
                if (ignoreAttribute != null)
                {
                    Debug.WriteLine(" ignoreAttribute!=null custom attribute found for " + prop.Name);
                    continue;
                }

                var propType = prop.PropertyType();
                var propTypeName = prop.Name;

                if (prop.IsClass())
                {
                    // invoke generic for the type to get the class props 
                    var st = new StackTrace();
                    var methodBase = st.GetFrame(0).GetMethod();

                    var method = typeof(Extensions).GetMethod(methodBase.Name);
                    if (method != null)
                    {
                        var genericMethod = method.MakeGenericMethod(propType);
                        Debug.WriteLine("\r\nClass " + propType);
                        var size = 0;
                        if (propTypeName == "Header")
                        {
                            size = PeripheralBaseMessageType<PeripheralHeader>.Size;
                        }
                        else if (prop.DeclaringType.IsGenericType == false)
                        {
                            size = Marshal.SizeOf(propType);
                        }
                      
                        var b = bytes.Skip(idx).Take(size).ToArray();
                        object m = genericMethod.Invoke(null, new object[] { b });
                        if (m != null)
                        {
                            prop.SetValue(model, m);
                            idx += size;
                        }

                        continue;
                    }
                }

                var isByte = prop.IsEnum() ? Enum.GetUnderlyingType(prop.PropertyType()).Name == "Byte" : prop.PropertyType().Name == "Byte";
                if (isByte)
                {
                    var value = bytes[idx++];
                    Debug.WriteLine("  Set {0} = {1} Byte[ {2} ]", prop.Name, value, value);
                    if (prop.Name == "Checksum")
                    {
                        Debug.WriteLine("Setting Checksum value = " + value);
                    }

                    // is the byte an enum  and does it equate. Throw exception if the value is not ours
                    if (prop.IsEnum())
                    {
                        // validate enums are correct values
                        if (!Enum.IsDefined(prop.PropertyType(), value))
                        {
#if DEBUG_EXTENSION
                            Debugger.Launch();
#endif
                            var s = string.Format("Invalid enum value 0x{0:X} is not valid for enum type {1}. Prop type is enum but is unknown for enum type.", value, prop.PropertyType());
                            Logger.Error("FromBytes() Failed  {0}", s);
                            Debug.WriteLine(s);
                            // debug dump the buffer
                            s = bytes.Aggregate(string.Empty, (current, b) => current + string.Format("0x{0:X},", b));
                            Logger.Warn(s);
                            throw new InvalidDataException(s);
                        }
                    }

                    prop.SetValue(model, value);
                }
                else
                {
                    // ToInt16 as example convert from that to bytes
                    var methodName = "To" + propType.Name;
                    var method = typeof(BitConverter).GetMethod(methodName);
                    if (method != null)
                    {
                        Type p = propType.IsEnum ? typeof(int) : propType;
                        int size = Marshal.SizeOf(p);
                        var source = bytes.Skip(idx).Take(size).ToArray();
                        var value = method.Invoke(null, new object[] { source, 0 });
                        idx += size;
                        prop.SetValue(model, value);
                        Debug.WriteLine("  {0} = {1} Bytes[ {2} ]", propTypeName, value, string.Join(",", bytes));
                    }
                    else if (propType.IsArray)
                    {
                        Debug.WriteLine("Setting Array from Bytes");
                        var name = propType.Name.Replace("[]", string.Empty);

                        var marshalAsAttribute = prop.GetCustomAttributes(typeof(MarshalAsAttribute), false).FirstOrDefault() as MarshalAsAttribute;
                        if (marshalAsAttribute != null && name.Contains("Byte"))
                        {
                            var sizeAttribute = marshalAsAttribute.SizeConst;
                            if (sizeAttribute > 0)
                            {
                                var byteArray = bytes.Skip(idx).Take(sizeAttribute).ToArray();
                                prop.SetValue(model, byteArray);
                                idx += sizeAttribute;
                            }
                            else
                            {
                                throw new NotSupportedException("Custom Size attribute is invalid! " + propType.Name);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("Array type unsupported " + propType.Name);
                        }
                    }
                    else if (propType.IsValueType)
                    {
                        // TODO Handle the Structs, set the value
                        var size = propType.StructLayoutAttribute.Size;
                        Debug.WriteLine("Sturct Size = " + size);

                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.Copy(bytes, idx, ptr, size);
                        var value = Marshal.PtrToStructure(ptr, propType);
                        Marshal.FreeHGlobal(ptr);
                        prop.SetValue(model, value);
                        idx += size;

                        // bump the idx by size of the prop or structure size
                    }
                }
            }

            return model;
        }

        /// <summary>The get fields and properties.</summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingAttr">The binding attr.</param>
        /// <returns>The <see cref="List"/>.</returns>
        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty)
        {
            var targetMembers = new List<MemberInfo>();
            targetMembers.AddRange(type.GetFields(bindingAttr));
            targetMembers.AddRange(type.GetProperties(bindingAttr).Where(m=> m.CanRead && m.CanWrite));

            if (targetMembers.Any(m => m.GetCustomAttributes(typeof(OrderAttribute), false).Any()))
            {
                var sortedList = targetMembers.OrderBy(
                    m =>
                        {
                            var o = m.GetCustomAttributes(typeof(OrderAttribute), false).SingleOrDefault() as OrderAttribute;
                            return o != null ? o.Order : 0;
                        });

                return sortedList.ToList();
            }

            return targetMembers;
        }

        /// <summary>The get ordered properties for.</summary>
        /// <param name="t">The t.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="IList"/>.</returns>
        public static IList<MemberInfo> GetOrderedPropertiesFor<T>(Type t = null) where T : class
        {
            Type type = t ?? typeof(T);
            lock (PropsCache)
            {
                IList<MemberInfo> props;
                if (PropsCache.TryGetValue(type, out props))
                {
                    if (props != null)
                    {
                        return props;
                    }
                }

                var fieldsAndProperties = GetFieldsAndProperties(type);
                PropsCache.Add(type, fieldsAndProperties);
                return fieldsAndProperties;
            }
        }

        /// <summary>The get ordered properties for object.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="IList"/>Collection of MemberInfo</returns>
        public static IList<MemberInfo> GetOrderedPropertiesForObject<T>(this T model) where T : class
        {
            return model != null ? GetOrderedPropertiesFor<T>(model.GetType()) : new List<MemberInfo>();
        }

        /// <summary>The get start of message.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="framingByte">The framing byte.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] GetStartOfMessage(this byte[] buffer, byte framingByte = Constants.PeripheralFramingByte)
        {
            int framingPosition = buffer.FindFramingPosition();
            if (framingPosition >= 0)
            {
                // Framing byte found in the stream, position or skip to the byte after this one
                Logger.Trace("Read() Found the FrameByte in position {0} in the buffer", framingPosition);
                buffer = buffer.Skip(framingPosition + 1).ToArray();
            }

            return buffer;
        }

        /// <summary>Validate the buffer contains to determine if we have a valid header and the buffer length is valid</summary>
        /// <param name="buffer">Buffer containing a full message without the framing octet</param>
        /// <param name="headerInNetworkByteOrder">True if buffer contains header in Network Byte Order</param>
        /// <returns>The <see cref="bool"/>PeripheralHeader if valid else null.</returns>
        public static PeripheralHeader<TMessageType> GetValidPeripheralHeader<TMessageType>(this byte[] buffer, bool headerInNetworkByteOrder)
        {
            try
            {
                if (buffer == null || buffer.Length == 0)
                {
                    Logger.Warn("GetValidPeripheralHeader() Invalid Buffer, null or empty");
                    return null;
                }

                var headerSize = PeripheralHeader<TMessageType>.HeaderSize;
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

                Logger.Info("GetValidPeripheralHeader() testing if new bytes buffer.Length = {0} >= Expected PeripheralSize {1}", buffer.Length, headerSize);

                // get the header, validate the message and system types (enums), enough data to consider ?
                if (buffer.Length >= headerSize)
                {
                    // attempt to create header from the buffer
                    var peripheralHeader = buffer.FromBytes<PeripheralHeader<TMessageType>>();
                    if (peripheralHeader == null)
                    {
                        throw new InvalidDataException("FromBytes<PeripheralHeader> data buffer cannot be converted to a know valid header");
                    }

                    // flip the bytes for network byte order if told to and the length does not look valid
                    if (headerInNetworkByteOrder)
                    {
                        // a case could exists where the header/bytes is already in the correct order
                        // Flipping it when told we expect it and it has already been done would create a bug
                        Type classType = peripheralHeader.MessageType.FindPeripheralMessageClassType(peripheralHeader.SystemType);

                        Logger.Trace("CreateInstance of messageType = {0} to validate and flip the header bytes for network byte order", classType);
                        var tempModel = Activator.CreateInstance(classType) as IPeripheralBaseMessageType<TMessageType>;
                        if (tempModel != null && tempModel.Header.Length != peripheralHeader.Length)
                        {
                            Logger.Trace("Set Peripheral Header to NetworkToHostByteOrder tempModel.Header.Length {0} != header.Length {1}", tempModel.Header.Length, peripheralHeader.Length);
                            peripheralHeader.NetworkToHostByteOrder();
                        }
                    }

                    Logger.Trace("Current Peripheral Header Length = {0}, for MessageType {1}, headerSize={2}", peripheralHeader.Length, peripheralHeader.MessageType, headerSize);
                    if (peripheralHeader.Length >= headerSize)
                    {
                        // does the buffer have at least the required bytes for the full message ?
                        // KSH REVIEWED 10/13/16, return the header always if we have it and account for data shortage
                        if (buffer.Length >= peripheralHeader.Length)
                        {
                            Logger.Info("GetValidPeripheralHeader() found Valid Header = {0}", peripheralHeader);
                        }
                        else
                        {
                            Logger.Warn("Buffer is Short Data, Read Again  buffer.Length = {0} < header.Length = {1}", buffer.Length, peripheralHeader.Length);
                        }

                        return peripheralHeader;
                    }
                    else
                    {
                        Logger.Warn("GetValidPeripheralHeader() Invalid Length in Header Value={0}", peripheralHeader.Length);
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

        /// <summary>The get value.</summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="forObject">The for object.</param>
        /// <returns>The <see cref="object"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    Logger.Warn("Unknown member type not supported in GetValue(). Type = {0}", memberInfo.MemberType);
                    throw new NotImplementedException();
            }
        }
      

        //public static void UpdateHeaderToNetworkByteOrder(this IPeripheralHeader header)
        //{
        //    header.Length = HostToNetworkOrder(header.Length);
        //    header.Address = HostToNetworkOrder(header.Address);
        //}

        ///// <summary>The is ack message.</summary>
        ///// <param name="header">The message.</param>
        ///// <returns>The <see cref="bool"/>.</returns>
        //public static bool IsAckMessage(this IPeripheralHeader header)
        //{
        //    return header.MessageType == (ushort)PeripheralMessageType.Ack;
        //}

        ///// <summary>The is nak message.</summary>
        ///// <param name="message">The message.</param>
        ///// <returns>The <see cref="bool"/>.</returns>
        //public static bool IsNakMessage(this IPeripheralHeader message)
        //{
        //    return message.MessageType == (ushort)PeripheralMessageType.Nak;
        //}

        /// <summary>The is class.</summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsClass(this MemberInfo memberInfo)
        {
            bool isPrimitiveType;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    if (fieldInfo != null && string.IsNullOrEmpty(fieldInfo.FieldType.Namespace) == false)
                    {
                        isPrimitiveType = fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.Namespace.Equals("System");
                        return fieldInfo.FieldType.IsClass && isPrimitiveType == false;
                    }

                    break;

                case MemberTypes.Property:
                    var propInfo = memberInfo as PropertyInfo;
                    if (propInfo != null && string.IsNullOrEmpty(propInfo.PropertyType.Namespace) == false)
                    {
                        isPrimitiveType = propInfo.PropertyType.IsPrimitive || propInfo.PropertyType.Namespace.Equals("System");
                        return propInfo.PropertyType.IsClass && isPrimitiveType == false;
                    }

                    break;
            }

            return false;
        }

        /// <summary>The is enum.</summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsEnum(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType.IsEnum;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType.IsEnum;
                default:
                    return false;
            }
        }

        /// <summary>The is message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsMessage<T>(this IPeripheralBaseMessageType<T> message, T messageType)
        {
            var msgType = message != null ? message.Header.MessageType : default(T);
            return msgType.Equals(messageType);
        }
        
        /// <summary>The network byte order.</summary>
        /// <param name="header">The header.</param>
        /// <returns>The <see cref="PeripheralHeader"/>.</returns>
        public static PeripheralHeader<T> NetworkToHostByteOrder<T>(this PeripheralHeader<T> header)
        {
            header.Length = NetworkToHostByteOrder(header.Length);
            header.Address = NetworkToHostByteOrder(header.Address);
            return header;
        }

        /// <summary>The host to network byte order.</summary>
        /// <param name="header">The header.</param>
        /// <returns>The <see cref="IPeripheralHeader<T>"/>.</returns>
        public static PeripheralHeader<T> HostToNetworkByteOrder<T>(this PeripheralHeader<T> header)
        {
            header.Length = HostToNetworkOrder(header.Length);
            header.Address = HostToNetworkOrder(header.Address);
            return header;
        }

        /// <summary>The property type.</summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns>The <see cref="Type"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Type PropertyType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>The set value.</summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="forObject">The for object.</param>
        /// <param name="value">The value.</param>
        public static void SetValue(this MemberInfo memberInfo, object forObject, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(forObject, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(forObject, value);
                    break;
            }
        }

        /// <summary>The to byte array.</summary>
        /// <param name="obj">The obj.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] ToByteArray<T>(this T obj) where T : class
        {
            if (obj is string)
            {
                return Encoding.ASCII.GetBytes(obj as string);
            }

            var size = Marshal.SizeOf(obj); // caution if type T is generic!
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        public static int GetExpectedMessageSize<T>(this T model) where T : class, IPeripheralBaseMessageType<PeripheralAudioSwitchMessageType>
        {
            var fieldInfo = typeof(T).GetField("Size");
            if (fieldInfo != null)
            {
                return (int) fieldInfo.GetValue(model);
            }
            return (int)model?.Header.Length + 1;   // one byte for Checksum which is NOT part of the Length field in header. Not my idea
        }

        /// <summary>The object to bytes.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] ToBytes<T>(this T model) where T : class
        {
            var bytesList = new List<byte>();
            var className = typeof(T).FullName.Split('.').Last();
            var props = model.GetOrderedPropertiesForObject();
#if DEBUG
            var stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(1).GetMethod();
            Debug.WriteLine(string.Format("{0} called ToBytes() Start of Type={1}, properties {2}", callingMethod, className, props.Count));
#endif
            // get the public class properties
            foreach (var prop in props)
            {
                var ignoreAttribute = prop.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false).FirstOrDefault() as IgnoreDataMemberAttribute;
                if (ignoreAttribute != null)
                {
                    Debug.WriteLine(" ignoreAttribute!=null custom attribute found for " + prop.Name);
                    continue;
                }

                Type propType = prop.PropertyType();
                object value = prop.GetValue(model);
                // Debug.WriteLine("=> Set {0} = 0x{1:X}", prop.Name, value);
                // Exclude Checksum prop in the bytes returned
                if (prop.Name.ToLower() == "checksum")
                {
                    bool lastProp = model.GetOrderedPropertiesForObject().Last().Name.ToLower() == "checksum";
                    if (!lastProp)
                    {
                        Logger.Warn("Last Prop in model {0} is not Checksum", typeof(T));
                    }
                    else
                    {
                        Debug.WriteLine("Checksum property found correctly as last property OK, value = " + value);
                    }

                    // Debug.WriteLine(string.Format("{0} = {1} Bytes[ {2} ]", prop.Name, value, (byte)value));
                    // continue;
                }

                if (prop.IsClass())
                {
                    Debug.WriteLine("* IsClass == true");

                    // invoke generic for the type to get the class props 
                    var st = new StackTrace();
                    var methodBase = st.GetFrame(0).GetMethod();

                    var methodInfo = typeof(Extensions).GetMethod(methodBase.Name);
                    if (methodInfo != null)
                    {
                        // var genericMethod = methodInfo.MakeGenericMethod(prop.PropertyType);
                        var genericMethod = methodInfo.MakeGenericMethod(propType);
                        Debug.WriteLine("\r\nClass " + propType);
                        byte[] bytes = (byte[])genericMethod.Invoke(null, new[] { value });
                        if (bytes != null)
                        {
                            bytesList.AddRange(bytes);
                            continue;
                        }
                    }
                }

                var isEnum = propType.IsEnum;
                var isByte = isEnum ? Enum.GetUnderlyingType(propType).Name == "Byte" : propType.Name == "Byte";

                if (isByte)
                {
                    Debug.WriteLine(" Byte Prop {0} = 0x{1:X} ({2})", prop.Name, (byte)value, (byte)value);
                    bytesList.Add((byte)value);
                    continue;
                }

                var marshalAsAttribute = prop.GetCustomAttributes(typeof(MarshalAsAttribute), false).FirstOrDefault() as MarshalAsAttribute;
                if (marshalAsAttribute != null)
                {
                    var sizeConst = marshalAsAttribute.SizeConst;
                    Logger.Debug("* found custom marshal prop attribute Size={0} for property {1}", sizeConst, prop.Name);
                }

                // for properties find the GetBytes method on the property type
                var method = typeof(BitConverter).GetMethod("GetBytes", new[] { propType });
                if (method != null)
                {
                    Debug.WriteLine("Invoke GetBytes() on propType = " + propType);
                    byte[] bytes = (byte[])method.Invoke(null, new[] { value });
                    Debug.WriteLine(" Prop {0} = 0x{1:X} ({2})", prop.Name, value, value);
                    bytesList.AddRange(bytes);
                    continue;
                }

                if (prop.MemberType == MemberTypes.Field)
                {
                    if (propType.IsAnsiClass)
                    {
                        if (propType.Name == "Byte[]")
                        {
                            var sourceBytes = value as Byte[];
                            if (sourceBytes != null)
                            {
                                Debug.WriteLine("* Field Type found {0}.{1} Size = {2}", className, prop.Name, sourceBytes.Length);
                                bytesList.AddRange(sourceBytes);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Ignored MemberTypes.Field " + prop.Name);
                    }
                }
                else if (prop.MemberType == MemberTypes.Property && propType.IsValueType)
                {
                    var bytes = value.ToByteArray();
                    Debug.WriteLine("Property value type found {0} Size = {1}", prop.Name, bytes.Length);
                    bytesList.AddRange(bytes);
                }
            }

#if DEBUG
            Debug.Write("\r\nDump ToBytes() Start | ");
            Debug.Write(bytesList.Aggregate(string.Empty, (current, x) => current + string.Format("0x{0:X},", x)));
            Debug.WriteLine(" | ToBytes() End");
#endif

            return bytesList.ToArray();
        }

        /// <summary>
        /// Create AudioStatusMessage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="hardwareVersion"></param>
        /// <param name="softwareVersion"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public static AudioStatusMessage ToAudioStatusMessage(
          this PeripheralAudioStatus p,
          string hardwareVersion = "",
          string softwareVersion = "",
          string serialNumber = "")
        {
            if (p == null)
            {
                return null;
            }

            var a = new AudioStatusMessage
            {
                InteriorVolume = p.InteriorVolume,
                InteriorNoiseLevel = p.InteriorNoiseLevel,
                InteriorActive = p.ActiveSpeaker.HasFlag(ActiveSpeakerZone.Interior),
                ExteriorVolume = p.ExteriorVolume,
                ExteriorActive = p.ActiveSpeaker.HasFlag(ActiveSpeakerZone.Exterior),
                ExteriorNoiseLevel = p.ExteriorNoiseLevel,
                Initialized = true,
                HardwareVersion = hardwareVersion,
                SoftwareVersion = softwareVersion,
                SerialNumber = serialNumber
            };
            return a;
        }


        /// <summary>The from peripheral audio config.</summary>
        /// <param name="p">The p.</param>
        /// <returns>The <see cref="VolumeSettingsMessage"/>.</returns>
        public static VolumeSettingsMessage ToVolumeSettingsMessage(this PeripheralAudioConfig p)
        {
            var volumeSettings = new VolumeSettingsMessage
            {
                Interior =
                                             {
                                                 MinimumVolume = p.InteriorMinVolume,
                                                 MaximumVolume = p.InteriorMaxVolume,
                                                 CurrentVolume = p.InteriorDefaultVolume,
                                                 DefaultVolume = p.InteriorDefaultVolume
                                             },
                Exterior =
                                             {
                                                 MinimumVolume = p.ExteriorMinVolume,
                                                 MaximumVolume = p.ExteriorMaxVolume,
                                                 CurrentVolume = p.InteriorDefaultVolume,
                                                 DefaultVolume = p.InteriorDefaultVolume
                                             }
            };

            return volumeSettings;
        }

        #endregion

        #region Methods

        private static byte[] GetBytes(this string str, int expectedSize = 0)
        {
            int len = Math.Min(str?.Length ?? 0, expectedSize);
            if (len == 0)
            {
                len = expectedSize;
            }

            if (str != null)
            {
                var bytes = Encoding.ASCII.GetBytes(str.ToCharArray(), 0, len);
                if (bytes.Length < expectedSize)
                {
                    Array.Resize(ref bytes, expectedSize);
                }

                return bytes;
            }

            return new byte[0];
        }

        private static ushort HostToNetworkOrder(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        private static void InitPeripheralClassTypesDictionary<TMessageType>()
        {
            Debug.WriteLine("InitPeripheralTypesDictionary() Enter");
            var assembly = typeof(PeripheralBaseMessage<PeripheralMessageType>).Assembly;
            var enumNames = Enum.GetNames(typeof(TMessageType)).ToList();
            lock (messagesToTypeDictionary)
            {
                try
                {
                    Logger.Trace("Init peripheral types....");
                    var messageEntities =
                        assembly.GetTypes()
                            .Where(
                                m =>
                                m.IsClass &&
                                m.IsAbstract == false && 
                                m.ContainsGenericParameters == false).ToList();
                    foreach (Type classType in messageEntities)
                    {
                        Logger.Trace("className = {0}", classType.Name);
                        if (classType.GetProperty("Header") != null)
                        {
                            //if (classType.IsAbstract || classType.ContainsGenericParameters)
                            //{
                            //    continue;
                            //}

                            // test for parameter less constructor which is not supported, skip entity 
                            var constructor = classType.GetConstructor(Type.EmptyTypes);
                            if (constructor == null)
                            {
                                Logger.Error("Class Type {0} is missing a required Parameter less constructor", classType);
                                continue;
                            }

                            // test can be created
                            var model = Activator.CreateInstance(classType);
                            var baseMessage = model as IPeripheralBaseMessageType<TMessageType>;
                            if (baseMessage != null)
                            {
                                var hdr = new PeripheralHeader(baseMessage.Header.SystemType, baseMessage.Header.MessageType);
                                if (enumNames.Contains(baseMessage.Header.MessageType.ToString())
                                    && messagesToTypeDictionary.ContainsKey(hdr) == false)
                                {
                                    messagesToTypeDictionary.Add(
                                        new PeripheralHeader(baseMessage.Header.SystemType, Convert.ToByte(baseMessage.Header.MessageType)),
                                        model.GetType());
                                    Debug.WriteLine("Found Type = " + classType.FullName);
                                }
                                else
                                {
                                    // ignored
                                    Logger.Debug("Ignored class {0}", classType.Name);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Debug.WriteLine("InitPeripheralTypesDictionary() Exit");
                }
            }
        }

        private static ushort NetworkToHostByteOrder(ushort value)
        {
            return (ushort)((value & 0xFF00U) >> 8 | (value & 0xFFU) << 8);
        }

        [Obsolete]
        private static byte[] ObjectToBytes<T>(T model) where T : class
        {
            return model.ToBytes();
        }

        [Obsolete]
        private static byte[] ToBinaryBytesArray<T>(this T model) where T : class
        {
            using (var memStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(memStream, model);
                    return memStream.ToArray();
                }
                catch (SerializationException exception)
                {
                    Logger.Error(exception, "CalcluateCheckSum Failed to serialize. Reason: " + exception.Message);
                    throw;
                }
            }
        }

        #endregion
    }
}