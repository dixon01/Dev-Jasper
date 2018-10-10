// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Join.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Container of all the settings for a single transformation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Transformation config for a Join transformation that converts a string array to a string
    /// by combining the elements with the defined <see cref="Separator"/>.
    /// </summary>
    [Serializable]
    public class Join : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        public Join()
        {
            this.Separator = string.Empty;
        }

        /// <summary>
        /// Gets or sets the separator string used to combine the elements.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute]
        public string Separator { get; set; }
    }
}
