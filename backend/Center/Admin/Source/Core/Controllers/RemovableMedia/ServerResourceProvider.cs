// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerResourceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerResourceProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.RemovableMedia
{
    using System;
    using System.IO;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Implementation of <see cref="IResourceProvider"/> that downloads resources from the background system.
    /// </summary>
    public class ServerResourceProvider : IResourceProvider
    {
        private readonly IConnectionController connectionController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerResourceProvider"/> class.
        /// </summary>
        /// <param name="connectionController">
        /// The connection controller.
        /// </param>
        public ServerResourceProvider(IConnectionController connectionController)
        {
            this.connectionController = connectionController;
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
            Resource resource;
            using (var resourceService = this.connectionController.CreateChannelScope<IResourceService>())
            {
                resource = resourceService.Channel.GetAsync(hash).Result;
            }

            if (resource == null)
            {
                throw new UpdateException("Couldn't find resource on server with hash " + hash);
            }

            return new ResourceWrapper(resource, this.connectionController);
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
            throw new NotSupportedException();
        }

        private class ResourceWrapper : IResource
        {
            private readonly Resource resource;

            private readonly IConnectionController connectionController;

            public ResourceWrapper(Resource resource, IConnectionController connectionController)
            {
                this.resource = resource;
                this.connectionController = connectionController;
            }

            public string Hash
            {
                get
                {
                    return this.resource.Hash;
                }
            }

            public void CopyTo(string filePath)
            {
                using (var output = File.Create(filePath))
                {
                    using (var input = this.OpenRead())
                    {
                        input.CopyTo(output);
                    }
                }
            }

            public Stream OpenRead()
            {
                return new ResourceStream(this.Hash, this.connectionController);
            }
        }

        private class ResourceStream : WrapperStream
        {
            private readonly ChannelScope<IResourceService> resourceService;

            public ResourceStream(string hash, IConnectionController connectionController)
            {
                this.resourceService = connectionController.CreateChannelScope<IResourceService>();
                var request = new ResourceDownloadRequest { Hash = hash };
                var result = this.resourceService.Channel.DownloadAsync(request).Result;
                this.Open(result.Content);
            }

            public override void Close()
            {
                base.Close();
                this.resourceService.Dispose();
            }
        }
    }
}