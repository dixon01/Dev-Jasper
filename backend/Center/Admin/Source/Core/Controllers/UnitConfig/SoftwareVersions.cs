// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareVersions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoftwareVersions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using System;

    /// <summary>
    /// Static class containing all relevant software versions.
    /// </summary>
    public static class SoftwareVersions
    {
        /// <summary>
        /// The system manager software versions.
        /// </summary>
        public static class SystemManager
        {
            /// <summary>
            /// Medi supports gateways from this version of SystemManager.
            /// </summary>
            public static readonly Version MediSupportsGateways = new Version(2, 5, 1517, 9547);
        }

        /// <summary>
        /// The hardware manager software versions.
        /// </summary>
        public static class HardwareManager
        {
            /// <summary>
            /// HardwareManager has auto brightness parameters in its config file.
            /// </summary>
            public static readonly Version AutoBrightnessParameters = new Version(1, 5, 1508, 9178);

            /// <summary>
            /// HardwareManager supports DHCP in its config file.
            /// </summary>
            public static readonly Version SupportsDhcp = new Version(1, 5, 1508, 9178);

            /// <summary>
            /// HardwareManager supports DNS servers in its config file.
            /// </summary>
            public static readonly Version SupportsDnsServers = new Version(1, 5, 1511, 9294);
        }

        /// <summary>
        /// The update software versions.
        /// </summary>
        public static class Update
        {
            /// <summary>
            /// Update supports receiving updates through Azure.
            /// </summary>
            public static readonly Version SupportsAzureUpdateClient = new Version(2, 5, 1525, 9806);
        }

        /// <summary>
        /// The Protran software versions.
        /// </summary>
        public static class Protran
        {
            /// <summary>
            /// Protran supports the GO007 telegram.
            /// </summary>
            public static readonly Version SupportsGO007 = new Version(2, 5, 1515, 9501);
        }

        /// <summary>
        /// The Infomedia software versions.
        /// </summary>
        public static class Infomedia
        {
            /// <summary>
            /// Infomedia presentation files can be updated without restarting Composer or Renderers.
            /// </summary>
            public static readonly Version WithoutUpdateRestart = new Version(2, 5, 1508, 9166);

            /// <summary>
            /// DirectX Renderer supports pre-loading images at start-up.
            /// </summary>
            public static readonly Version DirectXPreloadImages = new Version(2, 5, 1525, 9883);
        }
    }
}
