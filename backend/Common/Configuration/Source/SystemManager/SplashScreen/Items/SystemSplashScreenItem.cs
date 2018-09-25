// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemSplashScreenItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Show information about the system on the splash screen.
    /// </summary>
    [Serializable]
    public class SystemSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show the machine name of this PC.
        /// </summary>
        [XmlAttribute]
        public bool MachineName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the overall RAM use of the system.
        /// </summary>
        [XmlAttribute]
        public bool Ram { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the overall CPU use of the system.
        /// </summary>
        [XmlAttribute]
        public bool Cpu { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the uptime of the system.
        /// </summary>
        [XmlAttribute]
        public bool Uptime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the serial number of the system.
        /// </summary>
        [XmlAttribute]
        public bool Serial { get; set; }
    }
}