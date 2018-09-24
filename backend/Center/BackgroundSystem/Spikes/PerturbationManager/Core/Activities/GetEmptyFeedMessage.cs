// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmptyFeedMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetEmptyFeedMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Core.Activities
{
    using System;
    using System.Activities;

    using Google.Transit.Realtime;

    public sealed class GetEmptyFeedMessage : CodeActivity<FeedMessage>
    {
        private static readonly DateTime EpochReference = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly FeedMessage EmptyFeedMessage = new FeedMessage
        {
            header =
                new FeedHeader
                {
                    gtfs_realtime_version = "1.0",
                    incrementality = FeedHeader.Incrementality.FULL_DATASET,
                    timestamp = ToEpoch(DateTime.UtcNow)
                }
        };

        protected override FeedMessage Execute(CodeActivityContext context)
        {
            return EmptyFeedMessage;
        }

        public static ulong ToEpoch(DateTime date)
        {
            return (ulong)date.Subtract(EpochReference).TotalSeconds;
        }
    }
}