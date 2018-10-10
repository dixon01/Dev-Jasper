// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateClientFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateClientFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Clients
{
    using System;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Factory for <see cref="IUpdateClient"/> objects.
    /// </summary>
    public class UpdateClientFactory
    {
        static UpdateClientFactory()
        {
            Instance = new UpdateClientFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateClientFactory"/> class.
        /// </summary>
        protected UpdateClientFactory()
        {
        }

        /// <summary>
        /// Gets or sets the single instance.
        /// </summary>
        public static UpdateClientFactory Instance { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="IUpdateClient"/> depending on the type of
        /// <see cref="UpdateClientConfigBase"/> provided.
        /// </summary>
        /// <param name="config">
        /// The update client config.
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IUpdateClient"/> implementation.
        /// </returns>
        public virtual IUpdateClient Create(UpdateClientConfigBase config, IUpdateContext context)
        {
            if (string.IsNullOrEmpty(config.Name))
            {
                throw new ArgumentException("UpdateClientConfigBase.Name has to be a valid name");
            }

            var provider = ConfigImplementationFactory.CreateFromConfig<IUpdateClient>(config);
            provider.Configure(config, context);
            return provider;
        }
    }
}
