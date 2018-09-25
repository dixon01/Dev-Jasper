// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSessionId.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamSessionId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Session id implementation for stream transports.
    /// This class simply uses an integer as identification.
    /// </summary>
    internal class StreamSessionId : ISessionId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamSessionId"/> class.
        /// </summary>
        /// <param name="id">
        /// The integer id.
        /// </param>
        /// <param name="remoteName">
        /// The name of the remote party (usually similar to its MediAddress).
        /// </param>
        public StreamSessionId(int id, string remoteName)
        {
            this.Id = id;
            this.RemoteName = remoteName;
        }

        /// <summary>
        /// Gets the integer id of this session id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the name of the remote party (usually similar to its MediAddress).
        /// </summary>
        public string RemoteName { get; private set; }

        /// <summary>
        /// Checks if the other session id is the same as this object.
        /// </summary>
        /// <param name="id">
        /// The other.
        /// </param>
        /// <returns>
        /// True if the session ids are the same.
        /// </returns>
        public bool Equals(ISessionId id)
        {
            var other = id as StreamSessionId;
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.Id == other.Id && this.RemoteName.Equals(other.RemoteName);
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0:X8}@{1}]", this.Id, this.RemoteName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ISessionId);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.RemoteName.GetHashCode();
        }
    }
}
