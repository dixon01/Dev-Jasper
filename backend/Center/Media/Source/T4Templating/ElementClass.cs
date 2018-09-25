// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementClass.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines an element class.
    /// </summary>
    public class ElementClass : Class
    {
        /// <summary>
        /// Gets or sets the type of the child elements.
        /// </summary>
        /// <value>
        /// The type of the child elements.
        /// </value>
        [XmlAttribute]
        public string ChildElementsType { get; set; }

        /// <summary>
        /// Gets or sets the supported screen types.
        /// </summary>
        /// <value>
        /// The supported screen types.
        /// </value>
        [XmlAttribute]
        public string SupportedScreenTypes { get; set; }
    }
}