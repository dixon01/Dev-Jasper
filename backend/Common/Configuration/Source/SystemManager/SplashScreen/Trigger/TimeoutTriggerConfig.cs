// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeoutTriggerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeoutTriggerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Trigger to hide the splash screen after a given timeout.
    /// </summary>
    [Serializable]
    public class TimeoutTriggerConfig : SplashScreenTriggerConfigBase
    {
        /// <summary>
        /// Gets or sets the timeout delay.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets or sets the launch delay as an XML compatible string.
        /// </summary>
        [XmlAttribute("Delay", DataType = "duration")]
        public string DelayString
        {
            get
            {
                return XmlConvert.ToString(this.Delay);
            }

            set
            {
                this.Delay = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}