// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizationConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for the update progress visualization.
    /// </summary>
    [Serializable]
    public class VisualizationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizationConfig"/> class.
        /// </summary>
        public VisualizationConfig()
        {
            this.HideTimeout = TimeSpan.FromSeconds(30);
            this.SplashScreen = new SplashScreenVisualizationConfig();
            this.Led = new LedVisualizationConfig();
        }

        /// <summary>
        /// Gets or sets the hide timeout after which any visualization is "hidden".
        /// </summary>
        [XmlIgnore]
        public TimeSpan HideTimeout { get; set; }

        /// <summary>
        /// Gets or sets the hide timeout as an XML serializable string.
        /// </summary>
        [XmlAttribute("HideTimeout")]
        public string HideTimeoutString
        {
            get
            {
                return XmlConvert.ToString(this.HideTimeout);
            }

            set
            {
                this.HideTimeout = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the configuration for the splash screen window.
        /// </summary>
        public SplashScreenVisualizationConfig SplashScreen { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the Update LED on the PC-2.
        /// </summary>
        [XmlElement("LED")]
        public LedVisualizationConfig Led { get; set; }
    }
}