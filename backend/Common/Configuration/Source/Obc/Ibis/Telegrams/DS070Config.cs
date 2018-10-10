// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS070Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS070 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS070Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS070Config"/> class.
        /// </summary>
        public DS070Config()
        {
            this.Enabled = true;
            this.RepeatInterval = 10;
            this.Threshold = 3;
        }

        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Asks for the status of configured ticket canceler. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        [XmlElement]
        public int Threshold { get; set; }
    }
}