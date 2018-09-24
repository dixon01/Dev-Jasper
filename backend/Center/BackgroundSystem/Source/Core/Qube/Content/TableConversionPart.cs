// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableConversionPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TableConversionPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Content
{
    /// <summary>
    /// Defines a part of the table conversion.
    /// </summary>
    public class TableConversionPart
    {
        /// <summary>
        /// Gets or sets the input of the conversion.
        /// </summary>
        public StreamContentResourceWrapper Input { get; set; }

        /// <summary>
        /// Gets or sets the output of the conversion to black and white.
        /// </summary>
        public StreamContentResourceWrapper BlackWhite { get; set; }

        /// <summary>
        /// Gets or sets the resulting resource in e-paper format.
        /// </summary>
        public StreamContentResourceWrapper Epd { get; set; }
    }
}