// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionAlgorithm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompressionAlgorithm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Common
{
    /// <summary>
    /// The compression algorithm of files within a repository.
    /// </summary>
    public enum CompressionAlgorithm
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The files are not compressed.
        /// </summary>
        None,

        /// <summary>
        /// The files are compressed using the GZIP algorithm (see RFC 1952).
        /// </summary>
        GZIP
        // ReSharper restore InconsistentNaming
    }
}