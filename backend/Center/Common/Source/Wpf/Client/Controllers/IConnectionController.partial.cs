// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ConnectionController interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// The ConnectionController interface.
    /// </summary>
    public partial interface IConnectionController
    {
        /// <summary>
        /// Gets a value indicating whether the controller is configured.
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Gets the currently used background system configuration.
        /// </summary>
        BackgroundSystemConfiguration BackgroundSystemConfiguration { get; }

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
        Task<User> ConfigureAsync(string address, UserCredentials userCredentials);

        /// <summary>
        /// Creates a new channel with the current credentials and wraps it in a scope disposable
        /// object.
        /// </summary>
        /// <returns>A new channel scope.</returns>
        /// <typeparam name="T">The type of the service. It must be a valid WCF service.</typeparam>
        ChannelScope<T> CreateChannelScope<T>() where T : class;

        /// <summary>
        /// Asynchronously changes the password for the current user.
        /// </summary>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on with the update user object containing the new password.
        /// </returns>
        Task<User> ChangePasswordAsync(string newPassword);
    }
}