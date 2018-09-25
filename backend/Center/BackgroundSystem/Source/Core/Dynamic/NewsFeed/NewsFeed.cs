// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewsFeed.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The news feed update.
    /// </summary>
    [Serializable]
    public class NewsFeed : IEquatable<NewsFeed>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewsFeed"/> class.
        /// </summary>
        public NewsFeed()
        {
            this.Items = new List<NewsFeedItem>();
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public List<NewsFeedItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(NewsFeed other)
        {
            if (!string.Equals(this.Title, other.Title)
                || this.Items.Count != other.Items.Count)
            {
                return false;
            }

            // compare content
            return this.Items.All(item => other.Items.Any(i => i.Id == item.Id && string.Equals(i.Title, item.Title)));
        }
    }
}