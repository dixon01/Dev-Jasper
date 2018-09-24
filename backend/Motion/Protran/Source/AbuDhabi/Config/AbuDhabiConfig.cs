// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Container of all the objects that compose
    /// the Abu Dhabi configuration file.
    /// </summary>
    [XmlRoot("AbuDhabi")]
    [Serializable]
    public class AbuDhabiConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiConfig"/> class.
        /// Container of all the settings required by Protran to deal with the
        /// Abu Dhabi protocol.
        /// </summary>
        public AbuDhabiConfig()
        {
            this.Behaviour = new BehaviourConfig();
            this.Cu5 = new Cu5Config();
            this.Isi = new IsiConfig();
            this.IsiSimulation = new IsiSimulationConfig();
            this.Ism = new IsmConfig();
            this.Ibis = new IbisFallbackConfig();
            this.Transformations = new List<Chain>();
            this.Subscriptions = new List<Subscription>();
        }

        /// <summary>
        /// Gets or sets the XML element called <code>Behaviour</code>.
        /// </summary>
        public BehaviourConfig Behaviour { get; set; }

        /// <summary>
        /// Gets or sets the XML element called CU5.
        /// </summary>
        [XmlElement("CU5")]
        public Cu5Config Cu5 { get; set; }

        /// <summary>
        /// Gets or sets the XML element called ISI.
        /// </summary>
        [XmlElement("ISI")]
        public IsiConfig Isi { get; set; }

        /// <summary>
        /// Gets or sets the XML element called ISISimulation.
        /// </summary>
        [XmlElement("ISISimulation")]
        public IsiSimulationConfig IsiSimulation { get; set; }

        /// <summary>
        /// Gets or sets the XML element called ISM.
        /// </summary>
        [XmlElement("ISM")]
        public IsmConfig Ism { get; set; }

        /// <summary>
        /// Gets or sets the XML element called IBIS.
        /// </summary>
        [XmlElement("IBIS")]
        public IbisFallbackConfig Ibis { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Subscriptions.
        /// </summary>
        public List<Subscription> Subscriptions { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Transformations.
        /// </summary>
        public List<Chain> Transformations { get; set; }
    }
}
