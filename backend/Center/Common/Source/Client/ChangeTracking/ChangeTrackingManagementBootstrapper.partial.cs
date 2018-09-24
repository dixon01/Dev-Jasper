// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingManagementBootstrapper.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingManagementBootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Bootstrapper to start change tracking managers locally (direct access to services stored in
    /// <see cref="DependencyResolver"/>).
    /// </summary>
    public partial class ChangeTrackingManagementBootstrapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private int runningCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingManagementBootstrapper"/> class.
        /// </summary>
        /// <param name="backgroundSystemConfiguration">
        ///     The background system configuration to use.
        /// </param>
        /// <param name="applicationName">
        ///     The application name.
        /// </param>
        /// <param name="sessionId">
        /// A unique identifier for an application session.
        /// </param>
        protected ChangeTrackingManagementBootstrapper(
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string applicationName,
            string sessionId = null)
        {
            this.SessionId = sessionId;
            this.BackgroundSystemConfiguration = backgroundSystemConfiguration;
            this.ApplicationName = applicationName;
        }

        /// <summary>
        /// Tracks the progress of this bootstrapper.
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Gets the background system configuration.
        /// </summary>
        protected BackgroundSystemConfiguration BackgroundSystemConfiguration { get; private set; }

        /// <summary>
        /// Gets or sets the current sql notification manager filter used for NotificationSubscriptionConfiguration.
        /// </summary>
        protected SqlNotificationManagerFilter CurrentSqlNotificationManagerFilter { get; set; }

        /// <summary>
        /// Gets or sets the current timeout used for NotificationSubscriptionConfiguration.
        /// </summary>
        protected TimeSpan CurrentTimeout { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        protected ChangeTrackingManagementBootstrapperResult Result { get; set; }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        protected string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the session id used for notification subscriptions.
        /// </summary>
        protected string SessionId { get; private set; }

        /// <summary>
        /// Runs the bootstrapper creating, initializing and registering all change tracking managers.
        /// After running the bootstrapper, services are ready to be used.
        /// </summary>
        /// <param name="userCredentials">
        /// The user credentials.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        /// <remarks>
        /// If the <see cref="SessionId" /> is null or empty, a new random session id will be created.
        /// </remarks>
        public virtual async Task<ChangeTrackingManagementBootstrapperResult> RunAsync(UserCredentials userCredentials)
        {
            if (string.IsNullOrEmpty(this.SessionId))
            {
                this.Result.SessionId = this.SessionId = NotificationManagerUtility.GenerateUniqueSessionId();
                Logger.Trace("Session Id not specified. Generated Id: {0}", this.SessionId);
            }
            else
            {
                this.Result.SessionId = this.SessionId;
                Logger.Trace("Session Id explicitly specified: {0}", this.SessionId);
            }

            var notificationSubscriptionConfigurations = this.GetPaths();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var configurations =
                notificationSubscriptionConfigurations.Select(
                    path => this.CreateConfiguration(this.BackgroundSystemConfiguration, path));
            var s = configurations.Select(
                    async configuration =>
                    await this.CreateAsync(configuration, userCredentials, this.BackgroundSystemConfiguration));

            Logger.Debug("Waiting for all managers to be ready");
            await Task.WhenAll(s);
            stopwatch.Stop();

            Logger.Info("All change tracking managers started in {0} ms", stopwatch.ElapsedMilliseconds);
            return this.Result;
        }

        /// <summary>
        /// Creates the notification subscription configuration.
        /// By default, it subscribes to messages with null ReplyToSessionId with unique subscription.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="notificationManagerConfiguration">
        /// The notification manager configuration.
        /// </param>
        /// <returns>
        /// The <see cref="NotificationSubscriptionConfiguration"/>.
        /// </returns>
        protected virtual NotificationSubscriptionConfiguration CreateNotificationSubscriptionConfiguration(
            string path,
            NotificationManagerConfiguration notificationManagerConfiguration)
        {
            // TODO this must be done better when refactoring is working
            var depluralized = path.Remove(path.Length - 1);
            return new NotificationSubscriptionConfiguration(
                notificationManagerConfiguration,
                this.ApplicationName,
                this.GetNotificationSubscriptionName(depluralized),
                this.SessionId)
                       {
                           IsUnique = this.SessionId != null, // TODO OK?
                           Filter   = this.CurrentSqlNotificationManagerFilter,
                           Timeout  = this.CurrentTimeout
                       };
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
        protected virtual void ConfigureChannelScopeFactory<TManager, TService>(
            TManager manager,
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string serviceName)
            where TManager : class
            where TService : class
        {
            Logger.Trace("By default, the ChannelScopeFactory is not reconfigured");
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
        protected virtual string GetNotificationSubscriptionName(string path)
        {
            return path;
        }

        private void UpdateProgress()
        {
            this.runningCount++;
            this.RaiseProgressChanged(new ProgressChangedEventArgs(this.runningCount * 100 / TotalCount, null));
        }

        private void RaiseProgressChanged(ProgressChangedEventArgs e)
        {
            var handler = this.ProgressChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        private NotificationSubscriptionConfiguration CreateConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string path)
        {
            var notificationManagerConfiguration = new NotificationManagerConfiguration
            {
                ConnectionString = backgroundSystemConfiguration.NotificationsConnectionString,
                Path = path
            };

            return this.CreateNotificationSubscriptionConfiguration(path, notificationManagerConfiguration);
        }
    }
}