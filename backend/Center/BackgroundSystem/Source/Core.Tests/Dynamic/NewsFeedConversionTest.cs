// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewsFeedConversionTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Dynamic
{
    using System;
    using System.Linq;
    using System.ServiceModel.Syndication;

    using Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The news feed conversion test.
    /// </summary>
    [TestClass]
    public class NewsFeedConversionTest
    {
        /// <summary>
        /// Tests the conversion from a <see cref="SyndicationFeed"/> to a <see cref="NewsFeed"/>.
        /// </summary>
        [TestMethod]
        public void TestConversion()
        {
            var feed = new SyndicationFeed { Title = new TextSyndicationContent("My feed") };
            feed.Items = new[]
                             {
                                 new SyndicationItem(
                                     "Item1",
                                     "Content1",
                                     new Uri("http://localhost"),
                                     "Id1",
                                     DateTimeOffset.UtcNow)
                             };
            var newsFeed = feed.ToNewsFeedUpdate();
            Assert.AreEqual("My feed", newsFeed.Title);
            Assert.AreEqual(1, newsFeed.Items.Count);
            var first = newsFeed.Items.First();
            Assert.AreEqual("Id1", first.Id);
            Assert.AreEqual("Item1", first.Title);
        }
    }
}