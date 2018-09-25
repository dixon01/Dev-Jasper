// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS009Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS009Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS009Config telegram config.
    /// </summary>
    [Serializable]
    public class DS009Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS009Config"/> class.
        /// </summary>
        public DS009Config()
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
                return "Transmits a text to an interior display. Preview next stop. "
                       + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}