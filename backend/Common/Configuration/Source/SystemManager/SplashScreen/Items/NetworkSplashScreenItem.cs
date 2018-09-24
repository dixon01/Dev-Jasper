// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NetworkSplashScreenItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Show information about the network configuration on the splash screen.
    /// </summary>
    [Serializable]
    public class NetworkSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSplashScreenItem"/> class.
        /// </summary>
        public NetworkSplashScreenItem()
        {
            this.StatusFilter = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the name of the network interface.
        /// </summary>
        [XmlAttribute]
        public bool Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the IP address of the network interface.
        /// </summary>
        [XmlAttribute]
        public bool Ip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the IP gateway address of the network interface.
        /// </summary>
        [XmlAttribute]
        public bool Gateway { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the MAC address of the network interface.
        /// </summary>
        [XmlAttribute]
        public bool Mac { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the status of the network interface.
        /// </summary>
        [XmlAttribute]
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the status filter telling which network interfaces should be shown.
        /// By default this property is empty, meaning all network interfaces are shown.
        /// This property can be set to a comma separated list of status values to be shown.
        /// Possible values are: Down, Up, NotPresent, Unknown, Dormant, LowerLayerDown, Testing
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string StatusFilter { get; set; }
    }
}