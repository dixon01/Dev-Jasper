// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Unit.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Unit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The representation of a unit that can be updated.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> class.
        /// </summary>
        public Unit()
        {
            this.Updates = new List<UpdateInfo>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a list of all updates that have ever been created
        /// for this unit.
        /// </summary>
        public List<UpdateInfo> Updates { get; set; }
    }
}