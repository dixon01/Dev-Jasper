// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssFeedDownloader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// The RSS feed downloader.
    /// </summary>
    public class RssFeedDownloader
    {
        private enum RssFormat
        {
            Unknown = 0,

            Rss10 = 1,

            Rss20 = 2,

            Dab = 3
        }

        /// <summary>
        /// The download.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="NewsFeed"/>.
        /// </returns>
        public async Task<NewsFeed> DownloadAsync(Uri uri)
        {
            string content;
            SyndicationFeedFormatter formatter;
            var handler = HttpMessageHandlerFactory.Current.Create();
            using (var client = new HttpClient(handler))
            {
                content = await client.GetStringAsync(uri);
                var format = DetectFormat(content);
                formatter = GetFormatter(format);
            }

            if (formatter == null)
            {
                throw new Exception("Couldn't download the feed for the given uri");
            }

            using (var stringReader = new StringReader(content))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    formatter.ReadFrom(xmlReader);
                    return formatter.Feed.ToNewsFeedUpdate();
                }
            }
        }

        private static RssFormat DetectFormat(string content)
        {
            if (content.Contains("<rss"))
            {
                return RssFormat.Rss20;
            }

            if (content.Contains("<rdf:RDF"))
            {
                return RssFormat.Rss10;
            }

            if (content.Contains("fahrgastinfo_1_0.xsd"))
            {
                return RssFormat.Dab;
            }

            return RssFormat.Unknown;
        }

        private static SyndicationFeedFormatter GetFormatter(RssFormat format)
        {
            switch (format)
            {
                case RssFormat.Unknown:
                    break;
                case RssFormat.Rss10:
                    return new Rss10FeedFormatter();
                case RssFormat.Rss20:
                    return new Rss20FeedFormatter();
                case RssFormat.Dab:
                    return new DabFeedFormatter();
                default:
                    throw new ArgumentOutOfRangeException("format", format, "Unrecognized format");
            }

            return null;
        }

        private sealed class Rss10FeedFormatter : SyndicationFeedFormatter
        {
            public override string Version
            {
                get
                {
                    return "Rss10";
                }
            }

            private string RdfNamespaceUri
            {
                get
                {
                    return "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
                }
            }

            private string NamespaceUri
            {
                get
                {
                    return "http://purl.org/rss/1.0/";
                }
            }

            public override bool CanRead(XmlReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }

                return reader.IsStartElement("RDF", this.RdfNamespaceUri);
            }

            public override void ReadFrom(XmlReader reader)
            {
                reader.MoveToContent();
                var xml = reader.ReadOuterXml();
                var document = XDocument.Parse(xml);
                // ReSharper disable once AssignNullToNotNullAttribute
                var namespaceManager = new XmlNamespaceManager(reader.NameTable);
                namespaceManager.AddNamespace("d", this.NamespaceUri);
                var title = document.XPathSelectElement("//d:channel/d:title", namespaceManager);
                if (this.Feed == null)
                {
                    this.SetFeed(new SyndicationFeed());
                }

                this.Feed.Title = new TextSyndicationContent(title.Value);
                this.Feed.Items = (from element in document.XPathSelectElements("//d:item", namespaceManager)
                             let itemTitle = element.XPathSelectElement("d:title", namespaceManager).Value
                             let itemLink = element.XPathSelectElement("d:link", namespaceManager).Value
                             select new SyndicationItem(itemTitle, string.Empty, new Uri(itemLink))).ToList();
            }

            public override void WriteTo(XmlWriter writer)
            {
                throw new NotSupportedException();
            }

            protected override SyndicationFeed CreateFeedInstance()
            {
                return new SyndicationFeed();
            }
        }

        private sealed class DabFeedFormatter : SyndicationFeedFormatter
        {
            public override string Version
            {
                get
                {
                    return "Dab";
                }
            }

            public override bool CanRead(XmlReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }

                return reader.IsStartElement("data", string.Empty);
            }

            public override void ReadFrom(XmlReader reader)
            {
                reader.MoveToContent();
                var xml = reader.ReadOuterXml();
                var document = XDocument.Parse(xml);
                // ReSharper disable once AssignNullToNotNullAttribute
                var namespaceManager = new XmlNamespaceManager(reader.NameTable);
                if (this.Feed == null)
                {
                    this.SetFeed(new SyndicationFeed());
                }

                var titleElement = document.XPathSelectElement("//newsfeed[@rubrik='copyright']/news/titel");
                this.Feed.Title = new TextSyndicationContent(titleElement == null ? "Dab" : titleElement.Value.Trim());
                this.Feed.Items =
                    (from element in
                         document.XPathSelectElements(
                             "//newsfeed[@rubrik='nachrichten' or @rubrik='wetter']/news/titel",
                             namespaceManager)
                     let itemTitle = element.Value
                     where (!string.IsNullOrWhiteSpace(itemTitle))
                     select new SyndicationItem(itemTitle.Trim(), string.Empty, new Uri("dab://localhost"))).ToList();
            }

            public override void WriteTo(XmlWriter writer)
            {
                throw new NotSupportedException();
            }

            protected override SyndicationFeed CreateFeedInstance()
            {
                return new SyndicationFeed();
            }
        }
    }
}