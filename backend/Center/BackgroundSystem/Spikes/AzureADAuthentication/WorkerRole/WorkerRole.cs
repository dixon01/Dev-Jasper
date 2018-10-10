// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerRole.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WorkerRole type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WorkerRole
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading;

    using Host;

    using Microsoft.WindowsAzure.ServiceRuntime;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    using ServiceModel;

    /// <summary>
    /// The worker role.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        private static Logger logger;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        /// <summary>
        /// The run.
        /// </summary>
        public override void Run()
        {
            Trace.TraceInformation("WorkerRole is running");

            try
            {
                this.Run(this.cancellationTokenSource.Token);
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        /// <summary>
        /// The on start.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole has been started");

            return result;
        }

        /// <summary>
        /// The on stop.
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole has stopped");
        }

        private void Run(CancellationToken cancellationToken)
        {
            this.ConfigureNLog();

            var host = new TestHost();
            host.Start(this.CreateServiceEndpoint());
            cancellationToken.WaitHandle.WaitOne();
            host.Stop();
        }

        private ServiceEndpoint CreateServiceEndpoint()
        {
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["TestServices"];

            var uriBuilder =
                new UriBuilder("net.tcp", endpoint.IPEndpoint.Address.ToString(), endpoint.IPEndpoint.Port)
                {
                    Path = "TestService"
                };
            logger.Info("EndpointAddress Uri: {0}", uriBuilder.Uri);
            var endPointAddress = new EndpointAddress(uriBuilder.Uri, new DnsEndpointIdentity("BackgroundSystem"));
            var binding = ServiceUtility.GetNetTcpBinding();
            return new ServiceEndpoint(ContractDescription.GetContract(typeof(ITestService)), binding, endPointAddress);
        }

        private void ConfigureNLog()
        {
            try
            {
                var logs = RoleEnvironment.GetLocalResource("Logs").RootPath;
                var configFile = Path.Combine(logs, "NLog.config");
                var configuration = new XmlLoggingConfiguration(configFile);
                Environment.SetEnvironmentVariable("CenterLogsPath", logs);
                Environment.SetEnvironmentVariable("CenterRoleInstanceId", RoleEnvironment.CurrentRoleInstance.Id);
                Environment.SetEnvironmentVariable("CenterDeploymentId", RoleEnvironment.DeploymentId);
                LogManager.Configuration = configuration;
                logger = LogManager.GetCurrentClassLogger();
                logger.Info("Logging configured");
            }
            catch (Exception ex)
            {
                SimpleConfigurator.ConfigureForTargetLogging(new DebuggerTarget());
                Trace.TraceError("Error while configuring NLog. Stack trace: {0}", ex.StackTrace);
                logger = LogManager.GetCurrentClassLogger();
            }
        }
    }
}
