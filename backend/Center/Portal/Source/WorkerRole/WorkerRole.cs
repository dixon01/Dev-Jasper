// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerRole.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WorkerRole type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.WorkerRole
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Azure;

    using Gorba.Center.Common.Azure;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host;
    using Gorba.Center.Portal.Host.Configuration;
    using Gorba.Center.Portal.Host.Settings;

    using Microsoft.WindowsAzure.ServiceRuntime;

    using NLog;

    /// <summary>
    /// The worker role to host the Portal.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        /// <inheritdoc />
        public override void Run()
        {
            Trace.TraceInformation("WorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        /// <inheritdoc />
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at <see href="http://go.microsoft.com/fwlink/?LinkId=166357" />.
            var result = base.OnStart();

            Trace.TraceInformation("WorkerRole has been started");

            return result;
        }

        /// <inheritdoc />
        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole has stopped");
        }

#pragma warning disable 1998
        private async Task RunAsync(CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            try
            {
                AzureConfigurator.Current.EnsureConfiguration();
                StartupConfigurator.Set(new AzureStartupConfigurator());
                PortalSettingsProvider.SetCurrent(new AzurePortalSettingsProvider());
                BackgroundSystemConfigurationProvider.Set(new AzureBackgroundSystemConfigurationProvider());
                PortalRepositoryConfigurationProvider.Set(new AzurePortalRepositoryConfigurationProvider());
                Logger.Info("Starting");
                ServicePointManager.DefaultConnectionLimit = 24;
                using (PortalUtility.GetHost(Host.PredefinedAzureItems.Endpoints.HttpEndpoint))
                {
                    using (
                        PortalUtility.GetHost(
                            Host.PredefinedAzureItems.Endpoints.HttpsEndpoint,
                            Host.PredefinedAzureItems.Settings.EnableHttps))
                    {
                        cancellationToken.WaitHandle.WaitOne();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception, "Can't start the portal because of an unhandled exception");
            }
        }
    }
}