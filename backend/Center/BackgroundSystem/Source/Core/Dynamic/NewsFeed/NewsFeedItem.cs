// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewsFeedItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NewsFeedItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed
{
    using System;

    /// <summary>
    /// Defines a news feed item.
    /// </summary>
    [Serializable]
    public class NewsFeedItem
    {
        /// <summary>
        /// Gets or sets the identifier of the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
    }
}