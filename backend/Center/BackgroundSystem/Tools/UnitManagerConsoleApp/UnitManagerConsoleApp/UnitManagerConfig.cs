// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitManagerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines UnitManagerConfig that enables to declare unit addresses used for tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnitManagerConsoleApp
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines UnitManagerConfig that enables to declare unit addresses used for tests
    /// </summary>
    [XmlRoot("UnitManagerConfig", Namespace = "UnitManagerConsoleApp.Config")]
    public class UnitManagerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitManagerConfig"/> class.
        /// Container of all the settings
        /// required by bus terminal to configure the
        /// stop point displays and the overview displays.
        /// </summary>
        public UnitManagerConfig()
        {
            this.UnitAddressList = new List<string>();
        }

        /// <summary>
        /// Gets or sets StopPointReferences.
        /// </summary>
        [XmlArray("UnitAddress")]
        [XmlArrayItem(ElementName = "NetworkAddress")]
        public List<string> UnitAddressList { get; set; }
    }
}
