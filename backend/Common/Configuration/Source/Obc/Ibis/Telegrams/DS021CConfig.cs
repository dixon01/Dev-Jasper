// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021CConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS021c telegram configuration.
    /// IMPORTANT: in some places (especially legacy code) this is still called HPW021b.
    /// </summary>
    [Serializable]
    public class DS021CConfig : TelegramConfigBase
    {
        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Transmits the route path information to route course indicator. " + TelegramConfigBase.TxtEvent;
            }

            set
            {
            }
        }
    }
}