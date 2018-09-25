// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputHandlingConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputHandlingConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.IO
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Configuration object to tell what to do with an input.
    /// </summary>
    [Serializable]
    public class InputHandlingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputHandlingConfig"/> class.
        /// </summary>
        public InputHandlingConfig()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the name of the unit where the I/O resides.
        /// If this is left empty or null, the local unit is used.
        /// </summary>
        [XmlAttribute]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the name of the application where the I/O resides.
        /// If this is left empty or null, all applications on the configured
        /// <see cref="Unit"/> are queried for the I/O.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the name of the I/O to use.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the transformation reference.
        /// This must match <see cref="Chain.Id"/> of a configured transformation.
        /// </summary>
        [XmlAttribute]
        public string TransfRef { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this input handling is enabled.
        /// Default value is true.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets what generic coordinate is filled with the value received from the I/O.
        /// </summary>
        public GenericUsage UsedFor { get; set; }
    }
}