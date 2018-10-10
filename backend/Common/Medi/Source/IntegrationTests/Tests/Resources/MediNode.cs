// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediNode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The medi node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// The medi node.
    /// </summary>
    internal class MediNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediNode"/> class.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        public MediNode(MediAddress address)
        {
            this.Address = address;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        public MediAddress Address { get; private set; }

        /// <summary>
        /// Gets the message dispatcher.
        /// </summary>
        public IRootMessageDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets the resource service or null if none was configured.
        /// </summary>
        public IResourceService ResourceService
        {
            get
            {
                return this.Dispatcher.GetService<IResourceService>();
            }
        }

        /// <summary>
        /// Gets the config of this node or null if <see cref="Configure"/>
        /// wasn't called yet.
        /// </summary>
        public MediConfig Config { get; private set; }

        /// <summary>
        /// Configures this node with the given configurations.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public void Configure(MediConfig config)
        {
            if (this.Config != null)
            {
                throw new ApplicationException("Can't configure node twice");
            }

            this.Config = config;
        }

        /// <summary>
        /// Starts this node.
        /// </summary>
        public void Start()
        {
            this.Dispatcher =
                MessageDispatcher.Create(
                    new ObjectConfigurator(this.Config, this.Address.Unit, this.Address.Application));
        }

        /// <summary>
        /// Stops this node.
        /// </summary>
        public void Stop()
        {
            this.Dispatcher.Dispose();
        }
    }
}