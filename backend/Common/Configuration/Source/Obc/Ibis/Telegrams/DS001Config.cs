// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS001Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS001Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System.Xml.Serialization;

    /// <summary>
    /// The DS001Config telegram configuration.
    /// </summary>
    public class DS001Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS001Config"/> class.
        /// </summary>
        public DS001Config()
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
                return "Transmits the line number. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}