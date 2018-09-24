// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;

    /// <summary>
    /// The splash screen configuration.
    /// </summary>
    [Serializable]
    public class SplashScreenConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenConfig"/> class.
        /// </summary>
        public SplashScreenConfig()
        {
            this.ShowOn = new List<SplashScreenTriggerConfigBase>();
            this.HideOn = new List<SplashScreenTriggerConfigBase>();
            this.Items = new List<SplashScreenItemBase>();

            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this splash screen is enabled.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        [XmlAttribute]
        public string Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        [XmlAttribute]
        public string Background { get; set; }

        /// <summary>
        /// Gets or sets the triggers when to show this splash screen.
        /// </summary>
        [XmlArrayItem("SystemBoot", typeof(SystemBootTriggerConfig))]
        [XmlArrayItem("SystemShutdown", typeof(SystemShutdownTriggerConfig))]
        [XmlArrayItem("ApplicationStateChange", typeof(ApplicationStateChangeTriggerConfig))]
        [XmlArrayItem("Input", typeof(InputTriggerConfig))]
        [XmlArrayItem("HotKey", typeof(HotKeyTriggerConfig))]
        public List<SplashScreenTriggerConfigBase> ShowOn { get; set; }

        /// <summary>
        /// Gets or sets the triggers when to hide this splash screen.
        /// </summary>
        [XmlArrayItem("SystemShutdown", typeof(SystemShutdownTriggerConfig))]
        [XmlArrayItem("ApplicationStateChange", typeof(ApplicationStateChangeTriggerConfig))]
        [XmlArrayItem("Input", typeof(InputTriggerConfig))]
        [XmlArrayItem("Timeout", typeof(TimeoutTriggerConfig))]
        [XmlArrayItem("HotKey", typeof(HotKeyTriggerConfig))]
        public List<SplashScreenTriggerConfigBase> HideOn { get; set; }

        /// <summary>
        /// Gets or sets the items to show on the screen.
        /// </summary>
        [XmlArrayItem("Logo", typeof(LogoSplashScreenItem))]
        [XmlArrayItem("System", typeof(SystemSplashScreenItem))]
        [XmlArrayItem("Network", typeof(NetworkSplashScreenItem))]
        [XmlArrayItem("Applications", typeof(ApplicationsSplashScreenItem))]
        [XmlArrayItem("IO", typeof(GioomSplashScreenItem))]
        [XmlArrayItem("Management", typeof(ManagementSplashScreenItem))]
        public List<SplashScreenItemBase> Items { get; set; }
    }
}
