// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Arriva
{
    /// <summary>
    /// The type of connection source.
    /// </summary>
    public enum ConnectionSource
    {
        /// <summary>
        /// The connection information is taken from the Arriva protocol (Albatross OBU).
        /// </summary>
        ArrivaProtocol,

        /// <summary>
        /// The connection information is taken from the FTP server drop folder
        /// </summary>
        Ftp
    }
}