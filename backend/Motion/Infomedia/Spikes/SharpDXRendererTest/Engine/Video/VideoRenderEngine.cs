// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Video
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The video render engine.
    /// </summary>
    public class VideoRenderEngine
        : RenderEngineBase<VideoItem, IVideoRenderEngine<IDxRenderContext>, VideoRenderManager<IDxRenderContext>>,
          IVideoRenderEngine<IDxRenderContext>
    {
        private VideoSpriteBase oldSprite;
        private VideoSpriteBase newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public VideoRenderEngine(VideoRenderManager<IDxRenderContext> manager)
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
        protected override void DoRender(double alpha, IDxRenderContext context)
        {
            if (this.oldSprite == null)
            {
                this.oldSprite = VideoSpriteBase.Create(this.Device);
            }

            if (this.newSprite == null)
            {
                this.newSprite = VideoSpriteBase.Create(this.Device);
            }

            var oldVideoUri = this.Manager.VideoUri.OldValue;
            var newVideoUri = this.Manager.VideoUri.NewValue;

            var bounds = this.Manager.Bounds;
            bounds.X -= this.Viewport.X;
            bounds.Y -= this.Viewport.Y;

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

                this.oldSprite.Setup(oldVideoUri);
                this.oldSprite.Render(bounds, (int)(255 * alpha * this.Manager.VideoUri.OldAlpha), context);
            }

            if (newVideoUri != null)
            {
                this.newSprite.Setup(newVideoUri);
                this.newSprite.Render(bounds, (int)(255 * alpha * this.Manager.VideoUri.NewAlpha), context);
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
                this.oldSprite.Dispose();
                this.oldSprite = null;
            }

            if (this.newSprite != null)
            {
                this.newSprite.Dispose();
                this.newSprite = null;
            }
        }
    }
}