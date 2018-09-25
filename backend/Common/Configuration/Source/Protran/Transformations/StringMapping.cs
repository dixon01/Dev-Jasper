// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringMapping.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the settings for a single transformation.
    /// </summary>
    [Serializable]
    public class StringMapping : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringMapping"/> class.
        /// </summary>
        public StringMapping()
        {
            this.Mappings = new List<Mapping>();
        }

        /// <summary>
        /// Gets or sets the set of all the mappings.
        /// </summary>
        [XmlElement("Mapping")]
        public List<Mapping> Mappings { get; set; }
    }
}
