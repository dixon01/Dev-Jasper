// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004BConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004BConfig type.
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
    public class DS004BConfig : TelegramConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004BConfig"/> class.
        /// </summary>
        public DS004BConfig()
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
                return "Transmits didok to the Atron ticketing system. " + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }
    }
}