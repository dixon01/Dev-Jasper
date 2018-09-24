// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamContentResourceWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamContentResourceWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Content
{
    using System;
    using System.IO;

    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Defines a content stream and the (optional) relative resource.
    /// </summary>
    public class StreamContentResourceWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamContentResourceWrapper"/> class.
        /// </summary>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="resource">
        ///     The resource.
        /// </param>
        public StreamContentResourceWrapper(Stream content, ContentResource resource = null)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this.Content = content;
            this.Resource = resource;
        }

        /// <summary>
        /// Gets the content stream.
        /// </summary>
        public Stream Content { get; private set; }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        public ContentResource Resource { get; private set; }
    }
}