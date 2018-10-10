// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStateRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStateRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream.Messages
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Request for a resource state.
    /// This message will be answered with a <see cref="ResourceStateResponse"/>.
    /// </summary>
    public class ResourceStateRequest : IInternalMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("ResourceStateRequest[{0}]", this.Id);
        }
    }
}