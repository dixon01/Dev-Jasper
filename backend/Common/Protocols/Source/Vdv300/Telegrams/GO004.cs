// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO004.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO004 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    using System.Xml.Serialization;

    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS GO004 telegram.
    /// </summary>
    public class GO004 : StringArrayTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }

        /// <summary>
        /// Gets or sets the message index.
        /// </summary>
        public int MessageIndex { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// Gets or sets the time range.
        /// The range is a 4 to 8 digit
        /// </summary>
        public int TimeRange { get; set; }

        /// <summary>
        /// Gets or sets the message string parts.
        /// The first message part will go into the
        /// title, the remaining parts into the body
        /// joined with BBCode "[br]".
        /// </summary>
        [XmlIgnore]
        public string[] MessageParts
        {
            get
            {
                return this.Data;
            }

            set
            {
                this.Data = value;
            }
        }
    }
}
