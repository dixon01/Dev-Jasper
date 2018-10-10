// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubscriptionMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISubscriptionMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    /// <summary>
    /// Interface implemented by all internal messages handling subscriptions.
    /// </summary>
    internal interface ISubscriptionMessage
    {
        /// <summary>
        /// Gets or sets the type names.
        /// </summary>
        string[] Types { get; set; }
    }
}