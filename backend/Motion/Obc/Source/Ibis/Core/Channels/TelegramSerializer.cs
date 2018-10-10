// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable SuggestBaseTypeForParameter
namespace Gorba.Motion.Obc.Ibis.Core.Channels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// The serializer for IBIS telegrams.
    /// </summary>
    public class TelegramSerializer
    {
        private const byte CarriageReturn = (byte)'\r';

        private static readonly Logger Logger = LogHelper.GetLogger<TelegramSerializer>();

        /// <summary>
        /// Serializes a telegram to the given stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="config">
        /// The telegram configuration.
        /// </param>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Big switch over all possible telegrams, not nicer if refactored into multiple methods")]
        public void Serialize(Stream stream, Telegram telegram, TelegramConfigBase config)
        {
            var ibisStream = new IbisStream(stream);

            var ds001 = telegram as DS001;
            if (ds001 != null)
            {
                this.Serialize(ibisStream, ds001);
                return;
            }

            var ds002 = telegram as DS002;
            if (ds002 != null)
            {
                this.Serialize(ibisStream, ds002);
                return;
            }

            var ds003 = telegram as DS003;
            if (ds003 != null)
            {
                this.Serialize(ibisStream, ds003, (DS003Config)config);
                return;
            }

            var ds003C = telegram as DS003C;
            if (ds003C != null)
            {
                this.Serialize(ibisStream, ds003C, (DS003CConfig)config);
                return;
            }

            var ds004 = telegram as DS004;
            if (ds004 != null)
            {
                this.Serialize(ibisStream, ds004, (DS004Config)config);
                return;
            }

            var ds004A = telegram as DS004A;
            if (ds004A != null)
            {
                this.Serialize(ibisStream, ds004A);
                return;
            }

            var ds004B = telegram as DS004B;
            if (ds004B != null)
            {
                this.Serialize(ibisStream, ds004B);
                return;
            }

            var ds004C = telegram as DS004C;
            if (ds004C != null)
            {
                this.Serialize(ibisStream, ds004C);
                return;
            }

            var ds005 = telegram as DS005;
            if (ds005 != null)
            {
                this.Serialize(ibisStream, ds005);
                return;
            }

            var ds006 = telegram as DS006;
            if (ds006 != null)
            {
                this.Serialize(ibisStream, ds006);
                return;
            }

            var ds009 = telegram as DS009;
            if (ds009 != null)
            {
                this.Serialize(ibisStream, ds009);
                return;
            }

            var ds010J = telegram as DS010J;
            if (ds010J != null)
            {
                this.Serialize(ibisStream, ds010J);
                return;
            }

            var ds020 = telegram as DS020;
            if (ds020 != null)
            {
                this.Serialize(ibisStream, ds020);
                return;
            }

            var ds021C = telegram as DS021C;
            if (ds021C != null)
            {
                this.Serialize(ibisStream, ds021C);
                return;
            }

            var ds036 = telegram as DS036;
            if (ds036 != null)
            {
                this.Serialize(ibisStream, ds036);
                return;
            }

            var ds070 = telegram as DS070;
            if (ds070 != null)
            {
                this.Serialize(ibisStream, ds070);
                return;
            }

            var ds080 = telegram as DS080;
            if (ds080 != null)
            {
                this.Serialize(ibisStream, ds080);
                return;
            }

            var ds081 = telegram as DS081;
            if (ds081 != null)
            {
                this.Serialize(ibisStream, ds081);
                return;
            }

            var ds084 = telegram as DS084;
            if (ds084 != null)
            {
                this.Serialize(ibisStream, ds084);
                return;
            }

            throw new NotSupportedException("Unsupported telegram: " + telegram.GetType().Name);
        }

        /// <summary>
        /// Deserializes a telegram from the given stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="Telegram"/>.
        /// </returns>
        public Telegram Deserialize(Stream stream)
        {
            var ibisStream = new IbisStream(stream);

            int read;
            while ((read = ibisStream.ReadByte()) >= 0)
            {
                try
                {
                    switch (read)
                    {
                        case 'l':
                            return this.DeserializeDS001(ibisStream);
                        case 'a':
                            return this.DeserializeDS120(ibisStream);
                        case 'g':
                            return this.DeserializeDS170(ibisStream);
                        case 'b':
                            if ((read = ibisStream.ReadByte()) == 'S')
                            {
                                return this.DeserializeDS184(ibisStream);
                            }

                            throw new NotSupportedException(
                                string.Format("Unexpected character after type 'b': {0:X2}", read));
                        default:
                            while ((read = ibisStream.ReadByte()) >= 0 && read != CarriageReturn)
                            {
                            }

                            // skip over checksum
                            ibisStream.ReadByte();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.DebugException("Coudln't decode telegram", ex);
                }

                ibisStream.ResetChecksum();
            }

            return null;
        }

        private static void WriteHeader(IbisStream stream, string identifier)
        {
            foreach (var c in identifier)
            {
                stream.WriteAsciiChar(c);
            }
        }

        private static void WriteFooter(IbisStream stream)
        {
            stream.WriteByte(CarriageReturn);
            stream.WriteByte((byte)(stream.Checksum & 0x7F));
        }

        private static void ReadFooter(IbisStream stream)
        {
            var read = stream.ReadByte();
            if (read != CarriageReturn)
            {
                throw new NotSupportedException(string.Format("Expected <CR> but got {0:X2}", read));
            }

            var checksum = stream.Checksum;
            read = stream.ReadByte();
            if (checksum != read)
            {
                throw new NotSupportedException(
                    string.Format("Expected checksum {0:X2} but got {1:X2}", checksum, read));
            }
        }

        private static int GetLawoString(string value, int modulo, byte[] buffer)
        {
            var index = 0;
            foreach (var c in value)
            {
                if (index == buffer.Length)
                {
                    break;
                }

                if (c >= 0xC0)
                {
                    if (index + 1 == buffer.Length)
                    {
                        break;
                    }

                    buffer[index++] = 2;
                    buffer[index] = (byte)(c - 0xA0);
                }
                else if (c >= 0x80)
                {
                    if (index + 1 == buffer.Length)
                    {
                        break;
                    }

                    buffer[index++] = 1;
                    buffer[index] = (byte)(c - 0x60);
                }
                else
                {
                    buffer[index] = (byte)c;
                }

                index++;
            }

            while (index < buffer.Length && (modulo == 0 || index % modulo != 0))
            {
                buffer[index++] = (byte)' ';
            }

            return index;
        }

        private void Serialize(IbisStream stream, DS001 telegram)
        {
            var payload = Encoding.ASCII.GetBytes(telegram.LineNumber);
            if (payload.Length != 3)
            {
                throw new NotSupportedException("Can't serialize DS001: it needs to be a 3 digit line number");
            }

            WriteHeader(stream, "l");
            stream.Write(payload, 0, payload.Length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS002 telegram)
        {
            if (telegram.RunNumber > 99 || telegram.RunNumber < 0)
            {
                throw new NotSupportedException(
                    "Can't serialize DS002: run number must be between 0..99: " + telegram.RunNumber);
            }

            WriteHeader(stream, "k");
            stream.WriteDigits(telegram.RunNumber, 2);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS003 telegram, DS003Config config)
        {
            WriteHeader(stream, "z");
            stream.WriteDigits(telegram.DestinationNumber, config.DestinationSize);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS003C telegram, DS003CConfig config)
        {
            WriteHeader(stream, "zI");
            var payload = new byte[config.MaxTextLength];
            var length = GetLawoString(telegram.StopName, 4, payload);
            stream.WriteHexValue(length / 4);
            stream.Write(payload, 0, length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS004 telegram, DS004Config config)
        {
            if (telegram.Characteristics >= Math.Pow(10, config.DigitCount) || telegram.Characteristics < 0)
            {
                throw new NotSupportedException(
                    string.Format(
                        "Can't serialize DS004: characteristics can have a maximum {0} digits: {1}",
                        config.DigitCount,
                        telegram.Characteristics));
            }

            WriteHeader(stream, "e");
            stream.WriteDigits(telegram.Characteristics, config.DigitCount);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS004A telegram)
        {
            if (telegram.Characteristics > 9999 || telegram.Characteristics < 0)
            {
                throw new NotSupportedException(
                    "Can't serialize DS004a: characteristics must be between 0..9999: " + telegram.Characteristics);
            }

            WriteHeader(stream, "eA");
            stream.WriteDigits(telegram.Characteristics, 4);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS004B telegram)
        {
            if (telegram.StopId > 9999999 || telegram.StopId < 0)
            {
                throw new NotSupportedException(
                    "Can't serialize DS004b: stop ID must be between 0..9999999: " + telegram.StopId);
            }

            WriteHeader(stream, "eH");
            stream.WriteDigits(telegram.StopId, 7);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS004C telegram)
        {
            var payload = new byte[16];
            var length = GetLawoString(telegram.StopName, 0, payload);

            WriteHeader(stream, "eT");
            stream.WriteHexValue(length / 4);
            stream.Write(payload, 0, length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS005 telegram)
        {
            var payload = Encoding.ASCII.GetBytes(telegram.Time);

            WriteHeader(stream, "u");
            stream.Write(payload, 0, payload.Length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS006 telegram)
        {
            var payload = Encoding.ASCII.GetBytes(telegram.Date);

            WriteHeader(stream, "d");
            stream.Write(payload, 0, payload.Length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS009 telegram)
        {
            var payload = new byte[16];
            var length = GetLawoString(telegram.StopName, 0, payload);

            WriteHeader(stream, "v");
            stream.Write(payload, 0, length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS010J telegram)
        {
            WriteHeader(stream, "x");
            stream.WriteDigits(telegram.Status, 1);
            stream.WriteDigits(telegram.StopIndex, 3);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS020 telegram)
        {
            WriteHeader(stream, "a");
            stream.WriteHexValue(telegram.IbisAddress);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS021C telegram)
        {
            if (telegram.StopData == null || telegram.StopData.Length < 3)
            {
                throw new ArgumentException("Stop data must be at least 3 parts");
            }

            var text = new StringBuilder();
            text.Append(telegram.StopData[0]);
            text.Append('\x03');
            text.Append(telegram.StopData[1]);
            text.Append('\x04');
            for (var i = 2; i < telegram.StopData.Length; i++)
            {
                text.Append(telegram.StopData[i]);
            }

            var payload = new byte[1024];
            var length = GetLawoString(text.ToString(), 1, payload);

            WriteHeader(stream, "aX");
            stream.WriteHexValue(telegram.IbisAddress);
            stream.Write(payload, 0, length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS036 telegram)
        {
            var payload = Encoding.ASCII.GetBytes(telegram.AnnouncementIndex);
            if (payload.Length != 4)
            {
                throw new NotSupportedException("Can't serialize DS036: it needs to be a 4 digit announcement number");
            }

            WriteHeader(stream, "hP");
            stream.Write(payload, 0, payload.Length);
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS070 telegram)
        {
            WriteHeader(stream, "g");
            stream.WriteHexValue(telegram.IbisAddress);
            WriteFooter(stream);
        }

        // ReSharper disable once UnusedParameter.Local
        private void Serialize(IbisStream stream, DS080 telegram)
        {
            WriteHeader(stream, "bT");
            WriteFooter(stream);
        }

        // ReSharper disable once UnusedParameter.Local
        private void Serialize(IbisStream stream, DS081 telegram)
        {
            WriteHeader(stream, "bM");
            WriteFooter(stream);
        }

        private void Serialize(IbisStream stream, DS084 telegram)
        {
            WriteHeader(stream, "bS");
            stream.WriteHexValue(telegram.IbisAddress);
            WriteFooter(stream);
        }

        private Telegram DeserializeDS001(IbisStream stream)
        {
            var payload = string.Empty;
            for (var i = 0; i < 3; i++)
            {
                var b = (byte)stream.ReadByte();
                if (b < '0' || b > '9')
                {
                    throw new NotSupportedException(string.Format("Bad status code: {0:X1}", b));
                }

                payload += b.ToString();
            }

            ReadFooter(stream);
            return new DS001 { LineNumber = payload };
        }

        private Telegram DeserializeDS120(IbisStream stream)
        {
            var status = stream.ReadByte();
            ReadFooter(stream);
            if (status < '0' || status > '9')
            {
                throw new NotSupportedException(string.Format("Bad status code: {0:X2}", status));
            }

            return new DS120 { Status = status - '0' };
        }

        private Telegram DeserializeDS170(IbisStream stream)
        {
            var status = stream.ReadByte();
            ReadFooter(stream);
            if (status < '0' || status > '9')
            {
                throw new NotSupportedException(string.Format("Bad status code: {0:X2}", status));
            }

            return new DS170 { Status = status - '0' };
        }

        private Telegram DeserializeDS184(IbisStream stream)
        {
            var status = stream.ReadByte();
            ReadFooter(stream);
            if (status < '0' || status > '9')
            {
                throw new NotSupportedException(string.Format("Bad status code: {0:X2}", status));
            }

            return new DS184 { Status = status - '0' };
        }
    }
}
