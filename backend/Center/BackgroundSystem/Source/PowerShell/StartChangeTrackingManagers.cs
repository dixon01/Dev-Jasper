// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartChangeTrackingManagers.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StartChangeTrackingManagers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System;
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Starts the change tracking managers.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "ChangeTrackingManagers")]
    public class StartChangeTrackingManagers : AsyncCmdlet
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the background system configuration.
        /// </summary>
        [Parameter(Mandatory = true)]
        public BackgroundSystemConfiguration BackgroundSystemConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the user credentials.
        /// </summary>
        [Parameter(Mandatory = true)]
        public UserCredentials UserCredentials { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            try
            {
                var sessionId = NotificationManagerUtility.GenerateUniqueSessionId();
                var manager = new RemoteChangeTrackingManagementBootstrapper(
                    this.BackgroundSystemConfiguration,
                    "TestChangeTrackingManagers",
                    sessionId);
                Logger.Debug("Running the manager");
                var result = await manager.RunAsync(this.UserCredentials);
                Logger.Info("Created the change tracking managers");
                var set = result.ToSet();
                this.WriteObject(set);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "ErrorStartChangeTrackingManagers",
                        ErrorCategory.ReadError,
                        this.BackgroundSystemConfiguration));
            }
        }
    }
}