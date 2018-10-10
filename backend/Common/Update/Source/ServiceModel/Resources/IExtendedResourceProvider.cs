// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendedResourceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IExtendedResourceProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Resources
{
    /// <summary>
    /// Extends the <see cref="IResourceProvider"/> with the possibility to delete a resource.
    /// </summary>
    public interface IExtendedResourceProvider : IResourceProvider
    {
        /// <summary>
        /// Deletes the resource with the given <paramref name="hash"/>, if found. If the resource doesn't exist,
        /// nothing will be done.
        /// </summary>
        /// <param name="hash">The hash of the resource to delete.</param>
        /// <exception cref="UpdateException">
        /// Any error occurred while removing the resource from the provider.
        /// </exception>
        void DeleteResource(string hash);

        /// <summary>
        /// Tries to get the resource with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// <c>true</c> if the resource was found and returned as the out parameter <paramref name="resource"/>;
        /// otherwise, <c>false</c> (and the out parameter doesn't have a meaningful value).
        /// </returns>
        bool TryGetResource(string hash, out IResource resource);
    }
}