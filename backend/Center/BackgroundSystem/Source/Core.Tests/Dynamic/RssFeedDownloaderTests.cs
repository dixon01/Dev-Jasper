// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssFeedDownloaderTests.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Dynamic
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed;
    using Gorba.Center.Common.ServiceModel;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the <see cref="RssFeedDownloader"/>.
    /// </summary>
    [TestClass]
    public class RssFeedDownloaderTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            HttpMessageHandlerFactory.ResetCurrent();
        }

        /// <summary>
        /// Cleans the test up.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            HttpMessageHandlerFactory.ResetCurrent();
        }

        /// <summary>
        /// Tests feed for RSS 1.0 format.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Dynamic\Rss10.txt")]
        public void TestRss10()
        {
            var downloader = new RssFeedDownloader();
            this.SetHttpMessageHandlerFactory("Rss10.txt");
            var feed = downloader.DownloadAsync(new Uri("http://localhost")).Result;
            Assert.IsNotNull(feed);
            Assert.AreEqual("Example Feed", feed.Title);
            Assert.AreEqual(1, feed.Items.Count);
            var title = feed.Items[0].Title;
            Assert.AreEqual("The Example Item with special chars öäü", title);
        }

        /// <summary>
        /// Tests feed for RSS 2.0 format.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Dynamic\Rss20.txt")]
        public void TestRss20()
        {
            var downloader = new RssFeedDownloader();
            this.SetHttpMessageHandlerFactory("Rss20.txt");
            var feed = downloader.DownloadAsync(new Uri("http://localhost")).Result;
            Assert.IsNotNull(feed);
            Assert.AreEqual("Die Nachrichten des Bayerischen Rundfunks   ", feed.Title);
            Assert.AreEqual(11, feed.Items.Count);
            var title = feed.Items[3].Title;
            Assert.AreEqual("AfD-Gründer Lucke will heute seinen Austritt aus der Partei  vollziehen", title);
        }

        /// <summary>
        /// Tests feed for DAB format.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Dynamic\Dab.txt")]
        public void TestDab()
        {
            var downloader = new RssFeedDownloader();
            this.SetHttpMessageHandlerFactory("Dab.txt");
            var feed = downloader.DownloadAsync(new Uri("http://localhost")).Result;
            Assert.IsNotNull(feed);
            Assert.AreEqual("Die Nachrichten des Bayerischen Rundfunks", feed.Title);
            Assert.AreEqual(11, feed.Items.Count);
            var title = feed.Items[2].Title;

            // test special chars
            Assert.AreEqual(@"""Batman""-Amokläufer Holmes wegen Mordes schuldig gesprochen", title);

            // test weather news
            title = feed.Items[10].Title;
            Assert.AreEqual(@"Das Wetter in Bayern: Überwiegend sonnig, Höchstwerte 32 bis 36 Grad", title);
        }

        private void SetHttpMessageHandlerFactory(string fileName)
        {
            var content = this.GetContent(fileName);
            var factory = new TestHttpMessageHandlerFactory(content);
            HttpMessageHandlerFactory.SetCurrent(factory);
        }

        private string GetContent(string name)
        {
            var fileName = Path.Combine(this.TestContext.DeploymentDirectory, name);
            using (var stream = File.OpenText(fileName))
            {
                return stream.ReadToEnd();
            }
        }

        private class TestHttpMessageHandlerFactory : HttpMessageHandlerFactory
        {
            private readonly string content;

            public TestHttpMessageHandlerFactory(string content)
            {
                this.content = content;
            }

            public override HttpMessageHandler Create()
            {
                return new TestHttpMessageHandler(this.content);
            }

            private class TestHttpMessageHandler : HttpMessageHandler
            {
                private readonly string content;

                public TestHttpMessageHandler(string content)
                {
                    this.content = content;
                }

                protected override Task<HttpResponseMessage> SendAsync(
                    HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                                       {
                                           Content =
                                               new StringContent(this.content)
                                       };
                    var result = new TaskCompletionSource<HttpResponseMessage>();
                    result.SetResult(response);
                    return result.Task;
                }
            }
        }
    }
}