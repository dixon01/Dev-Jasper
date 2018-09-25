// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS010JConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS010JConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS010j telegram configuration.
    /// IMPORTANT: in some places (especially legacy code) this is still called HPW010b.
    /// </summary>
    [Serializable]
    public class DS010JConfig : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS010JConfig"/> class.
        /// </summary>
        public DS010JConfig()
        {
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
                return
                    "Transmits the current stop index. "
                    ////+ "Attention: Because structure of this telegram is the same like telegram DS010, "
                    ////+ "it's not allowed to use DSHPW010_1 and DS010 in the same vehicle. "
                    + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}