// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEPaperConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IEPaperConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper
{
    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Defines a component used to convert images to the e-paper format.
    /// </summary>
    public interface IEPaperConverter
    {
        /// <summary>
        /// Converts the given resource to the e-paper format.
        /// </summary>
        /// <param name="originalResource">
        /// The original resource to convert.
        /// </param>
        /// <returns>
        /// The <see cref="ConversionResult"/>.
        /// </returns>
        ConversionResult Convert(ContentResource originalResource);
    }
}