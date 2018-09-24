// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedMessagePersistance.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.GorbaProtocol
{
    using System;

    /// <summary>
    /// A feed message received by protran.
    /// </summary>
    [Serializable]
    public class FeedMessagePersistance
    {
        /// <summary>
        /// Gets or sets the last feed message which is stored so it can be resend on startup
        /// </summary>
        public string FeedMessage { get; set; }
    }
}