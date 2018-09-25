// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCmdletBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceCmdletBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// Base for all cmdlets.
    /// </summary>
    public abstract class ServiceCmdletBase : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the user credentials.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public UserCredentials UserCredentials { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        [Parameter(Position = 2)]
        public BackgroundSystemConfiguration Configuration { get; set; }

        /// <summary>
        /// Creates a new channel scope.
        /// </summary>
        /// <param name="entityName">
        /// The name of the entity.
        /// </param>
        /// <typeparam name="T">
        /// The type of the service.
        /// </typeparam>
        /// <returns>
        /// A new channel scope for the given service type.
        /// </returns>
        protected virtual ChannelScope<T> CreateDataChannelScope<T>(string entityName)
            where T : class
        {
            this.ConfigureDataServiceChannelScopeFactory<T>(entityName);
            return ChannelScopeFactory<T>.Current.Create(this.UserCredentials);
        }

        /// <summary>
        /// Creates a new channel scope.
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>
        /// A new channel scope for the given service type.
        /// </returns>
        protected virtual ChannelScope<T> CreateFunctionalChannelScope<T>(string serviceName) where T : class
        {
            this.ConfigureFunctionalServiceChannelScopeFactory<T>(serviceName);
            return ChannelScopeFactory<T>.Current.Create(this.UserCredentials);
        }

        /// <summary>
        /// Configures the channel scope factory for the specified service.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <typeparam name="T">The type of the contract.</typeparam>
        private void ConfigureDataServiceChannelScopeFactory<T>(string entityName) where T : class
        {
            if (this.Configuration == null)
            {
                this.Configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            }

            ChannelScopeFactoryUtility<T>.ConfigureAsDataService(this.Configuration.DataServices, entityName);
        }

        /// <summary>
        /// Configures the channel scope factory for the specified service.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <typeparam name="T">The type of the contract.</typeparam>
        private void ConfigureFunctionalServiceChannelScopeFactory<T>(string serviceName) where T : class
        {
            if (this.Configuration == null)
            {
                this.Configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            }

            ChannelScopeFactoryUtility<T>.ConfigureAsFunctionalService(this.Configuration.DataServices, serviceName);
        }
    }
}