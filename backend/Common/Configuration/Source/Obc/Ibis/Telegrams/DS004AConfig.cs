// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004AConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004AConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// IBIS Razzia message
    /// </summary>
    [Serializable]
    public class DS004AConfig : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004AConfig"/> class.
        /// </summary>
        public DS004AConfig()
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
                return "Transmits Razzia Start/Stop messages to the ticketing system. "
                       + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}