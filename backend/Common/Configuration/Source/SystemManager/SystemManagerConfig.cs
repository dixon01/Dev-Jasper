// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The root of the system manager configuration.
    /// </summary>
    [XmlRoot("SystemManager")]
    [Serializable]
    public class SystemManagerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagerConfig"/> class.
        /// </summary>
        public SystemManagerConfig()
        {
            this.Defaults = new SystemManagerDefaults();
            this.SplashScreens = new SplashScreenConfigList();
            this.System = new SystemConfig();
            this.Applications = new List<ApplicationConfigBase>();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(SystemManagerConfig).Assembly.GetManifestResourceStream(
                            typeof(SystemManagerConfig), "SystemManager.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find SystemManager.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the default values.
        /// </summary>
        public SystemManagerDefaults Defaults { get; set; }

        /// <summary>
        /// Gets or sets the splash screens.
        /// </summary>
        public SplashScreenConfigList SplashScreens { get; set; }

        /// <summary>
        /// Gets or sets the system configuration.
        /// </summary>
        public SystemConfig System { get; set; }

        /// <summary>
        /// Gets or sets the applications to launch and observe.
        /// </summary>
        [XmlArrayItem("Process", typeof(ProcessConfig))]
        [XmlArrayItem("Component", typeof(ComponentConfig))]
        public List<ApplicationConfigBase> Applications { get; set; }
    }
}
