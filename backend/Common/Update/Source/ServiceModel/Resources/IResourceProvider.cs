// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Resources
{
    /// <summary>
    /// An interface that can provide resources for a given hash.
    /// </summary>
    public interface IResourceProvider
    {
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
        IResource GetResource(string hash);

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
        void AddResource(string hash, string resourceFile, bool deleteFile);
    }
}
