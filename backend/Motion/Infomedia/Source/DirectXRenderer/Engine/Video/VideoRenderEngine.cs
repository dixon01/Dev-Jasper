// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The video render engine.
    /// </summary>
    public class VideoRenderEngine
        : RenderEngineBase<VideoItem,
                IVideoRenderEngine<IDxDeviceRenderContext>,
                VideoRenderManager<IDxDeviceRenderContext>>,
          IVideoRenderEngine<IDxDeviceRenderContext>
    {
        private VideoSpriteBase oldSprite;
        private VideoSpriteBase newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public VideoRenderEngine(VideoRenderManager<IDxDeviceRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(double alpha, IDxDeviceRenderContext context)
        {
            if (this.oldSprite == null)
            {
                this.oldSprite = VideoSpriteBase.Create(context);
                this.oldSprite.IsLooping = this.Manager.Replay;
                this.oldSprite.VideoStarted += this.OldSpriteOnVideoStarted;
                this.oldSprite.VideoEnded += this.OldSpriteOnVideoEnded;
            }

            if (this.newSprite == null)
            {
                this.newSprite = VideoSpriteBase.Create(context);
                this.newSprite.IsLooping = this.Manager.Replay;
                this.newSprite.VideoStarted += this.NewSpriteOnVideoStarted;
                this.newSprite.VideoEnded += this.NewSpriteOnVideoEnded;
            }

            var oldVideoUri = this.Manager.VideoUri.OldValue;
            var newVideoUri = this.Manager.VideoUri.NewValue;

            var bounds = this.Manager.Bounds;
            bounds.X += this.Viewport.X;
            bounds.Y += this.Viewport.Y;

            if (oldVideoUri != null)
            {
                if (this.oldSprite.CurrentVideoUri != oldVideoUri
                    && this.newSprite.CurrentVideoUri == oldVideoUri)
                {
                    // swap sprites
                    var sprite = this.oldSprite;
                    this.oldSprite = this.newSprite;
                    this.newSprite = sprite;
                }

                this.oldSprite.Setup(oldVideoUri, this.Manager.FallbackImage);
                this.oldSprite.Render(
                    bounds, this.Manager.Scaling, (int)(255 * alpha * this.Manager.VideoUri.OldAlpha), context);
            }

            if (newVideoUri != null)
            {
                this.newSprite.Setup(newVideoUri, this.Manager.FallbackImage);
                this.newSprite.Render(
                    bounds, this.Manager.Scaling, (int)(255 * alpha * this.Manager.VideoUri.NewAlpha), context);
            }
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        protected override void OnResetDevice()
        {
            base.OnResetDevice();
            if (this.oldSprite != null)
            {
                this.oldSprite.OnResetDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnResetDevice();
            }
        }

        /// <summary>
        /// Override this method to be notified when a device is lost.
        /// If you have any DirectX resources, make sure to call
        /// OnLostDevice() on them if available.
        /// </summary>
        protected override void OnLostDevice()
        {
            base.OnLostDevice();
            if (this.oldSprite != null)
            {
                this.oldSprite.OnLostDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnLostDevice();
            }
        }

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
        protected override void Release()
        {
            if (this.oldSprite != null)
            {
                this.oldSprite.VideoStarted -= this.OldSpriteOnVideoStarted;
                this.oldSprite.VideoEnded -= this.OldSpriteOnVideoEnded;
                this.oldSprite.Dispose();
                this.oldSprite = null;
            }

            if (this.newSprite != null)
            {
                this.newSprite.VideoStarted -= this.NewSpriteOnVideoStarted;
                this.newSprite.VideoEnded -= this.NewSpriteOnVideoEnded;
                this.newSprite.Dispose();
                this.newSprite = null;
            }
        }

        private void SendVideoPlaybackEvent(bool playing, string videoUri)
        {
            MessageDispatcher.Instance.Broadcast(
                new VideoPlaybackEvent {UnitName = MessageDispatcher.Instance.LocalAddress.Unit, ItemId = this.Manager.ItemId, VideoUri = videoUri, Playing = playing });
        }

        private void OldSpriteOnVideoStarted(object sender, EventArgs e)
        {
            this.SendVideoPlaybackEvent(true, this.Manager.VideoUri.OldValue);
        }

        private void NewSpriteOnVideoStarted(object sender, EventArgs e)
        {
            this.SendVideoPlaybackEvent(true, this.Manager.VideoUri.NewValue);
        }

        private void OldSpriteOnVideoEnded(object sender, EventArgs e)
        {
            this.SendVideoPlaybackEvent(false, this.Manager.VideoUri.OldValue);
        }

        private void NewSpriteOnVideoEnded(object sender, EventArgs e)
        {
            this.SendVideoPlaybackEvent(false, this.Manager.VideoUri.NewValue);
        }
    }
}