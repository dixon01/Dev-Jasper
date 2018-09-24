// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HostApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.ConsoleHost
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Gorba.Center.BackgroundSystem.Core.Security;
    using Gorba.Center.BackgroundSystem.Host;
    using Gorba.Common.SystemManagement.Host;

    using NLog;
    using NLog.Config;

    /// <summary>
    /// Defines the host application for the BackgroundSystem.
    /// </summary>
    public class HostApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// Implementing classes should either override <see cref="ApplicationBase.DoRun(string[])"/> or this method.
        /// </summary>
        protected override void DoRun()
        {
            this.Logger.Info("Starting host");
            LogManager.ConfigurationReloaded += this.OnNLogConfigurationReloaded;

            try
            {
                LoginValidatorProvider.SetCurrent(new DomainLoginValidatorProvider());
                var systemHost = new BackgroundSystemHost();
                systemHost.Start();
                systemHost.VerifyResources();
                this.SetRunning();
                this.runWait.WaitOne();
                systemHost.Stop();
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error in the host application");
                LogManager.Flush();
            }

            this.cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="ApplicationBase.DoRun()"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.runWait.Set();
        }

        /// <summary>
        /// <see href="https://wcfpro.wordpress.com/2010/11/21/how-to-add-wcf-traces-programmatically/"/>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnNLogConfigurationReloaded(object sender, EventArgs e)
        {
            LoggingConfigurationReloadedEventArgs logEvent;
            if (e != null)
            {
                logEvent = e as LoggingConfigurationReloadedEventArgs;
                if (logEvent == null)
                {
                    // tsnh
                    this.Logger.Info("LogEvent was not from NLog.");
                    return;
                }

                if (logEvent.Exception != null)
                {
                    this.Logger.Info("Error in config nlog.config. Will not change source log level.");
                    return;
                }
            }

            this.Logger.Info("Logger was reconfigured");

            // determine current nlog level
            var rule =
                LogManager.Configuration.LoggingRules.FirstOrDefault(i => i.Targets.Any(t => t.Name.Equals("file")));
            if (rule == null)
            {
                this.Logger.Info("No log rule found.");
                return;
            }

            // get first log level, might not be
            var logLevel = rule.Levels.FirstOrDefault();
            if (logLevel == null)
            {
                this.Logger.Info("No log level in config file.");
                return;
            }

            // change WCF log level
            SourceLevels newSourceLevel;
            var success = logLevel.MapNLogLevelToSourceLevel(out newSourceLevel);
            if (!success)
            {
                return;
            }

            LogLevelController.Instance.LevelController(newSourceLevel);
            this.Logger.Info("Reconfigured application log level to: {0}", newSourceLevel);
        }
    }
}