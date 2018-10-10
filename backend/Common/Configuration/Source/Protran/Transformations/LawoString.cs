// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LawoString.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OffsetEscape type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// LAWO string escape configuration.
    /// </summary>
    [Serializable]
    public class LawoString : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LawoString"/> class.
        /// </summary>
        public LawoString()
        {
            this.CodePage = 1252;
        }

        /// <summary>
        /// Gets or sets the code page to be used.
        /// Suggested values: 858 or 1252
        /// Default is 1252
        /// </summary>
        [DefaultValue(1252)]
        public int CodePage { get; set; }
    }
}
