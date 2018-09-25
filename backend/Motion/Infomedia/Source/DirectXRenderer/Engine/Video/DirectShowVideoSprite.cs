// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectShowVideoSprite.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectShowVideoSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;
    using System.Drawing;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The DirectShow video sprite.
    /// </summary>
    public class DirectShowVideoSprite : VideoSpriteBase
    {
        private Sprite sprite;

        private DirectShowVideoTexture texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectShowVideoSprite"/> class.
        /// </summary>
        /// <param name="context">
        ///     The render context.
        /// </param>
        public DirectShowVideoSprite(IDxDeviceRenderContext context)
            : base(context)
        {
            this.sprite = new Sprite(context.Device);
        }

        /// <summary>
        /// Gets the size of the video.
        /// </summary>
        public override Size Size
        {
            get
            {
                return this.texture == null ? Size.Empty : this.texture.Size;
            }
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public override void OnResetDevice()
        {
            base.OnResetDevice();
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.OnResetDevice();
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public override void OnLostDevice()
        {
            base.OnLostDevice();
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.OnLostDevice();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (this.sprite == null)
            {
                return;
            }

            this.sprite.Dispose();
            this.sprite = null;
        }

        /// <summary>
        /// Creates the video.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        protected override void CreateVideo(string filename)
        {
            this.texture = new DirectShowVideoTexture(filename);
            this.texture.IsLooping = this.IsLooping;
            this.texture.CreateTexture(this.Device);
            this.texture.VideoStarted += this.TextureOnVideoStarted;
            this.texture.VideoEnded += this.TextureOnVideoEnded;
            this.texture.Start();
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Render(Rectangle bounds, int alpha, IDxRenderContext context)
        {
            if (this.sprite == null || this.texture == null || this.texture.Texture == null || alpha == 0)
            {
                return;
            }

            this.sprite.Begin(SpriteFlags.AlphaBlend);
            this.sprite.Draw2D(
                this.texture.Texture,
                new Rectangle(Point.Empty, this.texture.Size),
                bounds.Size,
                bounds.Location,
                Color.FromArgb(alpha, Color.White));
            this.sprite.End();
        }

        /// <summary>
        /// Releases the video.
        /// </summary>
        protected override void ReleaseVideo()
        {
            if (this.texture == null)
            {
                return;
            }

            this.texture.VideoStarted -= this.TextureOnVideoStarted;
            this.texture.VideoEnded -= this.TextureOnVideoEnded;
            this.texture.Dispose();
            this.texture = null;
        }

        private void TextureOnVideoStarted(object sender, EventArgs e)
        {
            this.RaiseVideoStarted(e);
        }

        private void TextureOnVideoEnded(object sender, EventArgs e)
        {
            this.RaiseVideoEnded(e);
        }
    }
}
