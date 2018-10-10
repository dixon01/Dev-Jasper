// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS036Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS036Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS036 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS036Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS036Config"/> class.
        /// </summary>
        public DS036Config()
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
                return "Transmits an announcement number. " + TelegramConfigBase.TxtEvent;
            }

            set
            {
            }
        }
    }
}