// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotKeyTriggerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Trigger to show or hide the splash screen when a hotkey is pressed.
    /// </summary>
    [Serializable]
    public class HotKeyTriggerConfig : SplashScreenTriggerConfigBase
    {
        private string key;

        /// <summary>
        /// Gets or sets the hot key.
        /// </summary>
        [XmlAttribute]
        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length != 1)
                {
                    throw new ArgumentException("Value should be exactly one character");
                }

                this.key = value;
            }
        }
    }
}
