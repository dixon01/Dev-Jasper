// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitGroup.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitGroup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The representation of a group of units.
    /// </summary>
    public class UnitGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitGroup"/> class.
        /// </summary>
        public UnitGroup()
        {
            this.Units = new List<Unit>();
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
        /// Gets or sets the root of the current directory structure expected
        /// on this unit group.
        /// </summary>
        public DirectoryNode CurrentDirectoryStructure { get; set; }

        /// <summary>
        /// Gets or sets the list of units belonging to this group.
        /// A unit can only belong to a single group.
        /// </summary>
        public List<Unit> Units { get; set; }
    }
}