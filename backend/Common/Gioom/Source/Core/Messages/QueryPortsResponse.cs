// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryPortsResponse.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryPortsResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Response to a <see cref="QueryPortsRequest"/>.
    /// </summary>
    public class QueryPortsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryPortsResponse"/> class.
        /// </summary>
        public QueryPortsResponse()
        {
            this.Ports = new List<PortInfo>();
        }

        /// <summary>
        /// Gets or sets the request id matching <see cref="QueryPortsRequest.RequestId"/>.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Gets or sets the list of ports matching the request.
        /// </summary>
        public List<PortInfo> Ports { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("QueryPortsResponse[");
            sb.Append(this.RequestId);
            foreach (var port in this.Ports)
            {
                sb.Append(",'").Append(port.Name).Append("'");
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}