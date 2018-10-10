// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS037Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS037Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS037-1 telegram configuration.
    /// </summary>
    [Serializable]
    public class DS037Config : TelegramConfigBase
    {
        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public override string Description
        {
            get
            {
                return "Transmits the speaker volume to the TTS System. Volume 0..100%. " + TelegramConfigBase.TxtEvent;
            }

            set
            {
            }
        }
    }
}