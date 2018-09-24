// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteChangeTrackingManagementBootstrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteChangeTrackingManagementBootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Local implementation of the <see cref="ChangeTrackingManagementBootstrapper"/>.
    /// </summary>
    public class RemoteChangeTrackingManagementBootstrapper : ChangeTrackingManagementBootstrapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteChangeTrackingManagementBootstrapper"/> class.
        /// </summary>
        /// <param name="backgroundSystemConfiguration">
        ///     The background System Configuration.
        /// </param>
        /// <param name="applicationName">
        ///     The application name.
        /// </param>
        /// <param name="sessionId">
        /// A unique identifier for an application session.
        /// </param>
        public RemoteChangeTrackingManagementBootstrapper(
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string applicationName,
            string sessionId)
            : base(backgroundSystemConfiguration, applicationName, sessionId)
        {
            // this is the client side
            this.CurrentSqlNotificationManagerFilter =
                new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NULL");
            this.CurrentTimeout = TimeSpan.FromHours(8);
        }

        /// <summary>
        /// The run async.
        /// </summary>
        /// <param name="userCredentials">
        /// The user credentials.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task<ChangeTrackingManagementBootstrapperResult> RunAsync(
            UserCredentials userCredentials)
        {
            this.Result = new ChangeTrackingManagementBootstrapperResult();

            this.CreateDataServices();
            return await base.RunAsync(userCredentials);
        }

        /// <summary>
        /// Configures the channel scope factory.
        /// The default implementation expects it to be already configured and it doesn't to anything.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="backgroundSystemConfiguration">The configuration.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <typeparam name="TManager">The type of the manager.</typeparam>
        /// <typeparam name="TService">The type of the service.</typeparam>
        protected override void ConfigureChannelScopeFactory<TManager, TService>(
            TManager manager,
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string serviceName)
        {
            ChannelScopeFactoryUtility<TService>.ConfigureAsDataService(
                backgroundSystemConfiguration.DataServices, serviceName);
        }

        /// <summary>
        /// Create data services to use remote access.
        /// </summary>
        private void CreateDataServices()
        {
            Logger.Debug("Creating remote data services");

            foreach (var path in this.GetPaths())
            {
                this.CreateRemoteDataService(this.BackgroundSystemConfiguration.DataServices, path);
            }

            Logger.Info("All remote data services created");
        }
    }
}