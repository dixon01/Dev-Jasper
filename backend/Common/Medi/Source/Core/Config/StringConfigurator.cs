// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringConfigurator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// An <see cref="IConfigurator"/> implementation that uses a given XML string
    /// to configure the <see cref="MessageDispatcher"/>.
    /// </summary>
    public class StringConfigurator : IConfigurator
    {
        private readonly string configXmlString;
        private readonly string unitName;
        private readonly string applicationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringConfigurator"/> class.
        /// </summary>
        /// <param name="configXmlString">
        /// The config XML string.
        /// </param>
        public StringConfigurator(string configXmlString)
            : this(configXmlString, ApplicationHelper.MachineName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringConfigurator"/> class.
        /// </summary>
        /// <param name="configXmlString">
        /// The config XML string.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        public StringConfigurator(string configXmlString, string unitName)
            : this(configXmlString, unitName, ApplicationHelper.GetEntryAssemblyName())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringConfigurator"/> class.
        /// </summary>
        /// <param name="configXmlString">
        /// The config XML string.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        public StringConfigurator(string configXmlString, string unitName, string applicationName)
        {
            this.configXmlString = configXmlString;
            this.unitName = unitName;
            this.applicationName = applicationName;
        }

        /// <summary>
        /// Creates the local Medi address using the unit and application name provided
        /// in the constructor.
        /// </summary>
        /// <returns>
        /// the local Medi address.
        /// </returns>
        public MediAddress CreateLocalAddress()
        {
            return new MediAddress { Unit = this.unitName, Application = this.applicationName };
        }

        /// <summary>
        /// Creates a config object by de-serializing it from the XML string provided in the constructor.
        /// </summary>
        /// <returns>
        /// the config object.
        /// </returns>
        public MediConfig CreateConfig()
        {
            var serializer = new XmlSerializer(typeof(MediConfig));
            using (TextReader reader = new StringReader(this.configXmlString))
            {
                return serializer.Deserialize(reader) as MediConfig;
            }
        }
    }
}
