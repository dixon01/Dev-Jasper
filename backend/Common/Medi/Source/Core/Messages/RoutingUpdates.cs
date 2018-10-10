// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingUpdates.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RoutingUpdates type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Message containing one or more <see cref="RouteUpdate"/>s.
    /// </summary>
    public class RoutingUpdates : INetworkMessage
    {
        /// <summary>
        /// Gets or sets the updates that are communicated with this message.
        /// </summary>
        public List<RouteUpdate> Updates { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.GetType().Name).Append("[");
            foreach (var update in this.Updates)
            {
                sb.Append(update);
                sb.Append(';');
            }

            sb.Length--;
            sb.Append(']');
            return sb.ToString();
        }
    }
}
