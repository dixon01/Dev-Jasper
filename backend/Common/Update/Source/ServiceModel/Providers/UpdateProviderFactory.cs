// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProviderFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProviderFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Providers
{
    using System;

    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Factory for <see cref="IUpdateProvider"/> objects.
    /// </summary>
    public class UpdateProviderFactory
    {
        static UpdateProviderFactory()
        {
            Instance = new UpdateProviderFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProviderFactory"/> class.
        /// </summary>
        protected UpdateProviderFactory()
        {
        }

        /// <summary>
        /// Gets or sets the single instance.
        /// </summary>
        public static UpdateProviderFactory Instance { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="IUpdateProvider"/> depending on the type of
        /// <see cref="UpdateProviderConfigBase"/> provided.
        /// </summary>
        /// <param name="config">
        /// The update client config.
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IUpdateProvider"/> implementation.
        /// </returns>
        public virtual IUpdateProvider Create(UpdateProviderConfigBase config, IUpdateContext context)
        {
            if (string.IsNullOrEmpty(config.Name))
            {
                throw new ArgumentException("UpdateProviderConfigBase.Name has to be a valid name");
            }

            var provider = ConfigImplementationFactory.CreateFromConfig<IUpdateProvider>(config);
            provider.Configure(config, context);
            return provider;
        }
    }
}
