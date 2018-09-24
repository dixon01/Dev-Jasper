// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOCondition.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOCondition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Condition verifying a digital input for its value.
    /// </summary>
    [Serializable]
    public class IOCondition
    {
        /// <summary>
        /// Gets or sets the unit name where the input can be found.
        /// If this property is null, the input is expected to be on the current unit.
        /// </summary>
        [XmlAttribute]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the application in which the input can be found.
        /// If this property is null, the input is searched in all applications
        /// found on the configured <see cref="Unit"/>.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the name of the input.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the expected value.
        /// </summary>
        [XmlAttribute]
        public int Value { get; set; }
    }
}