// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS130Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS130Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for DS130 status response telegrams.
    /// </summary>
    [Serializable]
    public class DS130Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS130Config"/> class.
        /// </summary>
        public DS130Config()
        {
            this.Responses = new List<Response>();
        }

        /// <summary>
        /// Gets or sets the set of all the responses.
        /// </summary>
        [XmlElement("Response")]
        public List<Response> Responses { get; set; }

        /// <summary>
        /// Gets or sets the XML element called DefaultResponse.
        /// </summary>
        public int DefaultResponse { get; set; }
    }
}
