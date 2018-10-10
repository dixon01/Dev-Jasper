// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS002Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS002Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS002Config telegram config.
    /// </summary>
    [Serializable]
    public class DS002Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS002Config"/> class.
        /// </summary>
        public DS002Config()
        {
            this.Enabled = true;
            this.RepeatInterval = 10;
        }

        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Transmits the trip number. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}