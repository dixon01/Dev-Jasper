// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WcfLoggingAzureConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WcfLoggingAzureConfigurator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Extensions
{
    using System;
    using System.Diagnostics;

    using Gorba.Center.BackgroundSystem.Host;
    using Gorba.Center.Common.Azure;

    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Configurator that also enables WCF logging.
    /// </summary>
    internal class WcfLoggingAzureConfigurator : AzureConfigurator.DefaultAzureConfigurator
    {
        private readonly object locker = new object();

        private bool isGenericLoggingConfigured;

        /// <summary>
        /// Configures the Azure system.
        /// </summary>
        public override void EnsureConfiguration()
        {
            if (!this.isGenericLoggingConfigured)
            {
                lock (this.locker)
                {
                    if (!this.isGenericLoggingConfigured)
                    {
                        base.EnsureConfiguration();
                        this.isGenericLoggingConfigured = true;
                    }
                }
            }

            this.EnsureWcfLogging();
        }

        private void EnsureWcfLogging(string logLevelName = null)
        {
            if (logLevelName == null)
            {
                logLevelName = RoleEnvironment.GetConfigurationSettingValue(PredefinedAzureItems.Settings.LogLevel);
                if (string.IsNullOrEmpty(logLevelName))
                {
                    this.Logger.Debug("Can't find the LogLevel setting");
                    return;
                }
            }

            try
            {
                SourceLevels sourceLevel;
                var success = LogUtility.MapNLogLevelToSourceLevel(logLevelName, out sourceLevel);
                if (!success)
                {
                    this.Logger.Debug("Couldn't reconfigure SourceLevel");
                    return;
                }

                if (LogLevelController.Instance.LevelController == null)
                {
                    this.Logger.Debug("The LogLevelController delegate is null.");
                    return;
                }

                this.Logger.Info("Setting SourceLevel to '{0}'", sourceLevel);
                try
                {
                    var ts = new TraceSource("System.ServiceModel");
                    ts.Switch.Level = sourceLevel;
                    this.Logger.Debug("Switch level set to '{0}' for trace source System.ServiceModel", sourceLevel);
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Can't configure switch level for System.ServiceModel");
                }

                LogLevelController.Instance.LevelController(sourceLevel);
            }
            catch (Exception exception)
            {
                Trace.TraceError("Error while setting SourceLevel: {0}", exception.Message);
            }
        }
    }
}