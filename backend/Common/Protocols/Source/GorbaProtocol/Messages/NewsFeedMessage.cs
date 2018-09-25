// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewsFeedMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.GorbaProtocol.Messages
{
    using System;

    /// <summary>
    /// The news feed message containing the titles of the news.
    /// </summary>
    [Serializable]
    public class NewsFeedMessage : GorbaMessage<string>
    {
        /// <summary>
        /// Gets or sets the identifier of the feed.
        /// </summary>
        public int FeedId { get; set; }
    }
}