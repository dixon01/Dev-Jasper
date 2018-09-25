// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseNextStop.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParseNextStop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Parser for next stop
    /// </summary>
    public class ParseNextStop
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public byte[] Msg { get; set; }

        #region PUBLIC_FUNCTIONS
        /// <summary>
        /// COS: 24 November 2010
        /// Function refactoring.
        /// </summary>
        /// <returns>
        /// The parse next stop data.
        /// </returns>
        public NextStopSlide ParseNextStopData()
        {
            var nsd = new NextStopSlide();
            int pos;

            this.ParseRegion(ref nsd);
            this.ParsePunctuality(ref nsd);
            this.ParseStopId(ref nsd);
            this.ParseLineNo(ref nsd);
            var noOfNextStops = this.ParseNoOfNextStops();
            pos = this.ParsePrevCity(ref nsd);
            pos = this.ParsePrevStop(pos, ref nsd);
            pos = this.ParseDest(pos, ref nsd);
            for (int i = 0; i < noOfNextStops; i++)
            {
                pos = this.ParseNextN(pos, ref nsd);
            }

            return nsd;
        }

        /// <summary>
        /// Gets an instance of a SlideShowMessage object parsing the information
        /// stored inside the message's buffer previously set.
        /// </summary>
        /// <returns>An instance of a SlideShowMessage object, or null in case of error.</returns>
        public SlideShowMessage ParseSlideShowMsg()
        {
            if (this.Msg == null || this.Msg.Length <= 0)
            {
                // I cannot get any SlideShowMessage
                return null;
            }

            var slideShowMsg = new SlideShowMessage();
            var parse = slideShowMsg.Parse(this.Msg);
            if (!parse)
            {
                // parsing phase failed.
                return null;
            }

            return slideShowMsg;
        }

        /// <summary>
        /// Gets an instance of an AdHocMessage object parsing the information
        /// stored inside the message's buffer previously set.
        /// </summary>
        /// <returns>An instance of an AdHocMessage object, or null in case of error.</returns>
        public AdHocMessage ParseAdHocMsg()
        {
            if (this.Msg == null || this.Msg.Length <= 0)
            {
                // I cannot get any AdHocMessage
                return null;
            }

            var message = new AdHocMessage();
            bool parse = message.Parse(this.Msg);
            if (!parse)
            {
                // parsing phase failed.
                return null;
            }

            return message;
        }

        /// <summary>
        /// Gets an instance of an ConnectionsMessage object parsing the information
        /// stored inside the message's buffer previously set.
        /// </summary>
        /// <returns>An instance of an ConnectionsMessage object, or null in case of error.</returns>
        public ConnectionsMessage ParseConnectionsMsg()
        {
            if (this.Msg == null || this.Msg.Length <= 0)
            {
                // I cannot get any ConnectionsMessage
                return null;
            }

            var connectionsMessage = new ConnectionsMessage();
            bool parse = connectionsMessage.Parse(this.Msg);
            if (!parse)
            {
                // parsing phase failed.
                return null;
            }

            return connectionsMessage;
        }

        /// <summary>
        /// Gets an instance of an LineInfoMessage object parsing the information
        /// stored inside the message's buffer previously set.
        /// </summary>
        /// <returns>An instance of an LineInfoMessage object, or null in case of error.</returns>
        public LineInfoMessage ParseLineInfoMsg()
        {
            if (this.Msg == null || this.Msg.Length <= 0)
            {
                // I cannot get any LineInfoMessage
                return null;
            }

            var lineInfoMessage = new LineInfoMessage();
            bool parse = lineInfoMessage.Parse(this.Msg);
            if (!parse)
            {
                // parsing phase failed.
                return null;
            }

            return lineInfoMessage;
        }

        /// <summary>
        /// Gets an instance of an WifiStatusMessage object parsing the information
        /// stored inside the message's buffer previously set.
        /// </summary>
        /// <returns>An instance of an WifiStatusMessage object, or null in case of error.</returns>
        public WifiStatusMessage ParseWifiStatusMsg()
        {
            if (this.Msg == null || this.Msg.Length <= 0)
            {
                // I cannot get any WifiStatusMessage
                return null;
            }

            var wifiStatusMessage = new WifiStatusMessage();
            bool parse = wifiStatusMessage.Parse(this.Msg);
            if (!parse)
            {
                // parsing phase failed.
                return null;
            }

            return wifiStatusMessage;
        }
        #endregion PUBLIC_FUNCTIONS

        #region PRIVATE_FUNCTIONS
        private int MakeInt16(byte a, byte b)
        {
            int value = a << 8;
            value += b;
            return value;
        }

        private void ParseRegion(ref NextStopSlide nsd)
        {
            if (this.Msg == null || this.Msg.Length < 34 || nsd == null)
            {
                // invalid parameter.
                return;
            }

            var region = (ushort)this.MakeInt16(this.Msg[32], this.Msg[33]);
            nsd.Region = region.ToString(CultureInfo.InvariantCulture);
        }

        private void ParsePunctuality(ref NextStopSlide nsd)
        {
            if (this.Msg == null || this.Msg.Length < 44 || nsd == null)
            {
                // invalid parameter.
                return;
            }

            var punctuality = (uint)((this.Msg[40] << 24) |
                                      (this.Msg[41] << 16) | (this.Msg[42] << 8) |
                                       this.Msg[43]);
            nsd.Punctuality = punctuality.ToString(CultureInfo.InvariantCulture);
        }

        private void ParseStopId(ref NextStopSlide nsd)
        {
            if (this.Msg == null || this.Msg.Length < 40 || nsd == null)
            {
                // invalid parameter.
                return;
            }

            var stopId = (uint)((this.Msg[36] << 24) |
                                 (this.Msg[37] << 16) |
                                 (this.Msg[38] << 8) |
                                  this.Msg[39]);
            nsd.StopId = stopId.ToString(CultureInfo.InvariantCulture);
        }

        private void ParseLineNo(ref NextStopSlide nsd)
        {
            if (this.Msg == null || this.Msg.Length < 36)
            {
                // invalid message.
                // I cannot parse nothing.
                return;
            }

            int lineNo = this.MakeInt16(this.Msg[34], this.Msg[35]);
            nsd.LineNo = lineNo.ToString(CultureInfo.InvariantCulture);
        }

        private int ParseNoOfNextStops()
        {
            if (this.Msg == null || this.Msg.Length < 46)
            {
                // invalid message.
                // I cannot parse nothing.
                return 0x00;
            }

            return this.MakeInt16(this.Msg[44], this.Msg[45]);
        }

        private int ParsePrevCity(ref NextStopSlide nsd)
        {
            int prevCityLen = this.MakeInt16(this.Msg[46], this.Msg[47]);
            Encoding be = new UnicodeEncoding(true, true);
            nsd.PrevCity = be.GetString(this.Msg, 48, prevCityLen * 2);
            return 48 + (prevCityLen * 2);
        }

        private int ParsePrevStop(int pos, ref NextStopSlide nsd)
        {
            int prevStopLen = this.MakeInt16(this.Msg[pos], this.Msg[pos + 1]);
            Encoding be = new UnicodeEncoding(true, true);
            nsd.PrevStopName = be.GetString(this.Msg, pos + 2, prevStopLen * 2);
            return pos + 2 + (prevStopLen * 2);
        }

        private int ParseDest(int pos, ref NextStopSlide nsd)
        {
            int destLen = this.MakeInt16(this.Msg[pos], this.Msg[pos + 1]);
            Encoding be = new UnicodeEncoding(true, true);
            string work = be.GetString(this.Msg, pos + 2, destLen * 2);

            var item = this.SeparateStationFields(work);
            nsd.DestCity = item.City;
            nsd.DestStopName = item.StopName;
            nsd.DestTime = item.Time;

            return pos + 2 + (destLen * 2);
        }

        private int ParseNextN(int pos, ref NextStopSlide nsd)
        {
            int nextLen = this.MakeInt16(this.Msg[pos], this.Msg[pos + 1]);
            Encoding be = new UnicodeEncoding(true, true);
            string work = be.GetString(this.Msg, pos + 2, nextLen * 2);

            var item = this.SeparateStationFields(work);

            nsd.NextNList.Add(item);

            return pos + 2 + (nextLen * 2);
        }

        private NextStopListItem SeparateStationFields(string work)
        {
            var result = new NextStopListItem();
            int pos1 = work.IndexOf(";", StringComparison.Ordinal);
            int pos2 = work.IndexOf(";", pos1 + 1, StringComparison.Ordinal);

            try
            {
                result.City = work.Substring(0, pos1);
                result.StopName = work.Substring(pos1 + 1, pos2 - pos1 - 1);
                result.Time = work.Substring(pos2 + 1, work.Length - pos2 - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                result.City = string.Empty;
                result.StopName = string.Empty;
                result.Time = string.Empty;
            }

            return result;
        }
        #endregion PRIVATE_FUNCTIONS
    }
}
