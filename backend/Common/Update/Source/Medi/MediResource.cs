// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediResource.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi
{
    using System.IO;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// <see cref="IResource"/> implementation that uses an <see cref="IResourceService"/>.
    /// </summary>
    internal class MediResource : IResource
    {
        private readonly ResourceInfo resourceInfo;

        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediResource"/> class.
        /// </summary>
        /// <param name="resourceInfo">
        /// The resource info.
        /// </param>
        /// <param name="resourceService">
        /// The resource service.
        /// </param>
        public MediResource(ResourceInfo resourceInfo, IResourceService resourceService)
        {
            this.resourceInfo = resourceInfo;
            this.resourceService = resourceService;
        }

        /// <summary>
        /// Gets the unique MD5 hash of this resource.
        /// </summary>
        public string Hash
        {
            get
            {
                return this.resourceInfo.Id.Hash;
            }
        }

        /// <summary>
        /// Copies this resource to the given path.
        /// </summary>
        /// <param name="filePath">
        /// The full file path where to copy the resource to.
        /// </param>
        public void CopyTo(string filePath)
        {
            if (!this.resourceService.ExportResource(this.resourceInfo, filePath))
            {
                throw new UpdateException("Couldn't export resource " + this.Hash);
            }
        }

        /// <summary>
        /// Opens this resource for reading.
        /// </summary>
        /// <returns>
        /// A stream that allows reading the resource.
        /// </returns>
        public Stream OpenRead()
        {
            var tempFile = Path.GetTempFileName();
            this.CopyTo(tempFile);
            return new TemporaryFileStream(tempFile);
        }
    }
}