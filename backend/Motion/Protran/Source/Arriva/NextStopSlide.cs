// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NextStopSlide.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NextStopSlide type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using NLog;

    /// <summary>
    /// Container for next stop slide message
    /// </summary>
    public class NextStopSlide
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="NextStopSlide"/> class.
        /// Default constructor.
        /// </summary>
        public NextStopSlide()
        {
            this.Region = string.Empty;
            this.Punctuality = string.Empty;
            this.StopId = string.Empty;

            this.LineNo = string.Empty;
            this.PrevCity = string.Empty;
            this.PrevStopName = string.Empty;
            this.DestCity = string.Empty;
            this.DestStopName = string.Empty;
            this.DestTime = string.Empty;
            this.TickerText = string.Empty;
            this.MessageId = "EvtGorbaNextStop";
            this.NextNList = new List<NextStopListItem>();
        }

        /// <summary>
        /// Gets or sets MessageId.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets NextNList.
        /// </summary>
        public List<NextStopListItem> NextNList { get; set; }

        /// <summary>
        /// Gets or sets Region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets Punctuality.
        /// </summary>
        public string Punctuality { get; set; }

        /// <summary>
        /// Gets or sets StopID.
        /// </summary>
        public string StopId { get; set; }

        /// <summary>
        /// Gets or sets LineNo.
        /// </summary>
        [XmlElement("LineNo")]
        public string LineNo { get; set; }

        /// <summary>
        /// Gets or sets Previous City.
        /// </summary>
        [XmlElement("PrevCity")]
        public string PrevCity { get; set; }

        /// <summary>
        /// Gets or sets Previous StopName.
        /// </summary>
        [XmlElement("PrevStopName")]
        public string PrevStopName { get; set; }

        /// <summary>
        /// Gets or sets Destination City.
        /// </summary>
        [XmlElement("DestCity")]
        public string DestCity { get; set; }

        /// <summary>
        /// Gets or sets Destination StopName.
        /// </summary>
        [XmlElement("DestStopName")]
        public string DestStopName { get; set; }

        /// <summary>
        /// Gets or sets Destination Time.
        /// </summary>
        [XmlElement("DestTime")]
        public string DestTime { get; set; }

        /// <summary>
        /// Gets or sets TickerText.
        /// </summary>
        [XmlElement("TickerText")]
        public string TickerText { get; set; }

        /// <summary>
        /// Clears the "Next Stop slide" represented by this object.
        /// </summary>
        public void ClearNsd()
        {
            this.NextNList.Clear();
        }

        /// <summary>
        /// Serialize this object into an XML string.
        /// </summary>
        /// <returns>The XML string as the result of the serialization.</returns>
        public string Serialize()
        {
            var memoryStream = new MemoryStream();
            var xs = new XmlSerializer(this.GetType());
            XmlTextWriter xmlTextWriter;
            try
            {
                xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Default);
                xs.Serialize(xmlTextWriter, this);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not serialize");
                return string.Empty;
            }

            xmlTextWriter.Flush();
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            string xmlizedString = Encoding.Default.GetString(memoryStream.ToArray());
            return xmlizedString;
        }
    }
}
