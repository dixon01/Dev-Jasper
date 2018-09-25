// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Class type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a class.
    /// </summary>
    public class Class
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the base.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        [XmlAttribute]
        public string Base { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        [XmlElement("Property", typeof(Property))]
        [XmlElement("CompositeProperty", typeof(CompositeProperty))]
        [XmlElement("ReferenceProperty", typeof(ReferenceProperty))]
        [XmlElement("ListProperty", typeof(ListProperty))]
        public List<PropertyBase> Properties { get; set; }
    }
}