// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Consts.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Consts type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;

    /// <summary>
    /// Constant values.
    /// </summary>
    internal static class Consts
    {
        /// <summary>
        /// The timeout for subscriptions.
        /// </summary>
        public static readonly TimeSpan SubscriptionTimeout = TimeSpan.FromHours(8);
    }
}