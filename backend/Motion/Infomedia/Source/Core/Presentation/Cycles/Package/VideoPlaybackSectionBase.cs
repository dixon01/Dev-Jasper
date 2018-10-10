// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlaybackSectionBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoPlaybackSectionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Base class for sections that can play videos.
    /// </summary>
    public abstract class VideoPlaybackSectionBase : SinglePageSectionBase
    {
        private VideoItem videoItem;

        private TimeSpan currentDuration;

        private bool playingAdaptedVideo;
        private bool videoStopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoPlaybackSectionBase"/> class.
        /// </summary>
        /// <param name="sectionConfig">
        /// The section config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected VideoPlaybackSectionBase(SectionConfigBase sectionConfig, IPresentationContext context)
            : base(sectionConfig, context)
        {
            MessageDispatcher.Instance.Subscribe<VideoPlaybackEvent>(this.HandleVideoPlaybackEvent);
        }

        /// <summary>
        /// Gets the way of dealing with a video that has
        /// a different duration than defined in this section.
        /// </summary>
        protected abstract VideoEndMode VideoEndMode { get; }

        /// <summary>
        /// Prepares the given item to be shown for this section.
        /// </summary>
        /// <param name="item">
        /// The screen item.
        /// </param>
        public override void PrepareItem(ScreenItemBase item)
        {
            base.PrepareItem(item);

            var video = item as VideoItem;
            if (video != null)
            {
                this.videoItem = video;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            MessageDispatcher.Instance.Unsubscribe<VideoPlaybackEvent>(this.HandleVideoPlaybackEvent);
        }

        /// <summary>
        /// Check if we are currently showing a video.
        /// </summary>
        /// <returns>
        /// True if we are showing a video, otherwise false.
        /// </returns>
        protected abstract bool IsShowingVideo();

        /// <summary>
        /// Starts the timer that will elapse after <see cref="SectionBase{T}.GetDuration"/>.
        /// </summary>
        protected override void StartTimer()
        {
            this.videoStopped = false;
            if (this.VideoEndMode != VideoEndMode.Adapt || !this.IsShowingVideo())
            {
                this.playingAdaptedVideo = false;
                base.StartTimer();
                return;
            }

            // we have a video item that is in "Adapt" mode, so we don't have to
            // start the normal timer but wait for the completion event (or a timeout we start here)
            this.playingAdaptedVideo = true;
            this.currentDuration = this.GetDuration();
            this.Context.Time.AddTimeElapsedHandler(this.currentDuration, this.OnVideoEventTimeout);
        }

        /// <summary>
        /// Raises the <see cref="SectionBase{T}.DurationExpired"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseDurationExpired(EventArgs e)
        {
            // make sure we call the "RaiseDurationExpired" only exactly once
            if (this.videoStopped)
            {
                return;
            }

            this.videoStopped = true;
            this.playingAdaptedVideo = false;
            base.RaiseDurationExpired(e);
        }

        private void OnVideoEventTimeout(DateTime obj)
        {
            // we didn't get the "Playing" event in time, let's raise the "DurationExpired" event
            this.Logger.Warn("No VideoPlaybackEvent event received, stopping after standard duration");
            this.RaiseDurationExpired(EventArgs.Empty);
        }

        private void HandleVideoPlaybackEvent(object sender, MessageEventArgs<VideoPlaybackEvent> e)
        {
            if (!this.playingAdaptedVideo || this.videoItem == null || e.Message.VideoUri != this.videoItem.VideoUri
                || e.Message.ItemId != this.videoItem.Id)
            {
                return;
            }

            this.Context.Time.RemoveTimeElapsedHandler(this.currentDuration, this.OnVideoEventTimeout);
            if (e.Message.Playing)
            {
                this.Logger.Debug("{0} received, disabled fallback timer", e.Message);
            }
            else
            {
                // the video is no more playing, let's raise the "DurationExpired" event
                // we need to do this from within an "update", so let's start a dummy timer
                this.Logger.Debug("{0} received, switching to next page", e.Message);
                this.Context.Time.AddTimeElapsedHandler(new TimeSpan(1), d => this.RaiseDurationExpired(e));
            }
        }
    }
}