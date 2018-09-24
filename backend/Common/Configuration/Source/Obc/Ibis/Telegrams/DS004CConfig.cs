// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004CConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004CConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// IBIS <c>DruckName</c> message
    /// </summary>
    [Serializable]
    public class DS004CConfig : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004CConfig"/> class.
        /// </summary>
        public DS004CConfig()
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
                return "Transmits druckName to the Atron ticketing system. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}