// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostnameSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HostnameSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    /// <summary>
    /// The way how the hostname is set.
    /// </summary>
    public enum HostnameSource
    {
        /// <summary>
        /// The hostname is not changed.
        /// </summary>
        None,

        /// <summary>
        /// The hostname is changed to <code>TFT-xx-xx-xx</code> using the MAC address of the first network interface.
        /// </summary>
        MacAddress,

        /// <summary>
        /// The hostname is changed to <code>TFT-xxxxxxx</code> using the serial number of the device.
        /// </summary>
        SerialNumber
    }
}