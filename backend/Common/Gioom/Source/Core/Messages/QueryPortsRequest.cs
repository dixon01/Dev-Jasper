// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryPortsRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryPortsRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Requests the list of ports from a remote node.
    /// </summary>
    public class QueryPortsRequest
    {
        /// <summary>
        /// Gets or sets the request id which is used to identify responses for a given request.
        /// <seealso cref="QueryPortsResponse.RequestId"/>
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Gets or sets the name of the port.
        /// This can be null or empty to tell the receiver of this message
        /// that we want to get all ports on that node.
        /// Otherwise only the port with the given name is sent back (or nothing if
        /// the port doesn't exist).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("QueryPortsRequest[{0},'{1}']", this.RequestId, this.Name);
        }
    }
}
