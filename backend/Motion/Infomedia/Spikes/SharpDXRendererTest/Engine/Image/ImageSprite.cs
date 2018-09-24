// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSprite.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Image
{
    using System;
    using System.Drawing;
    using System.IO;

    using Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine;

    using NLog;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Rectangle = System.Drawing.Rectangle;

    /// <summary>
    /// The image sprite.
    /// </summary>
    public class ImageSprite : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Device device;
        private Bitmap bitmap;
        private Sprite sprite;

        private Texture texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSprite"/> class.
        /// </summary>
        /// <param name="device">
        ///     The device.
        /// </param>
        public ImageSprite(Device device)
        {
            this.device = device;
            this.sprite = new Sprite(this.device);
        }

        /// <summary>
        /// Gets the file name of the currently displayed image.
        /// </summary>
        public string CurrentFilename { get; private set; }

        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        public Size Size
        {
            get
            {
                return this.bitmap == null ? Size.Empty : this.bitmap.Size;
            }
        }

        /// <summary>
        /// Sets this sprite up for later rendering.
        /// This method should be called before every rendering.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void Setup(string filename)
        {
            if (filename == this.CurrentFilename)
            {
                return;
            }

            Logger.Trace("Changing file name to {0}", filename);
            this.CurrentFilename = filename;

            this.ReleaseImage();

            if (!File.Exists(filename))
            {
                Logger.Warn("Image file does not exist: {0}", filename);
                return;
            }

            this.bitmap = new Bitmap(filename);
            this.texture = TextureFactory.FromBitmap(this.device, this.bitmap, Usage.None, Pool.Managed);
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
        public virtual void Render(Rectangle bounds, int alpha, IDxRenderContext context)
        {
            if (this.sprite == null || this.texture == null)
            {
                Logger.Trace("Image not rendered as sprite or texture not available");
                return;
            }

            this.sprite.Begin(SpriteFlags.AlphaBlend);
            this.DrawTexture(
                this.sprite,
                this.texture,
                new Rectangle(Point.Empty, this.bitmap.Size),
                bounds.Size,
                bounds.Location,
                new ColorBGRA(0xFF, 0xFF, 0xFF, (byte)alpha));
            this.sprite.End();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public void OnResetDevice()
        {
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.OnResetDevice();
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public void OnLostDevice()
        {
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.OnLostDevice();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ReleaseImage();

            if (this.sprite == null)
            {
                return;
            }

            this.sprite.Dispose();
            this.sprite = null;
        }

        /// <summary>
        /// Draws the texture to the sprite.
        /// This is available for subclasses that need to 
        /// render images differently.
        /// </summary>
        /// <param name="destinationSprite">
        /// The destination sprite.
        /// </param>
        /// <param name="srcTexture">
        /// The source texture.
        /// </param>
        /// <param name="srcRectangle">
        /// The source rectangle.
        /// </param>
        /// <param name="destinationSize">
        /// The destination size.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        protected virtual void DrawTexture(Sprite destinationSprite, Texture srcTexture, Rectangle srcRectangle, SizeF destinationSize, PointF position, ColorBGRA color)
        {
            destinationSprite.Draw(srcTexture, color, srcRectangle.ToSharpDx(), null, new Vector3(position.X, position.Y, 0));
            /*destinationSprite.Draw(
                srcTexture,
                srcRectangle,
                destinationSize,
                position,
                color);*/
        }

        private void ReleaseImage()
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }

            if (this.bitmap != null)
            {
                this.bitmap.Dispose();
                this.bitmap = null;
            }
        }
    }
}