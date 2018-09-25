// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompressionType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Logging.NLogExtensions.Targets
{
    using System.IO.Compression;

    /// <summary>
    /// The supported types of compression when archiving a log file.
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// No compression is used.
        /// </summary>
        None,

        /// <summary>
        /// GZIP compression is used.
        /// </summary>
        /// <seealso cref="GZipStream"/>
        GZIP
    }
}