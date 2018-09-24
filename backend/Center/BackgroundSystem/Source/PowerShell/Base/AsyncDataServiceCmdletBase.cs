// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncDataServiceCmdletBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncDataServiceCmdletBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell.Base
{
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// The entity cmdlet base.
    /// </summary>
    public abstract class AsyncDataServiceCmdletBase : AsyncServiceCmdletBase
    {
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public abstract string EntityName { get; }

        /// <summary>
        /// Creates a new channel scope.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>
        /// A new channel scope for the given service type.
        /// </returns>
        protected virtual ChannelScope<T> CreateDataChannelScope<T>()
            where T : class
        {
            this.ConfigureDataServiceChannelScopeFactory<T>();
            return ChannelScopeFactory<T>.Current.Create(this.UserCredentials);
        }

        /// <summary>
        /// Configures the channel scope factory for the specified service.
        /// </summary>
        /// <typeparam name="T">The type of the contract.</typeparam>
        private void ConfigureDataServiceChannelScopeFactory<T>() where T : class
        {
            if (this.Configuration == null)
            {
                this.Configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            }

            ChannelScopeFactoryUtility<T>.ConfigureAsDataService(this.Configuration.DataServices, this.EntityName);
        }
    }
}