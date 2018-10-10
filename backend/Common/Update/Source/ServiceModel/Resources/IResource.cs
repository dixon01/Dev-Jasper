// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResource.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Resources
{
    using System.IO;

    /// <summary>
    /// A resource that can be used to create an update.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the unique MD5 hash of this resource.
        /// </summary>
        string Hash { get; }

        /// <summary>
        /// Copies this resource to the given path.
        /// </summary>
        /// <param name="filePath">
        /// The full file path where to copy the resource to.
        /// </param>
        void CopyTo(string filePath);

        /// <summary>
        /// Opens this resource for reading.
        /// </summary>
        /// <returns>
        /// A stream that allows reading the resource.
        /// </returns>
        Stream OpenRead();
    }
}