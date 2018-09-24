// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextualReplacementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TextualReplacementDataModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    /// <summary>
    /// The TextualReplacementDataModel.
    /// </summary>
    public class TextualReplacementDataModel
    {
        /// <summary>
        /// Gets or sets the number.
        /// Has to have a unique value between 1 and 99
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the replacement uses an image
        /// </summary>
        public bool IsImageReplacement { get; set; }
    }
}