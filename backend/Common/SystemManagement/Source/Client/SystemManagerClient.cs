// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// The system manager client.
    /// </summary>
    public class SystemManagerClient : SystemManagerClientBase, IManageable
    {
        private static volatile SystemManagerClient instance;

        private readonly Dictionary<string, ApplicationRegistration> registeredApplications =
            new Dictionary<string, ApplicationRegistration>();

        private SystemManagerClient()
            : base(MessageDispatcher.Instance, MessageDispatcher.Instance.LocalAddress.Unit)
        {
            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                "SystemManagerClient", root, this);
            root.AddChild(provider);
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static SystemManagerClient Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (typeof(SystemManagerClient))
                {
                    if (instance == null)
                    {
                        instance = new SystemManagerClient();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Creates a registration for an application.
        /// This method has to be called by classes interfacing with System Manager.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The new registration. You should call <see cref="IApplicationRegistration.Register"/>
        /// on the returned object after this method returns.
        /// </returns>
        public IApplicationRegistration CreateRegistration(string name)
        {
            ApplicationRegistration registration;
            lock (this.registeredApplications)
            {
                if (this.registeredApplications.ContainsKey(name))
                {
                    throw new ArgumentException("Can't register application twice: " + name, "name");
                }

                registration = new ApplicationRegistration(name, this);
                this.registeredApplications.Add(name, registration);
            }

            registration.Initialize();
            return registration;
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            lock (this.registeredApplications)
            {
                foreach (var application in this.registeredApplications.Values)
                {
                    yield return parent.Factory.CreateManagementProvider(application.Name, parent, application);
                }
            }
        }

        /// <summary>
        /// Deregisters the given registration.
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <returns>
        /// True if the deregistration was successful (i.e. the application was found).
        /// </returns>
        internal bool Deregister(ApplicationRegistration registration)
        {
            lock (this.registeredApplications)
            {
                return this.registeredApplications.Remove(registration.Name);
            }
        }
    }
}
