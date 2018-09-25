// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceAccess.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceAccess type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System.IO;

    /// <summary>
    /// Interface to access a resource from an <see cref="IResourceStore"/> as a stream.
    /// The two methods are provided so they can be implemented in the most effective way for each
    /// <see cref="IResourceStore"/> implementation.
    /// </summary>
    public interface IResourceAccess
    {
        /// <summary>
        /// Copies the resource to the given file location.
        /// </summary>
        /// <param name="newFileName">
        /// The local file name to copy the file to.
        /// </param>
        void CopyTo(string newFileName);

        /// <summary>
        /// Opens the stream to read the resource from it.
        /// In subclasses it might be possible to
        /// limit the use of this method per object
        /// to exactly once.
        /// </summary>
        /// <returns>
        /// A stream from which the resource can be read.
        /// </returns>
        Stream OpenRead();
    }
}