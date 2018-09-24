// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Special parser for the GO002 telegram: used for the connections information.
    /// </summary>
    public class GO002Parser : SimpleHeaderTelegramParser<GO002>
    {
        private const string TelegramHeader = "aU";
        private const int FieldDataLengthBytes = 3;
        private const int FieldDepartureTimeBytes = 4;

        private static readonly Logger Logger = LogHelper.GetLogger<GO002Parser>();

        private GO002Config configGo002;
        private DS120Factory ds120Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO002Parser"/> class.
        /// </summary>
        public GO002Parser()
            : base(TelegramHeader, new Digit(FieldDataLengthBytes))
        {
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header.
        /// </summary>
        public override int HeaderSize
        {
            get
            {
                return base.HeaderSize + FieldDataLengthBytes;
            }
        }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="byteInfo">
        /// The byte information.
        /// </param>
        /// <param name="config">
        /// The config of the telegram.
        /// </param>
        public override void Configure(ByteInfo byteInfo, TelegramConfig config)
        {
            var ds120Config = config.Answer.Telegram as DS120Config;
            if (ds120Config != null && ds120Config.Enabled)
            {
                this.ds120Factory = new DS120Factory(ds120Config, byteInfo);
            }

            this.configGo002 = (GO002Config)config;
            base.Configure(byteInfo, config);
        }

        /// <summary>
        /// Checks whether the given telegram needs an answer to be sent.
        /// </summary>
        /// <param name="telegram">The telegram including header, marker and checksum.</param>
        /// <returns>True if the telegram requires an answer, else false.</returns>
        public override bool RequiresAnswer(byte[] telegram)
        {
            // the GO002, by its specification requires always an answer
            // so, here below I check only if the incoming telegram is only
            // suitable for this kind of parser and the user has configured it rightly.
            return this.ds120Factory != null;
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">The telegram including header, marker and checksum.</param>
        /// <param name="status">The status with which make the answer.</param>
        /// <returns>This method always returns null.</returns>
        public override byte[] CreateAnswer(byte[] telegram, Status status)
        {
            return this.ds120Factory.CreateAnswer(telegram, status);
        }

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// </summary>
        /// <param name="data">The telegram including header, marker and checksum.</param>
        /// <returns>The telegram object containing the payload of the given telegram.</returns>
        protected override GO002 Parse(byte[] data)
        {
            var telegram = base.Parse(data);

            // important: use the base header size since we are now parsing our part of the header
            var offset = base.HeaderSize;

            try
            {
                // 1) let's study the field "DataLength" (it's made by 3 bytes, only digits).
                telegram.DataLength = this.ExtractIntField(data, ref offset, FieldDataLengthBytes);
                if (offset + 2 == data.Length)
                {
                    // we have a telegram only with the data length (which should be 000)
                    return telegram;
                }

                // 2) let's study the field "StopIndex"
                telegram.StopIndex = this.ExtractIntField(data, ref offset, this.configGo002.StopIndexSize);
                if (offset + 2 == data.Length)
                {
                    // we have a telegram only with the data length and stop index
                    telegram.Payload = new byte[0];
                    return telegram;
                }

                // 3) let's study the field "RowNumber"
                telegram.RowNumber = this.ExtractIntField(data, ref offset, this.configGo002.RowNumberSize);

                // 4) let's study the field "Pictogram"
                telegram.Pictogram = this.ExtractStringField(data, ref offset, this.configGo002.PictogramSize);

                // 5) let's study the field "LineNumber"
                telegram.LineNumber = this.ExtractStringField(data, ref offset, this.configGo002.LineNumberSize);

                // 6) let's study the field "DepartureTime" (it's made by 4 bytes, only digits with the format HHMM).
                telegram.DepartureTime = this.ExtractStringField(data, ref offset, FieldDepartureTimeBytes);

                // 7) let's study the field "TrackNumber"
                telegram.TrackNumber = this.ExtractStringField(data, ref offset, this.configGo002.TrackNumberSize);

                // 8) let's study the field "Deviation"
                telegram.Deviation = this.ExtractStringField(data, ref offset, this.configGo002.ScheduleDeviationSize);

                // remove everything including "Deviation"
                var payload = new byte[data.Length - offset - 2];
                Array.Copy(data, offset, payload, 0, payload.Length);
                telegram.Payload = payload;
            }
            catch (Exception ex)
            {
                // invalid field.
                Logger.Warn(ex, "GO002 with wrong field content at offset " + offset, ex);
                return null;
            }

            // "DataLength" refers to the whole GO002 but without the header (Length = 2),
            // datalength(Length = 3), carriage return and the check sum.
            int calculatedLength = (data.Length - 5) - (this.ByteInfo.ByteSize * 2);
            if (this.configGo002.CheckLength && calculatedLength != telegram.DataLength)
            {
                Logger.Info(
                    "GO002 with wrong length (calculated:{0}, declared:{1})",
                    calculatedLength,
                    telegram.DataLength);
                return null;
            }

            telegram.LineNumber = telegram.LineNumber.TrimStart(' ');
            telegram.TrackNumber = telegram.TrackNumber.TrimStart(' ');

            return this.ComposeTelegram(telegram);
        }

        private int ExtractIntField(byte[] buffer, ref int offset, int charCount)
        {
            return int.Parse(this.ExtractStringField(buffer, ref offset, charCount));
        }

        private string ExtractStringField(byte[] buffer, ref int offset, int charCount)
        {
            int count = charCount * this.ByteInfo.ByteSize;
            var value = this.ByteInfo.Encoding.GetString(buffer, offset, count);
            offset += count;
            return value;
        }

        private GO002 ComposeTelegram(GO002 telegram)
        {
            int deviation = int.Parse(telegram.Deviation, NumberStyles.AllowLeadingSign);
            int absDeviation = Math.Abs(deviation);
            telegram.Deviation = deviation == 0
                                     ? this.configGo002.ScheduleDeviation.OnTime
                                     : string.Format(
                                         deviation > 0
                                             ? this.configGo002.ScheduleDeviation.Delayed
                                             : this.configGo002.ScheduleDeviation.Ahead,
                                         absDeviation);

            telegram.Pictogram = string.Format(this.configGo002.PictogramFormat, telegram.Pictogram);

            telegram.LineNumber = string.Format(this.configGo002.LineNumberFormat, telegram.LineNumber);

            telegram.DepartureTime = string.Format(
                "{0}:{1}",
                telegram.DepartureTime.Substring(0, 2),
                telegram.DepartureTime.Substring(2, 2));

            return telegram;
        }
    }
}
