// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Definitions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Definitions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP
{
    using System;

    /// <summary>
    /// Global definitions for IBIS-IP.
    /// </summary>
    public static class Definitions
    {
        /// <summary>
        /// The current version (1.0).
        /// </summary>
        public static readonly Version CurrentVersion = new Version(1, 0);

        /// <summary>
        /// The IBIS_IP over HTTP protocol name (<c>_ibisip_http._tcp</c>).
        /// </summary>
        public static readonly string HttpProtocol = "_ibisip_http._tcp";

        /// <summary>
        /// The IBIS_IP over UDP protocol name (<c>_ibisip_udp._udp</c>).
        /// </summary>
        public static readonly string UdpProtocol = "_ibisip_udp._udp";

        /// <summary>
        /// The <c>path</c> attribute name.
        /// </summary>
        public static readonly string PathAttribute = "path";

        /// <summary>
        /// The <c>ver</c> attribute name.
        /// </summary>
        public static readonly string VersionAttribute = "ver";

        /// <summary>
        /// The <c>multicast</c> attribute name.
        /// </summary>
        public static readonly string MulticastAttribute = "multicast";

        /// <summary>
        /// The <c>sntp-server</c> attribute name.
        /// </summary>
        public static readonly string SntpServerAttribute = "sntp-server";
    }
}
