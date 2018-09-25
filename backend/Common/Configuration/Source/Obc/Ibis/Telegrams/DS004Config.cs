// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS004Config telegram configuration.
    /// </summary>
    [Serializable]
    public class DS004Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004Config"/> class.
        /// </summary>
        public DS004Config()
        {
            this.RepeatInterval = 10;
            this.Enabled = false;
            this.DigitCount = 7;
        }

        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Transmits the characteristics for the ticket canceler. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets number of digits to be sent for DS004. Default is 7.
        /// </summary>
        public int DigitCount { get; set; }
    }
}