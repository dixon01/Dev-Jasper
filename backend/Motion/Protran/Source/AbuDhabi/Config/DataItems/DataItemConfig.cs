// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config.DataItems
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Container of all the information
    /// concerning an ISI data item (Stop-1, Stop2, Stop3, TickerText...).
    /// </summary>
    [Serializable]
    public class DataItemConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemConfig"/> class.
        /// </summary>
        public DataItemConfig()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the item's name as used
        /// by the ISI server
        /// (example, ISI does use "Stop-1").
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this tag
        /// is enabled or not.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the transformation's name
        /// that has to be applied to the payload of
        /// this ISI data item.
        /// </summary>
        [XmlAttribute("TransfRef")]
        public string TransfRef { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedFor { get; set; }
    }
}
