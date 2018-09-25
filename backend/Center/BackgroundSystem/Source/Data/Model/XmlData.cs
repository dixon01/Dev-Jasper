// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlData.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Model
{
    /// <summary>
    /// The XML data from the corresponding table.
    /// This data is used always together with the entity owning it.
    /// </summary>
    public class XmlData
    {
        /// <summary>
        /// Gets or sets the unique identifier of this entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the XML data string.
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Gets or sets the qualified type name (without version) of the serialized XML data.
        /// </summary>
        public string Type { get; set; }
    }
}
