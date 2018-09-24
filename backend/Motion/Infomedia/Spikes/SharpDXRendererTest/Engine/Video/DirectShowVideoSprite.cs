// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectShowVideoSprite.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectShowVideoSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Video
{
    using System.Drawing;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Rectangle = System.Drawing.Rectangle;

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
        /// <param name="device">
        ///     The device.
        /// </param>
        public DirectShowVideoSprite(Device device)
            : base(device)
        {
            this.sprite = new Sprite(device);
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
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="alpha">
        ///     The alpha.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void Render(Rectangle bounds, int alpha, IDxRenderContext context)
        {
            if (this.sprite == null || this.texture == null || this.texture.Texture == null)
            {
                return;
            }

            this.sprite.Begin(SpriteFlags.AlphaBlend);
            this.sprite.Draw(
                this.texture.Texture,
                new ColorBGRA(255, 255, 255, alpha),
                new SharpDX.Rectangle(0, 0, this.texture.Width, this.texture.Height),
                null,
                new Vector3(bounds.X, bounds.Y, 0));
            this.sprite.End();
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
            this.texture.CreateTexture(this.Device);
            this.texture.Start();
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

            this.texture.Dispose();
            this.texture = null;
        }
    }
}
