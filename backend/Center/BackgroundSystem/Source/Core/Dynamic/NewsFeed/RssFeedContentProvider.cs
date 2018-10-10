// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssFeedContentProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Protocols.GorbaProtocol.Messages;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The RSS feed content provider.
    /// </summary>
    internal class RssFeedContentProvider : NewsFeedContentProviderBase
    {
        private const double ResendTimePercentage = 0.8; // resend after percentage of validity is passed

        private const string WhiteSpacesRegexPattern = @"[ ]{2,}";

        private static readonly Regex WhiteSpacesRegex = new Regex(WhiteSpacesRegexPattern, RegexOptions.None);

        private readonly RssFeedDownloader rssFeedDownloader = new RssFeedDownloader();

        private readonly ITimer refreshTimer; // used to pool the news feed

        private readonly ITimer resendTimer;  // resend feed string

        private NewsFeed lastSendFeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssFeedContentProvider"/> class.
        /// </summary>
        /// <param name="updateGroup">The update group.</param>
        /// <param name="configPart">The config part.</param>
        public RssFeedContentProvider(UpdateGroupReadableModel updateGroup, RssFeedDynamicContentPart configPart)
            : base(updateGroup)
        {
            this.lastSendFeed = null;

            this.ConfigPart = configPart;
            this.refreshTimer = TimerFactory.Current.CreateTimer("RssFeed_" + updateGroup.Name);
            this.refreshTimer.Interval = configPart.RefreshInterval;
            this.refreshTimer.AutoReset = true;
            this.refreshTimer.Elapsed += this.OnRefreshIntervalElapsed;

            this.resendTimer = TimerFactory.Current.CreateTimer("RssFeed_" + updateGroup.Name);
            var validity = configPart.Validity.Ticks > 0 ? configPart.Validity.Ticks : TimeSpan.FromMinutes(1).Ticks;
            this.resendTimer.Interval =
                TimeSpan.FromTicks(Convert.ToInt64(validity  * ResendTimePercentage));
            this.resendTimer.Elapsed += this.OnResendIntervalElapsed;
        }

        /// <summary>
        /// Gets the config part.
        /// </summary>
        public RssFeedDynamicContentPart ConfigPart { get; private set; }

        /// <summary>
        /// The do start.
        /// </summary>
        protected override void DoStart()
        {
            base.DoStart();
            this.refreshTimer.Enabled = true;
            this.resendTimer.Enabled = true;
        }

        /// <summary>
        /// The do stop.
        /// </summary>
        protected override void DoStop()
        {
            this.refreshTimer.Enabled = false;
            this.resendTimer.Enabled = false;
            base.DoStop();
        }

        /// <summary>
        /// Creates the message to be sent.
        /// </summary>
        /// <param name="feed">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="GorbaMessage"/>.
        /// </returns>
        protected override GorbaMessage CreateMessage(NewsFeed feed)
        {
            var update = new NewsFeedMessage
                             {
                                 Id = Guid.NewGuid(),
                                 FeedId = this.ConfigPart.TableRow,
                                 ValidUntil = TimeProvider.Current.UtcNow.Add(this.ConfigPart.Validity),
                                 Content = this.CreateContent(feed)
                             };
            return update;
        }

        private static string CleanupContent(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            var trimmed = title.Trim();

            // removing new lines
            var result = trimmed.Replace(Environment.NewLine, string.Empty);

            // replacing multiple white spaces with a single one
            result = WhiteSpacesRegex.Replace(result, @" ");
            return result;
        }

        private string CreateContent(NewsFeed feed)
        {
            var s =
                feed.Items
                    .Select(item => CleanupContent(item.Title))
                    .Where(title => !string.IsNullOrEmpty(title))
                    .DefaultIfEmpty()
                    .Aggregate((item, feedItem) => item + this.ConfigPart.Delimiter + feedItem);
            if (string.Equals(s, string.Empty))
            {
                return string.Empty;
            }

            // Adding trailing delimiter (for the ring scrolling behavior)
            return s + this.ConfigPart.Delimiter;
        }

        private async void OnRefreshIntervalElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                this.CheckCancelled();
                var feed = await this.rssFeedDownloader.DownloadAsync(new Uri(this.ConfigPart.Url));

                if (this.lastSendFeed != null && this.lastSendFeed.Equals(feed))
                {
                    return;
                }

                this.Send(feed);
                this.lastSendFeed = feed;
                this.resendTimer.Enabled = false;
                this.resendTimer.Enabled = true;
            }
            catch (Exception exception)
            {
                this.Logger.Warn("Error while updating content {0}", exception);
            }
        }

        private void OnResendIntervalElapsed(object sender, EventArgs e)
        {
            try
            {
                this.CheckCancelled();
                this.Send(this.lastSendFeed);
                this.resendTimer.Enabled = false;
                this.resendTimer.Enabled = true;
            }
            catch (Exception exception)
            {
                this.Logger.Warn("Error while resend content {0}", exception);
            }
        }
    }
}