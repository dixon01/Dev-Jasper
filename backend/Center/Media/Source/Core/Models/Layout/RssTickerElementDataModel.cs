// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssTickerElementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// The RSS ticker element data model.
    /// </summary>
    [XmlRoot("RssTicker")]
    [Serializable]
    public class RssTickerElementDataModel : DrawableElementDataModelBase
    {
        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [XmlAttribute("TestData")]
        public string TestData { get; set; }

        /// <summary>
        /// Gets or sets the align.
        /// </summary>
        [XmlAttribute("Align")]
        public HorizontalAlignment Align
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the v align.
        /// </summary>
        [XmlAttribute("VAlign")]
        [DefaultValue(VerticalAlignment.Top)]
        public VerticalAlignment VAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scroll speed.
        /// </summary>
        [XmlAttribute("ScrollSpeed")]
        [DefaultValue(0)]
        public int ScrollSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        [XmlElement("Font")]
        public FontDataModel Font
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the RSS url.
        /// </summary>
        [XmlAttribute("RssUrl")]
        public string RssUrl { get; set; }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        [XmlAttribute("Delimiter")]
        public string Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the update interval.
        /// </summary>
        [XmlIgnore]
        public TimeSpan UpdateInterval { get; set; }

        /// <summary>
        /// Gets or sets the update interval xml.
        /// </summary>
        [XmlAttribute("UpdateInterval")]
        public string UpdateIntervalXml
        {
            get
            {
                return XmlConvert.ToString(this.UpdateInterval);
            }

            set
            {
                this.UpdateInterval = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the validity.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Validity { get; set; }

        /// <summary>
        /// Gets or sets the validity xml.
        /// </summary>
        [XmlAttribute("Validity")]
        public string ValidityXml
        {
            get
            {
                return XmlConvert.ToString(this.Validity);
            }

            set
            {
                this.Validity = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the exporting row.
        /// </summary>
        [XmlAttribute("ExportingRow")]
        public int ExportingRow { get; set; }
    }
}
