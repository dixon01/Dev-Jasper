// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Eci.Serialization
{
    using System;
    using System.IO;

    using Messages;

    /// <summary>
    /// The Eci serializer.
    /// </summary>
    public class EciSerializer
    {
        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception when the serialization if message does not exist.
        /// </exception>
        public static void Serialize(Stream stream, EciMessageBase msg)
        {
            EciBinaryPacket packet;
            if (msg is EciAckTs)
            {
                packet = CreatePacket((EciAckTs)msg);
            }
            else if (msg is EciAck)
            {
                packet = CreatePacket((EciAck)msg);
            }
            else if (msg is EciDelayedMessage)
            {
                packet = CreatePacket((EciDelayedMessage)msg);
            }
            else if (msg is EciKeepAliveMessage)
            {
                packet = CreatePacket((EciKeepAliveMessage)msg);
            }
            else if (msg is EciNewMessage)
            {
                packet = CreatePacket((EciNewMessage)msg);
            }
            else if (msg is EciPositionMessage)
            {
                packet = CreatePacket((EciPositionMessage)msg);
            }
            else if (msg is EciLogMessage)
            {
                packet = CreatePacket((EciLogMessage)msg);
            }
            else if (msg is EciPassengerCountMessage)
            {
                packet = CreatePacket((EciPassengerCountMessage)msg);
            }
            else if (msg is EciDutyMessage)
            {
                packet = CreatePacket((EciDutyMessage)msg);
            }
            else if (msg is EciTrafficLightAck)
            {
                packet = CreatePacket((EciTrafficLightAck)msg);
            }
            else if (msg is EciTrafficLightEntry)
            {
                packet = CreatePacket((EciTrafficLightEntry)msg);
            }
            else if (msg is EciTrafficLightExit)
            {
                packet = CreatePacket((EciTrafficLightExit)msg);
            }
            else if (msg is EciTrafficLightCheckPoint)
            {
                packet = CreatePacket((EciTrafficLightCheckPoint)msg);
            }
            else if (msg is EciUtilMessage)
            {
                packet = CreatePacket((EciUtilMessage)msg);
            }
            else if (msg is EciTextMessage)
            {
                packet = CreatePacket((EciTextMessage)msg);
            }
            else
            {
                throw new Exception("Unknown ECI message");
            }

            packet.SetCheckSum();
            stream.Write(packet.Buffer, 0, packet.PacketSize);
        }

        /// <summary>
        /// Deserializes from binary packets.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The message<see cref="EciMessageBase"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when the deserialization of the packet is not implemented.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciMessageBase Deserialize(EciBinaryPacket packet)
        {
            switch (packet.TypeCode)
            {
                case EciMessageCode.PositionV3:
                    return CreatePositionMessage(packet);
                case EciMessageCode.Duty:
                    return CreateDutyMessage(packet);
                case EciMessageCode.Ack:
                    return CreateAckMessage(packet);
                case EciMessageCode.AckTs:
                    return CreateAckTsMessage(packet);
                case EciMessageCode.Alarm:
                    return CreateAlarmMessage(packet);
                case EciMessageCode.Delay:
                    return CreateDelayedMessage(packet);
                case EciMessageCode.Log:
                    return CreateLogMessage(packet);
                case EciMessageCode.GeneralMessageNewFormat:
                    return CreateNewFormatMessage(packet);
                case EciMessageCode.PassengerCount:
                    return CreatePassengerCountMessage(packet);
                case EciMessageCode.Util:
                    return CreateUtilMessage(packet);
                case EciMessageCode.TrafficLight:
                    switch ((EciTrafficLightCode)packet.SubTypeCode)
                    {
                        case EciTrafficLightCode.Ack:
                            return CreateTrafficLightAckMessage(packet);
                        case EciTrafficLightCode.Entry:
                            return CreateTrafficLightEntryMessage(packet);
                        case EciTrafficLightCode.Checkpoint:
                            return CreateTrafficLightCheckPointMessage(packet);
                        case EciTrafficLightCode.Exit:
                            return CreateTrafficLightExitMessage(packet);
                        default:
                            throw new Exception("Unknown ECI Traffic packet");
                    }

                default:
                    throw new Exception("Unknown ECI packet");
            }
        }

        /* Methods ported from Eci.c */

        /// <summary>
        /// The create position packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciPositionMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(33);
            binaryPacket.AppendByte((byte)EciMessageCode.PositionV3);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte((byte)msg.PositionEvent);
            binaryPacket.AppendUShort(msg.StopId);
            binaryPacket.AppendDateTime(msg.TimeStamp);
            binaryPacket.AppendByte(msg.GpsNumberSats);
            binaryPacket.AppendFloat(msg.Longitude);
            binaryPacket.AppendFloat(msg.Latitude);
            binaryPacket.AppendByte((byte)Math.Floor(msg.Direction / 2.0));
            binaryPacket.AppendByte((byte)msg.SpeedKmS);
            binaryPacket.AppendInt(msg.BlockId);
            binaryPacket.AppendInt(msg.TripId);
            binaryPacket.AppendUShort(msg.LineId);
            binaryPacket.AppendByte(((int)msg.AlarmState & 0xF0) | (msg.AlarmId & 0x0F));
            return binaryPacket;
        }

        /// <summary>
        /// Creates a position message.
        /// </summary>
        /// <param name="binaryPacket">
        /// The binary packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciPositionMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciPositionMessage CreatePositionMessage(EciBinaryPacket binaryPacket)
        {
            if (binaryPacket.ParseByte() != (byte)EciMessageCode.PositionV3)
            {
                throw new Exception("Wrong Eci Packet, position expected");
            }

            var msg = new EciPositionMessage();
            msg.VehicleId = binaryPacket.ParseUShort();
            msg.PositionEvent = (PositionEvent)binaryPacket.ParseByte();
            msg.StopId = binaryPacket.ParseUShort();
            msg.TimeStamp = binaryPacket.ParseDateTime();
            msg.GpsNumberSats = binaryPacket.ParseByte();
            msg.Longitude = binaryPacket.ParseFloat();
            msg.Latitude = binaryPacket.ParseFloat();
            msg.Direction = binaryPacket.ParseByte() * 2;
            msg.SpeedKmS = binaryPacket.ParseByte();
            msg.BlockId = binaryPacket.ParseInt();
            msg.TripId = binaryPacket.ParseInt();
            msg.LineId = binaryPacket.ParseUShort();
            msg.AlarmState = (EciAlarmState)binaryPacket.ParseByte();
            return msg;
        }

        /// <summary>
        /// The create alarm packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciAlarmMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(18);
            binaryPacket.AppendByte((byte)EciMessageCode.Alarm);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendDateTime(msg.TimeStamp);
            binaryPacket.AppendByte((byte)msg.AlarmState);
            binaryPacket.AppendByte((byte)msg.GpsState);
            binaryPacket.AppendFloat(msg.Longitude);
            binaryPacket.AppendFloat(msg.Latitude);
            return binaryPacket;
        }

        /// <summary>
        /// The create alarm message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciAlarmMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciAlarmMessage CreateAlarmMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.Alarm)
            {
                throw new Exception("Wrong Eci packet, Util expected");
            }

            var msg = new EciAlarmMessage();
            msg.VehicleId = packet.ParseUShort();
            msg.TimeStamp = packet.ParseDateTime();
            msg.AlarmState = (EciAlarmState)packet.ParseByte();
            msg.GpsState = (GpsState)packet.ParseByte();
            msg.Longitude = packet.ParseFloat();
            msg.Latitude = packet.ParseFloat();
            return msg;
        }

        /// <summary>
        /// Create an acknowledgment packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciAck msg)
        {
            var binaryPacket = new EciBinaryPacket(6);
            binaryPacket.AppendByte((byte)EciMessageCode.Ack);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte((byte)msg.Type);
            binaryPacket.AppendUShort(msg.Value);
            return binaryPacket;
        }

        /// <summary>
        /// Create an acknowledgment message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciMessageBase"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciMessageBase CreateAckMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.Ack)
            {
                throw new Exception("Wrong Eci packet, Ack expected");
            }

            var msg = new EciAck();
            msg.VehicleId = packet.ParseUShort();
            msg.Type = (AckType)packet.ParseByte();
            msg.Value = packet.ParseUShort();
            return msg;
        }

        /// <summary>
        /// Creates a util packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciUtilMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(4);
            binaryPacket.AppendByte((byte)EciMessageCode.Util);
            binaryPacket.AppendByte((byte)msg.SubType);
            binaryPacket.AppendUShort(msg.VehicleId);
            return binaryPacket;
        }

        /// <summary>
        /// Creates an util message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciUtilMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciUtilMessage CreateUtilMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.Util)
            {
                throw new Exception("Wrong Eci packet, Util expected");
            }

            var msg = new EciUtilMessage();
            msg.SubType = (EciRequestCode)packet.ParseByte();
            msg.VehicleId = packet.ParseUShort();
            return msg;
        }

        /// <summary>
        /// Creates a text packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciTextMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(21 + msg.DisplayText.Length + msg.TtsText.Length);
            binaryPacket.AppendByte((byte)msg.MessageType);
            binaryPacket.AppendByte((byte)msg.SubType);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte(0);
            binaryPacket.AppendByte(0);
            binaryPacket.AppendByte(0);
            binaryPacket.AppendInt(msg.MessageId);
            binaryPacket.AppendByte((byte)msg.Target);
            binaryPacket.AppendByte((byte)msg.Duration.TotalSeconds);
            binaryPacket.AppendByte((byte)msg.CycleTime.TotalSeconds);
            binaryPacket.AppendByte((byte)msg.TotalDuration.TotalSeconds);
            binaryPacket.AppendInt(msg.MessageMp3);
            binaryPacket.AppendByte(msg.DisplayText.Length);
            binaryPacket.AppendByte(msg.TtsText.Length);
            binaryPacket.AppendString(msg.DisplayText);
            binaryPacket.AppendString(msg.TtsText);
            return binaryPacket;
        }

        /// <summary>
        /// Creates a text message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciTextMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciTextMessage CreateTextMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.TextMessage)
            {
                throw new Exception("Wrong Eci packet, Text expected");
            }

            var msg = new EciTextMessage();
            msg.SubType = (char)packet.ParseByte(); // offset:3
            msg.VehicleId = packet.ParseUShort(); // offset:4

            // TODO: [ALM] what are these three bytes?
            packet.ParseByte();
            packet.ParseByte();
            packet.ParseByte();
            msg.MessageId = packet.ParseInt(); // offset:8
            msg.Target = (MessageTarget)packet.ParseByte(); // offset:12
            msg.Duration = new TimeSpan(0, 0, packet.ParseByte()); // offset:13
            msg.CycleTime = new TimeSpan(0, 0, packet.ParseByte()); // offset:14
            msg.TotalDuration = new TimeSpan(0, 0, packet.ParseByte()); // offset:15
            msg.MessageMp3 = packet.ParseInt(); // offset:16
            var displayLen = packet.ParseByte(); // offset:20
            var ttsLen = packet.ParseByte(); // offset:21
            msg.DisplayText = packet.ParseString(displayLen); // offset:22
            msg.TtsText = packet.ParseString(ttsLen); // offset:22+displayLen
            return msg;
        }

        /// <summary>
        /// The create log packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciLogMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(5 + msg.LogText.Length);
            binaryPacket.AppendByte((byte)EciMessageCode.Log);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte((byte)msg.LogLevel);
            binaryPacket.AppendByte(msg.LogText.Length);
            binaryPacket.AppendString(msg.LogText);
            return binaryPacket;
        }

        /// <summary>
        /// The create log message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciLogMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciLogMessage CreateLogMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.Log)
            {
                throw new Exception("Wrong Eci packet, Text expected");
            }

            EciLogMessage msg = new EciLogMessage();
            msg.VehicleId = packet.ParseUShort();
            msg.LogLevel = (EciLogCode)packet.ParseByte();
            var textLength = packet.ParseByte();
            msg.LogText = packet.ParseString(textLength);
            return msg;
        }

        /// <summary>
        /// The create keep alive packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciKeepAliveMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(3);
            binaryPacket.AppendByte((byte)EciMessageCode.KeepAlive);
            binaryPacket.AppendUShort(msg.VehicleId);
            return binaryPacket;
        }

        /// <summary>
        /// The create keep alive message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciKeepAliveMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciKeepAliveMessage CreateKeepAliveMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.KeepAlive)
            {
                throw new Exception("Wrong Eci packet, Text expected");
            }

            return new EciKeepAliveMessage { VehicleId = packet.ParseUShort() };
        }

        /// <summary>
        /// The create traffic light entry packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciTrafficLightEntry msg)
        {
            var binaryPacket = new EciBinaryPacket(27);
            binaryPacket.AppendByte((byte)EciMessageCode.TrafficLight);
            binaryPacket.AppendByte((byte)EciTrafficLightCode.Entry);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte(1);
            binaryPacket.AppendByte(msg.IntersectionId);
            binaryPacket.AppendInt(67150);
            binaryPacket.AppendUShort(msg.RouteId);
            binaryPacket.AppendTime(msg.GpsUtcTimeStamp.TimeOfDay);
            binaryPacket.AppendByte((byte)msg.SpeedKmS);
            binaryPacket.AppendUShort((ushort)msg.Distance);
            binaryPacket.AppendUShort((ushort)msg.Distance2);
            binaryPacket.AppendUShort((ushort)msg.Distance3);
            binaryPacket.AppendByte((byte)msg.Time.TotalMinutes);
            binaryPacket.AppendByte((byte)msg.Time2.TotalMinutes);
            binaryPacket.AppendByte((byte)msg.Time3.TotalMinutes);
            return binaryPacket;
        }

        /// <summary>
        /// The create traffic light entry message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciTrafficLightEntry"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciTrafficLightEntry CreateTrafficLightEntryMessage(EciBinaryPacket packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create traffic light check point packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciTrafficLightCheckPoint msg)
        {
            var binaryPacket = new EciBinaryPacket(21);
            binaryPacket.AppendByte((byte)EciMessageCode.TrafficLight);
            binaryPacket.AppendByte((byte)EciTrafficLightCode.Checkpoint);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte(1);
            binaryPacket.AppendByte(msg.IntersectionId);
            binaryPacket.AppendInt(67150);
            binaryPacket.AppendUShort(msg.RouteId);
            binaryPacket.AppendTime(msg.GpsUtcTimeStamp.TimeOfDay);
            binaryPacket.AppendByte((byte)msg.SpeedKmS);
            binaryPacket.AppendUShort((ushort)msg.Distance);
            binaryPacket.AppendByte((byte)msg.Time.TotalMinutes);
            return binaryPacket;
        }

        /// <summary>
        /// The create traffic light check point message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciTrafficLightCheckPoint"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciTrafficLightCheckPoint CreateTrafficLightCheckPointMessage(EciBinaryPacket packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create traffic light exit packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciTrafficLightExit msg)
        {
            var binaryPacket = new EciBinaryPacket(18);
            binaryPacket.AppendByte((byte)EciMessageCode.TrafficLight);
            binaryPacket.AppendByte((byte)EciTrafficLightCode.Exit);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendByte(1);
            binaryPacket.AppendByte(msg.IntersectionId);
            binaryPacket.AppendInt(67150);
            binaryPacket.AppendUShort(msg.RouteId);
            binaryPacket.AppendTime(msg.GpsUtcTimeStamp.TimeOfDay);
            binaryPacket.AppendByte((byte)msg.SpeedKmS);
            return binaryPacket;
        }

        /// <summary>
        /// The create traffic light exit message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciTrafficLightExit"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciTrafficLightExit CreateTrafficLightExitMessage(EciBinaryPacket packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create traffic light acknowledge packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciTrafficLightAck msg)
        {
            var binaryPacket = new EciBinaryPacket(17);
            binaryPacket.AppendByte((byte)msg.MessageType);
            binaryPacket.AppendByte((byte)msg.SubType);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendInt(msg.Acknowledge);
            binaryPacket.AppendInt(msg.Value);
            return binaryPacket;
        }

        /// <summary>
        /// The create eci traffic light acknowledge message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciTrafficLightAck"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciTrafficLightAck CreateTrafficLightAckMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.TrafficLight)
            {
                throw new Exception("Wrong Eci packet, Text expected");
            }

            if (packet.ParseByte() != (byte)EciTrafficLightCode.Ack)
            {
                throw new Exception("Wrong Eci packet, Trafic Ack expected");
            }

            EciTrafficLightAck msg = new EciTrafficLightAck { VehicleId = packet.ParseUShort() };

            return msg;
        }

        /* Methods ported from EciBus.c */

        /// <summary>
        /// The create new message format.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciNewMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(20);
            binaryPacket.AppendByte((byte)EciMessageCode.GeneralMessageNewFormat);
            binaryPacket.AppendByte(msg.PositionType);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendTime(msg.TimeStamp.TimeOfDay);
            binaryPacket.AppendUShort(msg.LineNumber);
            binaryPacket.AppendInt(msg.ServiceNumber);
            binaryPacket.AppendInt(msg.RouteId);
            binaryPacket.AppendUShort(msg.PositionId);
            binaryPacket.AppendByte(msg.VehicleType);
            return binaryPacket;
        }

        /// <summary>
        /// The create new format message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciNewMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciNewMessage CreateNewFormatMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.GeneralMessageNewFormat)
            {
                throw new Exception("Wrong Eci packet, duty expected");
            }

            var msg = new EciNewMessage();
            msg.PositionType = (char)packet.ParseByte();
            msg.VehicleId = packet.ParseUShort();
            TimeSpan ts = packet.ParseTime();

            // TODO: what is the right behavior for this?
            msg.TimeStamp = new DateTime(0, 0, 0, ts.Hours, ts.Minutes, ts.Seconds);
            msg.LineNumber = packet.ParseUShort();
            msg.ServiceNumber = packet.ParseInt();
            msg.RouteId = packet.ParseInt();
            msg.PositionId = packet.ParseUShort();
            msg.VehicleType = packet.ParseByte();
            return msg;
        }

        /// <summary>
        /// The create passenger count packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciPassengerCountMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(30);
            binaryPacket.AppendByte((byte)EciMessageCode.PassengerCount);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendTime(msg.TimeStamp.TimeOfDay);
            binaryPacket.AppendFloat(msg.Longitude);
            binaryPacket.AppendFloat(msg.Latitude);
            binaryPacket.AppendUShort(msg.LineId);
            binaryPacket.AppendInt(msg.BlockId);
            binaryPacket.AppendUShort(msg.PathId);
            binaryPacket.AppendUShort(msg.StopId);
            binaryPacket.AppendUShort(msg.DriverId);
            binaryPacket.AppendUShort(msg.NumberOfPassengers);
            return binaryPacket;
        }

        /// <summary>
        /// The create passenger count message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciPassengerCountMessage"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciPassengerCountMessage CreatePassengerCountMessage(EciBinaryPacket packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create vehicle delayed packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciDelayedMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(17);
            binaryPacket.AppendByte((byte)EciMessageCode.Delay);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendInt(msg.ServiceNumber);
            binaryPacket.AppendUShort(msg.LineNumber);
            binaryPacket.AppendInt(msg.RouteId);
            binaryPacket.AppendUShort(msg.StopId);
            binaryPacket.AppendUShort((ushort)msg.EstimatedDelay.TotalMinutes);
            return binaryPacket;
        }

        /// <summary>
        /// The create delayed message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciDelayedMessage"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciDelayedMessage CreateDelayedMessage(EciBinaryPacket packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create duty packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciDutyMessage msg)
        {
            var binaryPacket = new EciBinaryPacket(18);
            binaryPacket.AppendByte((byte)EciMessageCode.Duty);
            binaryPacket.AppendByte(msg.LoginType);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendTime(msg.TimeStamp.TimeOfDay);
            binaryPacket.AppendInt(msg.ServiceNumber);
            binaryPacket.AppendInt(msg.DriverId);
            binaryPacket.AppendUShort(msg.DriverPin);
            binaryPacket.AppendByte(msg.Option);
            return binaryPacket;
        }

        /// <summary>
        /// The create duty message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// Returns an ECI duty message<see cref="EciDutyMessage"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciDutyMessage CreateDutyMessage(EciBinaryPacket packet)
        {
            if (packet.ParseByte() != (byte)EciMessageCode.Duty)
            {
                throw new Exception("Wrong Eci packet, duty expected");
            }

            var msg = new EciDutyMessage();
            msg.LoginType = (char)packet.ParseByte();
            msg.VehicleId = packet.ParseUShort();
            TimeSpan ts = packet.ParseTime();

            // TODO: what is the right behavior for this?
            msg.TimeStamp = new DateTime(0, 0, 0, ts.Hours, ts.Minutes, ts.Seconds);
            msg.ServiceNumber = packet.ParseInt();
            msg.DriverId = packet.ParseInt();
            msg.DriverPin = packet.ParseUShort();
            msg.Option = packet.ParseByte();
            return msg;
        }

        /// <summary>
        /// The create Acknowledge TS packet.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="EciBinaryPacket"/>.
        /// </returns>
        public static EciBinaryPacket CreatePacket(EciAckTs msg)
        {
            var binaryPacket = new EciBinaryPacket(12);
            binaryPacket.AppendByte((byte)EciMessageCode.AckTs);
            binaryPacket.AppendByte(msg.SubType);
            binaryPacket.AppendUShort(msg.VehicleId);
            binaryPacket.AppendTime(msg.TimeStamp.TimeOfDay);
            binaryPacket.AppendByte(msg.Value);
            binaryPacket.AppendInt(msg.Reference);
            return binaryPacket;
        }

        /// <summary>
        /// The create acknowledge TS message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// The <see cref="EciAckTs"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Throws an exception when a wrong packet is passed.
        /// </exception>
        public static EciAckTs CreateAckTsMessage(EciBinaryPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
