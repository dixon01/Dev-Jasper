// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetResourceMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetResourceMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.GetResource"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class GetResourceMessage : ResourceMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an incomplete resource information is allowed.
        /// </summary>
        public bool AllowIncomplete { get; set; }
    }
}