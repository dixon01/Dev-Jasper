// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexDivider.cs" company="Gorba AG">
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
    using System.Text.RegularExpressions;

    /// <summary>
    /// Container of all the settings for a single transformation.
    /// </summary>
    [Serializable]
    public class RegexDivider : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexDivider"/> class.
        /// </summary>
        public RegexDivider()
        {
            this.Options = RegexOptions.None;
        }

        /// <summary>
        /// Gets or sets the XML element called Regex.
        /// </summary>
        public string Regex { get; set; }

        /// <summary>
        /// Gets or sets the regular expression options.
        /// </summary>
        [DefaultValue(RegexOptions.None)]
        public RegexOptions Options { get; set; }
    }
}