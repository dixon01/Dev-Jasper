// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PassengerCountConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PassengerCountConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The passenger counting configuration.
    /// </summary>
    [Serializable]
    public class PassengerCountConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassengerCountConfig"/> class.
        /// </summary>
        public PassengerCountConfig()
        {
            this.DoorAddresses = new List<string>();
            this.Description = "The IBIS addresses of Passenger count units. Model is 'None' or 'Iris'. "
                          + "Addresses are sequential: adress #2 means door #2. "
                          + "Use ; to separate cell numbers for a single door";
        }

        /// <summary>
        /// Gets or sets the counting model.
        /// </summary>
        public PassengerCountingModel Model { get; set; }

        /// <summary>
        /// Gets or sets the list of door addresses.
        /// One entry is for one door; if a door has multiple addresses, they need to be separated by semicolon.
        /// </summary>
        [XmlElement(ElementName = "IBIS_Address")]
        public List<string> DoorAddresses { get; set; }

        /// <summary>
        /// Gets the number of doors.
        /// </summary>
        [XmlIgnore]
        public int NbDoors
        {
            get
            {
                return this.DoorAddresses.Count;
            }
        }

        /// <summary>
        /// Gets or sets the description shown in the XML config file.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Creates the default config.
        /// </summary>
        public void CreateDefaultConfig()
        {
            this.Model = PassengerCountingModel.None;
            this.DoorAddresses.Clear();
            this.DoorAddresses.Add(string.Empty);
            this.DoorAddresses.Add(string.Empty);
        }

        /// <summary>
        /// Return the cells associated with that door index (0 based)
        /// </summary>
        /// <param name="door">The door for which the cells are required</param>
        /// <returns>The list of cells.</returns>
        public IEnumerable<int> Cells(int door)
        {
            List<int> ret = new List<int>();

            string add = this.DoorAddresses[door];

            if (add == string.Empty || add == "-1")
            {
                return ret;
            }

            foreach (var part in add.Split(';'))
            {
                ret.Add(int.Parse(part.Trim()));
            }

            return ret;
        }
    }
}