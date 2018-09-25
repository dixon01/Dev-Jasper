// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all data models.
    /// </summary>
    [Serializable]
    public abstract class DataModelBase
    {
        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        [XmlAttribute("DisplayText")]
        public virtual string DisplayText { get; set; }
    }
}
