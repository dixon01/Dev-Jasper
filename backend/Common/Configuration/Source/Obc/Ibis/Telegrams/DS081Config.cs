// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS081Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS081Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS081 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS081Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS081Config"/> class.
        /// </summary>
        public DS081Config()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Indicates that the door is closed. " + TelegramConfigBase.TxtEvent;
            }

            set
            {
            }
        }
    }
}