// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The connection controller.
    /// </summary>
    public partial class ConnectionController : IConnectionController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private UserCredentials credentials;

        private BackgroundSystemConfiguration backgroundSystemConfiguration;

        /// <summary>
        /// Gets a value indicating whether the controller is configured.
        /// </summary>
        public bool IsConfigured
        {
            get
            {
                return this.backgroundSystemConfiguration != null;
            }
        }

        /// <summary>
        /// Gets the currently used background system configuration.
        /// </summary>
        public BackgroundSystemConfiguration BackgroundSystemConfiguration
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't configuration when not connected to a server");
                }

                return this.backgroundSystemConfiguration;
            }
        }

        /// <summary>
        /// The configure.
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
        public async Task<User> ConfigureAsync(string address, UserCredentials userCredentials)
        {
            if (this.IsConfigured)
            {
                throw new NotSupportedException("Can't configure connection controller twice; call Close() first");
            }

            this.credentials = userCredentials;

            this.backgroundSystemConfiguration =
                await BackgroundSystemConfigurationProvider.Current.GetConfigurationAsync(address);

            ChannelScopeFactoryUtility<IMembershipService>.ConfigureAsFunctionalService(
                this.BackgroundSystemConfiguration.FunctionalServices,
                "Membership");
            ChannelScopeFactoryUtility<IUpdateService>.ConfigureAsFunctionalService(
                this.BackgroundSystemConfiguration.FunctionalServices,
                "Update");
            ChannelScopeFactoryUtility<IResourceService>.ConfigureAsFunctionalService(
                this.BackgroundSystemConfiguration.FunctionalServices, "Resources");
            ChannelScopeFactoryUtility<IContentResourceService>.ConfigureAsFunctionalService(
                this.BackgroundSystemConfiguration.FunctionalServices, "ContentResources");

            User user;
            using (var membershipService = this.CreateChannelScope<IMembershipService>())
            {
                user = await membershipService.Channel.AuthenticateUserAsync();
            }

            NotificationManagerFactoryUtility.ConfigureMediNotificationManager();
            await this.ConfigureChangeTrackingManagersAsync(
                this.BackgroundSystemConfiguration, userCredentials, this.ConfigureErrorCallback);
            return user;
        }

        /// <summary>
        /// Creates a new channel with the current credentials and wraps it in a scope disposable
        /// object.
        /// </summary>
        /// <returns>A new channel scope.</returns>
        /// <typeparam name="T">The type of the service. It must be a valid WCF service.</typeparam>
        public ChannelScope<T> CreateChannelScope<T>() where T : class
        {
            if (!this.IsConfigured)
            {
                throw new NotSupportedException("Can't create a channel scope when not connected to a server");
            }

            return ChannelScopeFactory<T>.Current.Create(this.credentials);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
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
        public async Task<User> ChangePasswordAsync(string newPassword)
        {
            if (!this.IsConfigured)
            {
                throw new NotSupportedException("Can't change password when not connected to a server");
            }

            var hashedPassword = SecurityUtility.Md5(newPassword);
            User result;
            Logger.Info("Changing password of user {0} to (hashed) {1}", this.credentials.Username, hashedPassword);
            using (var userService = this.CreateChannelScope<IUserDataService>())
            {
                var user =
                    (await userService.Channel.QueryAsync(UserQuery.Create().WithUsername(this.credentials.Username)))
                        .First();
                user.HashedPassword = hashedPassword;
                result = await userService.Channel.UpdateAsync(user);
            }

            this.credentials = new UserCredentials(this.credentials.Username, hashedPassword);
            this.UpdateChangeTrackingCredentials();
            return result;
        }

        /// <summary>
        /// Closes this connection controller so it can be reused later
        /// (<see cref="ConfigureAsync"/> has to be called again for this).
        /// </summary>
        public void Close()
        {
            if (!this.IsConfigured)
            {
                return;
            }

            this.DisposeChangeTrackingManagers();
            this.credentials = null;
            this.backgroundSystemConfiguration = null;
        }

        private void ConfigureErrorCallback(Exception[] e)
        {
            var aggregateException = new AggregateException(e);
            Logger.Error(aggregateException, "Error while configuring services.");
            MessageBox.Show(
                Strings.Login_CanNotGetConfigurationTitle,
                Strings.Login_CanNotGetConfiguration,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // dispose managed resources here
            }

            this.Close();
        }
    }
}
