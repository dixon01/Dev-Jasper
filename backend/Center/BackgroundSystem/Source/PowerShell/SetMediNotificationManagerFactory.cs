// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetMediNotificationManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SetMediNotificationManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;

    using NLog;

    /// <summary>
    /// Sets the medi notification factory.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "MediNotificationManagerFactory")]
    public class SetMediNotificationManagerFactory : PSCmdlet
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            Logger.Info("Configuring notifications to use Medi");
            Common.Client.NotificationManagerFactoryUtility.ConfigureMediNotificationManager();
        }
    }
}