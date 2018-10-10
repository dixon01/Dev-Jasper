// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Replace.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Replace type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Transformation that only replaces whole strings.
    /// </summary>
    [Serializable]
    public class Replace : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Replace"/> class.
        /// </summary>
        public Replace()
        {
            this.Mappings = new List<Mapping>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the replacement is case sensitive.
        /// Default value is false.
        /// </summary>
        [DefaultValue(false)]
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets the set of all the mappings.
        /// </summary>
        [XmlElement("Mapping")]
        public List<Mapping> Mappings { get; set; }
    }
}
