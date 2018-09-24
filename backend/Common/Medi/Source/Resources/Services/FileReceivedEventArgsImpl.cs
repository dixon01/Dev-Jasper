// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileReceivedEventArgsImpl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileReceivedEventArgsImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;

    /// <summary>
    /// Implementation of <see cref="FileReceivedEventArgs"/> for the resource service in this namespace.
    /// </summary>
    internal class FileReceivedEventArgsImpl : FileReceivedEventArgs
    {
        private readonly IResourceAccess resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReceivedEventArgsImpl"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        public FileReceivedEventArgsImpl(MediAddress source, string originalFileName, IResourceAccess resource)
            : base(source, originalFileName)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            this.resource = resource;
        }

        /// <summary>
        /// Copies the received (temporary) file to the given location.
        /// </summary>
        /// <param name="localFileName">
        /// The local file name to copy the file to.
        /// </param>
        public override void CopyTo(string localFileName)
        {
            this.resource.CopyTo(localFileName);
        }
    }
}