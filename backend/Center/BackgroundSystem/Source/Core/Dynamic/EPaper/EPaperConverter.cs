// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPaperConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper
{
    using System;
    using System.IO;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Converts an input resource to the EPaper format.
    /// </summary>
    public class EPaperConverter : IEPaperConverter
    {
        private readonly string tempDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EPaperConverter"/> class.
        /// </summary>
        public EPaperConverter()
        {
            this.tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Converts the original input into the EPaper format needed by the unit.
        /// It also creates and stores all intermediate resources.
        /// </summary>
        /// <param name="originalResource">
        /// The original resource.
        /// </param>
        /// <returns>
        /// The <see cref="ConversionResult"/> containing the final EPaper file and also all intermediate resources
        /// created during the process.
        /// </returns>
        public ConversionResult Convert(ContentResource originalResource)
        {
            var resourceService = DependencyResolver.Current.Get<IContentResourceService>();
            var convertedFilePath = this.GetTempFile(Guid.NewGuid() + ".tmp");
            var conversionResult = new ConversionResult { EpdResource = originalResource };

            // TODO: use AddResourceAsync when available
            resourceService.AddResource(
                conversionResult.EpdResource.Hash,
                conversionResult.EpdResource.HashAlgorithmType,
                convertedFilePath,
                true);
            return conversionResult;
        }

        private string GetTempFile(string fileName)
        {
            return Path.Combine(this.tempDirectory, fileName);
        }
    }
}