// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.VDV301
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Configuration for a single leaf tag of the IBIS-IP data structure.
    /// </summary>
    [Serializable]
    public class DataItemConfig : GenericUsage
    {
        /// <summary>
        /// Gets or sets a value indicating whether the handling of this tag is enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the transformation reference.
        /// </summary>
        [XmlAttribute("TransfRef")]
        public string TransfRef { get; set; }
    }
}