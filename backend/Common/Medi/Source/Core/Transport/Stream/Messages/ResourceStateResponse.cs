// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStateResponse.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStateResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream.Messages
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// The response to a <see cref="ResourceStateRequest"/>.
    /// </summary>
    public class ResourceStateResponse : IInternalMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets the resource status.
        /// </summary>
        public ResourceStatus ResourceStatus { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("ResourceStateResponse[{0}, {1}]", this.Id, this.ResourceStatus);
        }
    }
}