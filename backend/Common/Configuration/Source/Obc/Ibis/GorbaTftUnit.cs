// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaTftUnit.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GorbaTftUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The Gorba TFT IBIS configuration.
    /// </summary>
    [Serializable]
    public class GorbaTftUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaTftUnit"/> class.
        /// </summary>
        public GorbaTftUnit()
        {
            this.Description = "The IBIS addresses of the Gorba TFT units. Have a look to: DS020 and DSHPW021b_1.";
            this.Addresses = new List<int>();
        }

        /// <summary>
        /// Gets or sets the list of IBIS addresses for Gorba TFTs.
        /// If an address is set to -1, it is ignored.
        /// </summary>
        [XmlElement(ElementName = "IBIS_Address")]
        public List<int> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the description of this configuration used in XML.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Creates the default config.
        /// </summary>
        public void CreateDefaultConfig()
        {
            this.Addresses.Clear();
            this.Addresses.Add(-1);
            this.Addresses.Add(-1);
        }

        /// <summary>
        /// Checks if this configuration has a properly configured unit.
        /// </summary>
        /// <returns>
        /// True if there is a valid address, otherwise false.
        /// </returns>
        public bool HasUnit()
        {
            foreach (var address in this.Addresses)
            {
                if (address != -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}