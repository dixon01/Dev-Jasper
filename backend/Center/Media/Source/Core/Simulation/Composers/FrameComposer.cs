// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Threading;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Special composer handling <see cref="FrameElementDataViewModel"/>.
    /// </summary>
    public class FrameComposer : DrawableComposerBase<FrameElementDataViewModel, IncludeItem>
    {
        private readonly DispatcherTimer poolTimer;

        private readonly PoolSectionConfigDataViewModel poolSection;

        private int poolIndex;

        private PoolConfigDataViewModel mediaPool;

        private VideoItem currentVideo;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameComposer"/> class.
        /// </summary>
        /// <param name="context">
        /// The composer context.
        /// </param>
        /// <param name="parent">
        /// The parent composer.
        /// </param>
        /// <param name="viewModel">
        /// The data view model.
        /// </param>
        public FrameComposer(IComposerContext context, IComposer parent, FrameElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            if (context.ApplicationState.CurrentSection == null)
            {
                return;
            }

            this.poolSection = context.ApplicationState.CurrentSection as PoolSectionConfigDataViewModel;
            if (this.poolSection == null)
            {
                context.ApplicationState.CurrentSection.PropertyChanged += this.CurrentSectionOnPropertyChanged;
            }
            else
            {
                this.poolSection.PropertyChanged += this.PoolSectionOnPropertyChanged;
                MessageDispatcher.Instance.Subscribe<VideoPlaybackEvent>(this.HandleVideoPlaybackEvent);

                this.LoadMediaPool();

                this.poolTimer = new DispatcherTimer();
                this.poolTimer.Interval = this.GetPoolDuration();
                this.poolTimer.Tick += this.PoolTimerOnTick;
                this.poolTimer.Start();
            }

            this.UpdateFrameContent();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.mediaPool != null)
            {
                this.mediaPool.ResourceReferences.CollectionChanged -= this.ResourceReferencesOnCollectionChanged;
            }

            if (this.poolSection != null)
            {
                this.poolSection.PropertyChanged -= this.PoolSectionOnPropertyChanged;
                MessageDispatcher.Instance.Unsubscribe<VideoPlaybackEvent>(this.HandleVideoPlaybackEvent);
            }

            if (this.poolTimer != null)
            {
                this.poolTimer.Stop();
            }

            base.Dispose();
        }

        /// <summary>
        /// Method to be overridden by subclasses to handle the change of a property of the data view model.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected override void HandlePropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case "FrameId":
                    this.UpdateFrameContent();
                    break;
                case "X":
                case "Y":
                case "Width":
                case "Height":
                    this.UpdateItemBounds();
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }

        private TimeSpan GetPoolDuration()
        {
            if (this.poolSection == null || this.poolSection.Duration.Value < TimeSpan.FromSeconds(1))
            {
                return TimeSpan.FromSeconds(10);
            }

            return this.poolSection.Duration.Value;
        }

        private void LoadMediaPool()
        {
            if (this.mediaPool != null)
            {
                this.mediaPool.ResourceReferences.CollectionChanged -= this.ResourceReferencesOnCollectionChanged;
            }

            this.poolIndex = 0;
            this.mediaPool = this.poolSection.Pool;

            if (this.mediaPool != null)
            {
                this.mediaPool.ResourceReferences.CollectionChanged += this.ResourceReferencesOnCollectionChanged;
            }
        }

        private void UpdateFrameContent()
        {
            var root = new RootItem { Width = this.ViewModel.Width.Value, Height = this.ViewModel.Height.Value };
            var item = this.CreateFrameContent();
            if (item != null)
            {
                this.SetItemProperties(item);
                root.Items.Add(item);
            }

            if (this.Item.Include != null && this.Item.Include.Items.Count == 0 && root.Items.Count == 0)
            {
                // don't update if there was no item and we have no item now
                return;
            }

            this.Item.Include = root;
        }

        private DrawableItemBase CreateFrameContent()
        {
            var section = this.Context.ApplicationState.CurrentSection;
            this.currentVideo = null;

            var imageSection = section as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                return this.CreateImageItem(imageSection.Image, imageSection.Frame.Value);
            }

            var videoSection = section as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                return this.CreateVideoItem(
                    videoSection.Video, videoSection.VideoEndMode.Value, videoSection.Frame.Value);
            }

            if (this.poolSection != null)
            {
                return this.CreatePoolItem(this.poolSection.VideoEndMode.Value, this.poolSection.Frame.Value);
            }

            return null;
        }

        private ImageItem CreateImageItem(ResourceInfoDataViewModel image, int frame)
        {
            if (frame != this.ViewModel.FrameId.Value || image == null)
            {
                return null;
            }

            return new ImageItem
                       {
                           Filename = this.Context.GetImageFileName(image.Hash),
                           Scaling = ElementScaling.Scale
                       };
        }

        private VideoItem CreateVideoItem(ResourceInfoDataViewModel video, VideoEndMode endMode, int frame)
        {
            if (frame != this.ViewModel.FrameId.Value || video == null)
            {
                return null;
            }

            return new VideoItem
                       {
                           VideoUri = this.Context.GetVideoUri(video.Hash),
                           Scaling = ElementScaling.Scale,
                           Replay = endMode == VideoEndMode.Repeat
                       };
        }

        private DrawableItemBase CreatePoolItem(VideoEndMode endMode, int frame)
        {
            if (frame != this.ViewModel.FrameId.Value)
            {
                return null;
            }

            if (this.mediaPool == null || this.mediaPool.ResourceReferences.Count == 0)
            {
                return null;
            }

            if (this.poolIndex >= this.mediaPool.ResourceReferences.Count)
            {
                this.poolIndex = 0;
            }

            var resourceRef = this.mediaPool.ResourceReferences[this.poolIndex];
            var resource =
                this.Context.ApplicationState.CurrentProject.Resources.FirstOrDefault(r => r.Hash == resourceRef.Hash);
            if (resource == null)
            {
                return null;
            }

            switch (resource.Type)
            {
                case ResourceType.Image:
                    return this.CreateImageItem(resource, frame);
                case ResourceType.Video:
                    var video = this.CreateVideoItem(resource, endMode, frame);
                    if (endMode == VideoEndMode.Adapt)
                    {
                        this.poolTimer.Stop();
                        this.currentVideo = video;
                    }

                    return video;
                default:
                    return null;
            }
        }

        private void UpdateItemBounds()
        {
            if (this.Item == null || this.Item.Include == null || this.Item.Include.Items.Count == 0)
            {
                return;
            }

            var item = this.Item.Include.Items[0] as DrawableItemBase;
            if (item == null)
            {
                return;
            }

            this.SetItemProperties(item);
        }

        private void SetItemProperties(DrawableItemBase item)
        {
            item.X = this.ViewModel.X.Value;
            item.Y = this.ViewModel.Y.Value;
            item.Width = this.ViewModel.Width.Value;
            item.Height = this.ViewModel.Height.Value;
            item.ZIndex = this.ViewModel.ZIndex.Value;
        }

        private void CurrentSectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Image":
                case "Video":
                case "Frame":
                    this.UpdateFrameContent();
                    break;
            }
        }

        private void PoolSectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Duration":
                    this.poolTimer.Stop();
                    this.poolTimer.Interval = this.GetPoolDuration();
                    this.poolTimer.Start();
                    break;
                case "Pool":
                    this.LoadMediaPool();
                    this.UpdateFrameContent();
                    break;
                case "Frame":
                    this.UpdateFrameContent();
                    break;
            }
        }

        private void ResourceReferencesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.poolIndex = 0;
            this.UpdateFrameContent();
        }

        private void PoolTimerOnTick(object sender, EventArgs eventArgs)
        {
            this.poolIndex++;
            this.UpdateFrameContent();
        }

        private void HandleVideoPlaybackEvent(object sender, MessageEventArgs<VideoPlaybackEvent> e)
        {
            var video = this.currentVideo;
            if (video == null || video.Id != e.Message.ItemId || e.Message.Playing)
            {
                return;
            }

            this.currentVideo = null;
            this.poolTimer.Start();
            this.poolIndex++;
            this.UpdateFrameContent();
        }
    }
}
