// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed
{
    using System.ServiceModel.Syndication;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates a <see cref="NewsFeed"/> instance equivalent to the given <see cref="SyndicationFeed"/>.
        /// </summary>
        /// <param name="feed">The feed to be converted.</param>
        /// <returns>A feed update object equivalent to the given feed.</returns>
        public static NewsFeed ToNewsFeedUpdate(this SyndicationFeed feed)
        {
            var update = new NewsFeed
                             {
                                 Title = feed.Title.Text
                             };
            foreach (var item in feed.Items)
            {
                update.Items.Add(new NewsFeedItem { Id = item.Id, Title = item.Title.Text });
            }

            return update;
        }
    }
}