// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStreamConnection.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStreamConnection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream connection to a remote party (either client or server).
    /// </summary>
    internal interface IStreamConnection : IDisposable
    {
        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// Creates a more or less unique ID for this connection.
        /// </summary>
        /// <returns>
        /// The pseudo-unique ID.
        /// </returns>
        int CreateId();
    }
}