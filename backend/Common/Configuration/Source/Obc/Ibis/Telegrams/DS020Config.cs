// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS020Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS020Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS020 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS020Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS020Config"/> class.
        /// </summary>
        public DS020Config()
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
                return "Asks for the status of configured displays. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}