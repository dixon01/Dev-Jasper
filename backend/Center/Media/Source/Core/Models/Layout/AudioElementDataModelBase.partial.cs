// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioElementDataModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio element data model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System.Xml.Serialization;

    /// <summary>
    /// The audio element data model base.
    /// </summary>
    public partial class AudioElementDataModelBase
    {
        /// <summary>
        /// Gets or sets the list index.
        /// </summary>
        [XmlElement("Index")]
        public int ListIndex { get; set; }
    }
}
