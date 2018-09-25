// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageMappingConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LanguageMappingConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.VDV301
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The language mapping configuration.
    /// </summary>
    [Serializable]
    public class LanguageMappingConfig
    {
        /// <summary>
        /// Gets or sets the VDV 301 source language string.
        /// </summary>
        [XmlAttribute("VDV301")]
        public string Vdv301Input { get; set; }

        /// <summary>
        /// Gets or sets the Ximple output language string (according to the dictionary).
        /// </summary>
        [XmlAttribute("Ximple")]
        public string XimpleOutput { get; set; }
    }
}