// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainApplicationWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDomainApplicationWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Internal wrapper for an <see cref="IApplication"/> that is running in a different
    /// AppDomain. This class provides the necessary interface used by
    /// <see cref="ComponentApplicationController"/> to manage an application in an AppDomain.
    /// </summary>
    internal class AppDomainApplicationWrapper : MarshalByRefObject, IApplication
    {
        private readonly Type type;

        private IApplication application;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainApplicationWrapper"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public AppDomainApplicationWrapper(ComponentConfig config)
        {
            var asm = Assembly.LoadFrom(config.LibraryPath);
            this.type = asm.GetType(config.ClassName, true);
        }

        /// <summary>
        /// Configures Medi in the AppDomain of this wrapper.
        /// </summary>
        /// <param name="xmlString">
        /// The XML string.
        /// </param>
        public void ConfigureMedi(string xmlString)
        {
            using (var reader = new StringReader(xmlString))
            {
                var config = (MediConfig)new XmlSerializer(typeof(MediConfig)).Deserialize(reader);
                MessageDispatcher.Instance.Configure(
                    new ObjectConfigurator(
                        config,
                        ApplicationHelper.MachineName,
                        string.Format("AppDomain-{0}", Guid.NewGuid())));
            }
        }

        /// <summary>
        /// Creates the wrapped application and configures it with the given name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Configure(string name)
        {
            this.application = (IApplication)Activator.CreateInstance(this.type);
            this.application.Configure(name);
        }

        /// <summary>
        /// Starts the wrapped application.
        /// </summary>
        public void Start()
        {
            this.application.Start();
        }

        /// <summary>
        /// Stops the wrapped application.
        /// </summary>
        public void Stop()
        {
            this.application.Stop();
        }
    }
}