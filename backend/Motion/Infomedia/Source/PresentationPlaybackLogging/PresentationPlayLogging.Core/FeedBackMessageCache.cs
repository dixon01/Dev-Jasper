// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedBackMessageCache.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core
{
    using System;
    using System.Collections.Concurrent;

    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PresentationPlayLogging.Core.Models;

    /// <summary>The feed back message cache.</summary>
    internal class FeedBackMessageCache : ConcurrentDictionary<FeedBackMessageCacheKey, InfotransitPresentationInfo>
    {
        public FeedBackMessageCache()
        {
            throw new NotImplementedException("Work In Progress");
        }

        /// <summary>The find.</summary>
        /// <param name="unitsFeedBackMessage">The feed back message.</param>
        /// <returns>The <see cref="InfotransitPresentationInfo"/>.</returns>
        public InfotransitPresentationInfo FindOrAdd(UnitsFeedBackMessage<ScreenChange> unitsFeedBackMessage)
        {
            if (unitsFeedBackMessage?.Message != null)
            {
                var unitName = unitsFeedBackMessage.UnitName;
                if (!string.IsNullOrEmpty(unitName))
                {
                    lock (typeof(UnitsFeedBackMessage<ScreenChange>))
                    {
                        var screenId = unitsFeedBackMessage.Message?.Screen;
                        var key = new FeedBackMessageCacheKey(unitName, screenId);
                        var infotransitePresentationInfo = new InfotransitPresentationInfo
                                                               {
                            // TODO set props from the incomming feedback message
                            // TODO determine what is important in the message and if enough to create our model
                            // 
                                                                   PlayStarted = DateTime.Now,
                                                                   FileName = "?"
                                                               };
                        var presentationInfo = this.GetOrAdd(key, infotransitePresentationInfo);

                        return presentationInfo;
                    }
                }
            }

            return null;
        }
    }
}