// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObjectConfigurator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// An <see cref="IConfigurator"/> implementation that uses a given <see cref="MediConfig"/>
    /// to configure the <see cref="MessageDispatcher"/>.
    /// </summary>
    public class ObjectConfigurator : IConfigurator
    {
        private readonly MediConfig config;
        private readonly string unitName;
        private readonly string applicationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConfigurator"/> class.
        /// </summary>
        /// <param name="config">
        /// The config object that will be returned by <see cref="CreateConfig"/>.
        /// </param>
        public ObjectConfigurator(MediConfig config)
            : this(config, ApplicationHelper.MachineName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConfigurator"/> class.
        /// </summary>
        /// <param name="config">
        /// The config object that will be returned by <see cref="CreateConfig"/>.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        public ObjectConfigurator(MediConfig config, string unitName)
            : this(config, unitName, ApplicationHelper.GetEntryAssemblyName())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConfigurator"/> class.
        /// </summary>
        /// <param name="config">
        /// The config object that will be returned by <see cref="CreateConfig"/>.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        public ObjectConfigurator(MediConfig config, string unitName, string applicationName)
        {
            this.config = config;
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
        /// Returns the config object given in the constructor.
        /// </summary>
        /// <returns>
        /// The config object.
        /// </returns>
        public MediConfig CreateConfig()
        {
            return this.config;
        }
    }
}
