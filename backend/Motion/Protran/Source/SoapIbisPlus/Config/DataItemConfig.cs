// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Container of all basic information about SOAP data
    /// </summary>
    [Serializable]
    public class DataItemConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemConfig"/> class.
        /// </summary>
        public DataItemConfig()
        {
            this.Enabled = false;
            this.TransfRef = string.Empty;
        }

        /// <summary>
        /// Gets or sets the XML attribute called from.
        /// </summary>
        [XmlAttribute(AttributeName = "TransfRef")]
        public string TransfRef { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the SOAP data type is to
        /// be handled or not.
        /// </summary>
        [XmlAttribute(AttributeName = "Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the usage of the SOAP data types.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedFor { get; set; }
    }
}
