// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Property type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a property.
    /// </summary>
    public class Property : TypedProperty
    {
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        [XmlAttribute]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is dynamic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dynamic; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Dynamic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is animated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if animated; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Animated { get; set; }

        /// <summary>
        /// Gets or sets the supported screen types for the property.
        /// </summary>
        [XmlAttribute]
        public string SupportedScreenTypes { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        [XmlAttribute]
        public string Format { get; set; }

        /// <summary>
        /// Gets the escaped default value.
        /// </summary>
        /// <returns>The escaped default value.</returns>
        public string GetEscapedDefaultValue()
        {
            if (this.Type == "string")
            {
                return '"' + this.DefaultValue + '"';
            }

            return this.DefaultValue;
        }
    }
}