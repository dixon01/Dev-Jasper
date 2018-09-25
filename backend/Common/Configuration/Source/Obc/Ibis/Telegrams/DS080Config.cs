// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS080Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS080Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS080 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS080Config : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS080Config"/> class.
        /// </summary>
        public DS080Config()
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
                return "Indicates that the door is opened. " + TelegramConfigBase.TxtEvent;
            }

            set
            {
            }
        }
    }
}