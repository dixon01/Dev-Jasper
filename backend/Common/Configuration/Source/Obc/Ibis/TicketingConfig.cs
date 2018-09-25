// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicketingConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TicketingConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The ticketing configuration.
    /// </summary>
    [Serializable]
    public class TicketingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketingConfig"/> class.
        /// </summary>
        public TicketingConfig()
        {
            this.Addresses = new List<int>();
            this.Description = "The IBIS addresses of Ticketing units. Model is 'None', 'Krauth' or 'Atron'. "
                          + "Maximum 2 ticketing units are allowed for Krauth and 3 for Atron. "
                          + "Have a look to DS070. Addresses are sequential : adress #2 means device #2";
        }

        /// <summary>
        /// Gets or sets the ticketing model.
        /// </summary>
        public TicketingModel Model { get; set; }

        /// <summary>
        /// Gets or sets the list of IBIS addresses.
        /// </summary>
        [XmlElement(ElementName = "IBIS_Address")]
        public List<int> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the description shown in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Creates the default config.
        /// </summary>
        public void CreateDefaultConfig()
        {
            this.Model = TicketingModel.None;
            this.Addresses.Clear();
            this.Addresses.Add(-1);
            this.Addresses.Add(-1);
        }
    }
}