// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicContentProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicContentProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic
{
    using System;

    using Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper;
    using Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed;
    using Gorba.Center.BackgroundSystem.Core.Dynamic.ScreenGate;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Common.Update.ServiceModel.Common;

    using NLog;

    /// <summary>
    /// The base class for all dynamic content providers.
    /// Subclasses generate dynamic content from an online source and provide it
    /// to the update system to load on the unit(s).
    /// </summary>
    internal abstract class DynamicContentProviderBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContentProviderBase"/> class.
        /// </summary>
        protected DynamicContentProviderBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Event that is fired whenever the content of this provider was updated.
        /// This means the <see cref="CreateFolderStructure"/> method should be called again.
        /// </summary>
        public event EventHandler ContentUpdated;

        /// <summary>
        /// Gets a value indicating whether this provider is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates a <see cref="DynamicContentProviderBase"/> from the given configuration.
        /// </summary>
        /// <param name="updateGroup">
        /// The update Group.
        /// </param>
        /// <param name="part">
        /// The dynamic content part.
        /// </param>
        /// <returns>
        /// The newly created <see cref="DynamicContentProviderBase"/> implementation.
        /// </returns>
        public static DynamicContentProviderBase Create(
            UpdateGroupReadableModel updateGroup,
            DynamicContentPartBase part)
        {
            var screenGate = part as ScreenGateDynamicContentPart;
            if (screenGate != null)
            {
                return new ScreenGateContentProvider(screenGate);
            }

            var rssNewsFeed = part as RssFeedDynamicContentPart;
            if (rssNewsFeed != null)
            {
                return new RssFeedContentProvider(updateGroup, rssNewsFeed);
            }

            var paper = part as EPaperDynamicContentPart;
            if (paper != null)
            {
                return new EPaperContentProvider(updateGroup, paper);
            }

            // not supported, so just ignore this part
            return null;
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.IsRunning = true;
            this.DoStart();
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning)
            {
                return;
            }

            this.IsRunning = false;
            this.DoStop();
        }

        /// <summary>
        /// Creates the folder structure for this content provider.
        /// This method should never return null, but can return an empty object.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateFolderStructure"/>.
        /// </returns>
        public abstract UpdateFolderStructure CreateFolderStructure();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Implementation of <see cref="Start"/>.
        /// </summary>
        protected abstract void DoStart();

        /// <summary>
        /// Implementation of <see cref="Stop"/>.
        /// </summary>
        protected abstract void DoStop();

        /// <summary>
        /// Raises the <see cref="ContentUpdated"/> event.
        /// </summary>
        protected virtual void RaiseContentUpdated()
        {
            var handler = this.ContentUpdated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}