// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnounceResourceMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnnounceResourceMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.BeginAnnounceResource"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class AnnounceResourceMessage : ResourceMessage
    {
        /// <summary>
        /// Gets or sets the source of the resource.
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets the announcement.
        /// </summary>
        public ResourceAnnouncement Announcement { get; set; }
    }
}