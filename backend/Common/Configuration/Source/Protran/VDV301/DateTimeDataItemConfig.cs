// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeDataItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeDataItemConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.VDV301
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Special configuration for date/time data item that allows for the formatting of the contents.
    /// </summary>
    [Serializable]
    public class DateTimeDataItemConfig : DataItemConfig
    {
        /// <summary>
        /// Gets or sets the date time format.
        /// <seealso cref="DateTime.ToString(string)"/>
        /// </summary>
        [XmlAttribute("DateTimeFormat")]
        public string DateTimeFormat { get; set; }
    }
}