// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryServerCompletedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Arguments that are passed along with the SntpClient.QueryServerCompleted event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    using System;

    /// <summary>
    /// Arguments that are passed along with the <see cref="SntpClient.QueryServerCompleted"/> event.
    /// </summary>
    public class QueryServerCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryServerCompletedEventArgs"/> class.
        /// </summary>
        internal QueryServerCompletedEventArgs()
        {
            this.ErrorData = new ErrorData();
        }

        /// <summary>
        /// Gets the data that was returned by the server.
        /// </summary>
        public SntpData Data
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets data relating to any error that occurred.
        /// </summary>
        public ErrorData ErrorData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the server query completed successfully.
        /// NB: It is possible that other errors occurred (not related to the querying of the server) after the query,
        /// so ErrorData should still be examined regardless of the value of this property.
        /// </summary>
        public bool Succeeded
        {
            get;
            internal set;
        }
    }
}
