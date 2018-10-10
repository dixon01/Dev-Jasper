// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediResourceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediResourceProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi
{
    using System;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// <see cref="IResourceProvider"/> that uses a <see cref="IResourceService"/> to get resources.
    /// </summary>
    public class MediResourceProvider : IResourceProvider
    {
        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediResourceProvider"/> class.
        /// </summary>
        /// <param name="resourceService">
        /// The resource service. Usually this comes from
        /// <code>MessageDispatcher.Instance.GetService&lt;IResourceService&gt;()</code>.
        /// </param>
        public MediResourceProvider(IResourceService resourceService)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException("resourceService");
            }

            this.resourceService = resourceService;
        }

        /// <summary>
        /// Gets a resource for a given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the resource couldn't be found or is otherwise invalid.
        /// </exception>
        public IResource GetResource(string hash)
        {
            try
            {
                return new MediResource(this.resourceService.GetResource(new ResourceId(hash)), this.resourceService);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Couldn't get resource " + hash, ex);
            }
        }

        /// <summary>
        /// Adds a resource to the provider.
        /// </summary>
        /// <param name="hash">
        /// The expected hash of the resource file (the name from where it was copied).
        /// </param>
        /// <param name="resourceFile">
        /// The full resource file path.
        /// </param>
        /// <param name="deleteFile">
        /// A flag indicating whether the <see cref="resourceFile"/> should be deleted after being registered.
        /// </param>
        /// <exception cref="UpdateException">
        /// If the resource file doesn't match the given hash.
        /// </exception>
        public void AddResource(string hash, string resourceFile, bool deleteFile)
        {
            ResourceId id;
            try
            {
                id = this.resourceService.RegisterResource(resourceFile, deleteFile);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Couldn't add resource " + hash, ex);
            }

            if (!id.Hash.Equals(hash, StringComparison.InvariantCultureIgnoreCase))
            {
                this.resourceService.RemoveResource(id);
                throw new UpdateException(string.Format("Expected hash {0} but calculated {1}", hash, id.Hash));
            }
        }
    }
}
