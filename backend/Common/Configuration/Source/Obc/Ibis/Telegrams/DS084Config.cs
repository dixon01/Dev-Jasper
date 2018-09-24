// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS084Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS084Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS084 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS084Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS084Config"/> class.
        /// </summary>
        public DS084Config()
        {
            this.RepeatInterval = 600; // 10mn par specs
        }

        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Request Iris cell status. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}