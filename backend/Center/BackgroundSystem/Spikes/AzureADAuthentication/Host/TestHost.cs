// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHost.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The test host.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Host
{
    using System;
    using System.Diagnostics;
    using System.IdentityModel.Selectors;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    using NLog;

    using ServiceModel.Certificates;

    /// <summary>
    /// The test host.
    /// </summary>
    public class TestHost
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly bool onPremises;

        private readonly object locker = new object();

        private readonly string domain;

        private bool running;

        private ServiceHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHost"/> class.
        /// </summary>
        /// <param name="onPremises">
        /// The on premises.
        /// </param>
        /// <param name="domain">
        /// The domain.
        /// </param>
        public TestHost(bool onPremises = false, string domain = null)
        {
            this.onPremises = onPremises;
            this.domain = domain;
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="serviceEndpoint">
        /// The service endpoint.
        /// </param>
        public void Start(ServiceEndpoint serviceEndpoint)
        {
            lock (this.locker)
            {
                if (this.running)
                {
                    return;
                }

                this.running = true;
            }

            try
            {
                var service = new TestService();
                this.host = new ServiceHost(service);
                Logger.Info("Creating the endpoint");
                var endpoint = serviceEndpoint;
                Logger.Info("Creating service credentials.");
                var serviceCredential = this.host.Description.Behaviors.Find<ServiceCredentials>();
                if (serviceCredential == null)
                {
                    serviceCredential = new ServiceCredentials();
                    this.host.Description.Behaviors.Add(serviceCredential);
                }

                serviceCredential.ServiceCertificate.Certificate = CertificatesUtility.GetCertificate();
                serviceCredential.UserNameAuthentication.UserNamePasswordValidationMode =
                    UserNamePasswordValidationMode.Custom;

                var validator = this.onPremises
                                    ? (UserNamePasswordValidator)new OnPremisesLoginValidator { Domain = this.domain }
                                    : new AzureLoginValidator();
                serviceCredential.UserNameAuthentication.CustomUserNamePasswordValidator = validator;
                Logger.Info("Adding service endpoint");
                this.host.AddServiceEndpoint(endpoint);
                Logger.Info("Opening the host...");
                this.host.Open();
                Logger.Info("Host open");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error opening the host.", e);
                Trace.TraceError("Error setup host. Stack trace: {0}", e.StackTrace);
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            lock (this.locker)
            {
                if (!this.running)
                {
                    return;
                }

                this.running = false;
            }

            if (this.host != null)
            {
                Logger.Info("Closing host");
                this.host.Close();
            }
        }
    }
}
