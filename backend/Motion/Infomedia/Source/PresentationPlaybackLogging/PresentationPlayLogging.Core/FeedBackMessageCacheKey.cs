// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedBackMessageCacheKey.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core
{
    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>The feed back message cache key.</summary>
    internal class FeedBackMessageCacheKey
    {
        /// <summary>Initializes a new instance of the <see cref="FeedBackMessageCacheKey"/> class.</summary>
        /// <param name="unitName">The unit name.</param>
        /// <param name="screenId">The screen id.</param>
        public FeedBackMessageCacheKey(string unitName, ScreenId screenId)
        {
            this.ScreenId = screenId;
            this.UnitName = unitName;
        }

        /// <summary>Gets or sets the screen id.</summary>
        public ScreenId ScreenId { get; set; }

        /// <summary>Gets or sets the unit name.</summary>
        public string UnitName { get; set; }
    }
}