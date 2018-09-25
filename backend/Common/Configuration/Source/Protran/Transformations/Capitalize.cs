// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Capitalize.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Capitalize type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the settings for a single transformation.
    /// </summary>
    [Serializable]
    public class Capitalize : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Capitalize"/> class.
        /// </summary>
        public Capitalize()
        {
            this.Mode = CapitalizeMode.UpperLower;
        }

        #region PROPERTIES

        /// <summary>
        /// Gets or sets the capitalization mode of this transformation.
        /// Default value is <see cref="CapitalizeMode.UpperLower"/>.
        /// </summary>
        [XmlAttribute("Mode")]
        [DefaultValue(CapitalizeMode.UpperLower)]
        public CapitalizeMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Exceptions.
        /// </summary>
        public string[] Exceptions { get; set; }
        #endregion PROPERTIES
    }
}
