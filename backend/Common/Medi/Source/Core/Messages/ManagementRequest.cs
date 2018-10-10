// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementRequest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    public class ManagementRequest
    {
        /// <summary>
        /// Gets or sets the ID of this request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the requested path.
        /// </summary>
        public string[] Path { get; set; }
    }
}
