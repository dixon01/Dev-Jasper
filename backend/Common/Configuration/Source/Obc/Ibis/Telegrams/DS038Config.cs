// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS038Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS038Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS038-1 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS038Config : TelegramConfigBase
    {
        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Transmits a text to announce via TTS. " + TelegramConfigBase.TxtEvent;
            }

            set
            {
            }
        }
    }
}