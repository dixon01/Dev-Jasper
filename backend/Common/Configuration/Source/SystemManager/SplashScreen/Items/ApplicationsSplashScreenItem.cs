// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationsSplashScreenItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Show information about all (or the configured, see <see cref="Visibility"/>)
    /// applications and additional information (if enabled).
    /// </summary>
    [Serializable]
    public class ApplicationsSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsSplashScreenItem"/> class.
        /// </summary>
        public ApplicationsSplashScreenItem()
        {
            this.Visibility = new List<ApplicationsSplashScreenVisibilityBase>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the version of the application.
        /// </summary>
        [XmlAttribute]
        public bool Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the state of the application (in color).
        /// </summary>
        [XmlAttribute]
        public bool State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the RAM consumption of the application.
        /// </summary>
        [XmlAttribute]
        public bool Ram { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the CPU consumption of the application.
        /// </summary>
        [XmlAttribute]
        public bool Cpu { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the uptime of the application.
        /// </summary>
        [XmlAttribute]
        public bool Uptime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the last reason why the application was launched.
        /// </summary>
        [XmlAttribute]
        public bool LaunchReason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the last reason why the application exited.
        /// </summary>
        [XmlAttribute]
        public bool ExitReason { get; set; }

        /// <summary>
        /// Gets or sets the visibility.
        /// You shouldn't define both Hide and Show at the same time.
        /// If nothing is defined, all applications are shown.
        /// If one or more "Hide" are defined, then all applications except for the given ones are shown.
        /// If one or more "Show" are defined, then only the given applications are shown.
        /// </summary>
        [XmlElement("Hide", typeof(ApplicationsSplashScreenHide))]
        [XmlElement("Show", typeof(ApplicationsSplashScreenShow))]
        public List<ApplicationsSplashScreenVisibilityBase> Visibility { get; set; }
    }
}