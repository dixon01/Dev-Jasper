// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionConfigDataModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Presentation.Section
{
    using System.Xml.Serialization;

    /// <summary>
    /// The section config data model base.
    /// </summary>
    public partial class SectionConfigDataModelBase
    {
        /// <summary>
        /// Gets or sets the custom name of a section.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}
