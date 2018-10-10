// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestResourceMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RequestResourceMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.RequestFile"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class RequestResourceMessage
    {
        /// <summary>
        /// Gets or sets the full file name (locally on the system from which the resource is requested).
        /// </summary>
        public string FileName { get; set; }
    }
}
