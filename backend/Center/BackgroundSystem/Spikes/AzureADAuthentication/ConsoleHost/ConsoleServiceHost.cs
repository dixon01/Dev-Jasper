// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleServiceHost.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The console service host.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoleHost
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading;

    using Host;

    using NLog;

    using ServiceModel;

    /// <summary>
    /// The console service host.
    /// </summary>
    public class ConsoleServiceHost
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            try
            {
                Logger.Info("Starting console host.");
                var domain = ConfigurationManager.AppSettings["Domain"];
                var systemHost = new TestHost(true, domain);
                systemHost.Start(CreateServiceEndPoint());
                this.runWait.WaitOne();
                Logger.Info("Stop console host.");
                systemHost.Stop();
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error in the host application", exception);
                LogManager.Flush();
            }

            this.cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.runWait.Set();
        }

        private static ServiceEndpoint CreateServiceEndPoint()
        {
            var endPoint = ConfigurationManager.AppSettings["HostAddress"];
            var uriBuilder =
                new UriBuilder(endPoint)
                {
                    Path = "TestService"
                };
            Logger.Info("EndpointAddress Uri: {0}", uriBuilder.Uri);
            var endPointAddress = new EndpointAddress(uriBuilder.Uri, new DnsEndpointIdentity("BackgroundSystem"));
            var binding = ServiceUtility.GetNetTcpBinding();
            return new ServiceEndpoint(
         ContractDescription.GetContract(typeof(ITestService)), binding, endPointAddress);
        }
    }
}
