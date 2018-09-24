// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionControllerMock.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionControllerMock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Tests.Mocks
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.Wpf.Client.Controllers;

    /// <summary>
    /// Mock for <see cref="IConnectionController"/> that provides
    /// certain change tracking functionality for unit testing.
    /// </summary>
    public partial class ConnectionControllerMock : IConnectionController
    {
        private UserCredentials credentials;

        /// <summary>
        /// Gets a value indicating whether the controller is configured.
        /// </summary>
        public bool IsConfigured { get; private set; }

        /// <summary>
        /// Gets the currently used background system configuration.
        /// </summary>
        public BackgroundSystemConfiguration BackgroundSystemConfiguration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures the ChangeTrackingManagers and functional services.
        /// </summary>
        public void Configure()
        {
            this.ConfigureChangeTrackingManagers();
            this.IsConfigured = true;
        }

        /// <summary>
        /// Configures the ChangeTrackingManagers and functional services asynchronously.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="userCredentials">
        /// The user credentials.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await.
        /// </returns>
        public virtual Task<User> ConfigureAsync(string address, UserCredentials userCredentials)
        {
            this.credentials = userCredentials;
            this.Configure();
            return Task.FromResult(new User());
        }

        /// <summary>
        /// Creates a new channel with the current credentials and wraps it in a scope disposable
        /// object.
        /// </summary>
        /// <returns>A new channel scope.</returns>
        /// <typeparam name="T">The type of the service. It must be a valid WCF service.</typeparam>
        public virtual ChannelScope<T> CreateChannelScope<T>() where T : class
        {
            return ChannelScopeFactory<T>.Current.Create(this.credentials);
        }

        /// <summary>
        /// Asynchronously changes the password for the current user.
        /// </summary>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on with the update user object containing the new password.
        /// </returns>
        public virtual Task<User> ChangePasswordAsync(string newPassword)
        {
            throw new NotSupportedException("Can't change password with this mock");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// Closes all connections.
        /// </summary>
        public virtual void Close()
        {
        }
    }
}