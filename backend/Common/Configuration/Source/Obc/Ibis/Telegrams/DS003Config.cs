// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS003Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The DS003Config telegram configuration.
    /// </summary>
    [Serializable]
    public class DS003Config : TelegramConfigBase
    {
        private int destinationSize = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS003Config"/> class.
        /// </summary>
        public DS003Config()
        {
            this.Enabled = true;
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
                return "Transmits the destination number with the given number of characters (DestinationSize). "
                       + TelegramConfigBase.TxtCycleAndChange;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the destination size.
        /// </summary>
        public int DestinationSize
        {
            get
            {
                return this.destinationSize;
            }

            set
            {
                if (value < 2 && value > 8)
                {
                    throw new ArgumentNullException("value", "Value has to be between 2 and 8.");
                }

                this.destinationSize = value;
            }
        }
    }
}