// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalChangeTrackingManagementBootstrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalChangeTrackingManagementBootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Setup;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Local implementation of the <see cref="ChangeTrackingManagementBootstrapper"/>.
    /// </summary>
    public class LocalChangeTrackingManagementBootstrapper : ChangeTrackingManagementBootstrapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalChangeTrackingManagementBootstrapper"/> class.
        /// </summary>
        /// <param name="backgroundSystemConfiguration">
        /// The background system configuration
        /// </param>
        /// <param name="applicationName">
        ///     The application name.
        /// </param>
        public LocalChangeTrackingManagementBootstrapper(
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string applicationName)
            : base(backgroundSystemConfiguration, applicationName)
        {
            // this is the background system side
            this.CurrentSqlNotificationManagerFilter =
                new SqlNotificationManagerFilter("[sys].ReplyTo IS NOT NULL");
            this.CurrentTimeout = TimeSpan.FromDays(31);
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
            this.Result = new ChangeTrackingManagementHostBootstrapperResult();

            await this.CreateDataServicesAsync();
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
            ChannelScopeFactory<TManager>.SetCurrent(new InstanceChannelScopeFactory<TManager>(manager));
        }

        /// <summary>
        /// Gets the notification subscription name.
        /// By default, it returns the given path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetNotificationSubscriptionName(string path)
        {
            return "Local_" + path;
        }

        /// <summary>
        /// Create data services to use local instances.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task CreateDataServicesAsync()
        {
            var hostResult = this.Result as ChangeTrackingManagementHostBootstrapperResult;
            if (hostResult == null)
            {
                throw new Exception("Local bootstrapper needs host results.");
            }

            Logger.Debug("Creating data services");
            hostResult.NonChangeTrackingServiceHosts =
                DataServicesUtility.SetupNonChangeTrackingDataServices(this.BackgroundSystemConfiguration).ToList();
            hostResult.ChangeTrackingServiceHosts =
                (await ChangeTrackingDataServicesUtility.SetupChangeTrackingDataServicesAsync(
                this.BackgroundSystemConfiguration))
                .ToList();
            Logger.Info("All data services created");
        }
    }
}