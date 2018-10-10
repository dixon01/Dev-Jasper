// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mapping.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the settings for a single transformation.
    /// </summary>
    [Serializable]
    public class Mapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        public Mapping()
        {
            this.From = string.Empty;
            this.To = string.Empty;
        }

        /// <summary>
        /// Gets or sets the XML attribute called from.
        /// </summary>
        [XmlAttribute(AttributeName = "from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the XML attribute called to.
        /// </summary>
        [XmlAttribute(AttributeName = "to")]
        public string To { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} -> {1}", this.From, this.To);
        }
    }
}
