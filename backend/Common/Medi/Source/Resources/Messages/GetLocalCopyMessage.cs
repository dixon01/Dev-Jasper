// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLocalCopyMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetLocalCopyMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.CheckoutResource"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class GetLocalCopyMessage : ResourceMessage
    {
        /// <summary>
        /// Gets or sets resource info.
        /// </summary>
        public ResourceInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the local file name.
        /// </summary>
        public string LocalFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource service should
        /// continue tracking the copied file.
        /// </summary>
        public bool KeepTracking { get; set; }
    }
}