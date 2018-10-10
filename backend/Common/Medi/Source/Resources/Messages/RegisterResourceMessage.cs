// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterResourceMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegisterResourceMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.RegisterResource"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class RegisterResourceMessage : ResourceMessage
    {
        /// <summary>
        /// Gets or sets LocalFile.
        /// </summary>
        public string LocalFile { get; set; }

        /// <summary>
        /// Gets or sets the source of the resource (not the actual Medi message).
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DeleteLocal.
        /// </summary>
        public bool DeleteLocal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is only temporary.
        /// </summary>
        public bool Temporary { get; set; }
    }
}