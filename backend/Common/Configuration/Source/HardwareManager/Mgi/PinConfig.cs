// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PinConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a pin on MGI topbox
    /// </summary>
    [Serializable]
    public class PinConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PinConfig"/> class.
        /// </summary>
        public PinConfig()
        {
            this.Index = -1;
        }

        /// <summary>
        /// Gets or sets the name of the GIOoM port for this pin.
        /// </summary>
        [XmlText]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the index of the pin.
        /// -1 means disabled.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(-1)]
        public int Index { get; set; }
    }
}
