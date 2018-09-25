// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConversionResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConversionResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper
{
    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Defines the result of a conversion to the e-paper format.
    /// </summary>
    public class ConversionResult
    {
        /// <summary>
        /// Gets or sets the EPD (e-paper binary format) resource.
        /// </summary>
        public ContentResource EpdResource { get; set; }
    }
}