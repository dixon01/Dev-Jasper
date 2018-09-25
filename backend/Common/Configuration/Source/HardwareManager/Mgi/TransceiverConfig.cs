// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransceiverConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the transceiver
    /// </summary>
    [Serializable]
    public class TransceiverConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransceiverConfig"/> class.
        /// </summary>
        public TransceiverConfig()
        {
            this.Index = -1;
            this.Type = TransceiverType.RS485;
            this.Termination = false;
            this.Mode = TransceiverMode.FullDuplex;
        }

        /// <summary>
        /// Gets or sets the index of the transceiver.
        /// -1 means disabled.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(-1)]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the type config.
        /// </summary>
        public TransceiverType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether termination is active.
        /// </summary>
        public bool Termination { get; set; }

        /// <summary>
        /// Gets or sets the mode config.
        /// </summary>
        public TransceiverMode Mode { get; set; }
    }
}
