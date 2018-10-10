// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003CConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS003CConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS003CConfig telegram configuration.
    /// </summary>
    [Serializable]
    public class DS003CConfig : TelegramConfigBase
    {
        private int maxTextLength = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS003CConfig"/> class.
        /// </summary>
        public DS003CConfig()
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
                    "Transmits a text to an interior display. max 60 char. "
                    + "The value MaxTextLength defines the fixed length. "
                    + "Value should be a multiple of 4! maximum is 60 characters. "
                    + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the maximum text length.
        /// </summary>
        [XmlElement]
        public int MaxTextLength
        {
            get
            {
                return this.maxTextLength;
            }

            set
            {
                this.maxTextLength = value;
            }
        }
    }
}