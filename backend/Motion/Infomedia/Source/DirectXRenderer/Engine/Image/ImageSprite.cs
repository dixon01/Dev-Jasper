// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSprite.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.DirectXRenderer.Properties;
    using Gorba.Motion.Infomedia.RendererBase;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    /// <summary>
    /// The image sprite.
    /// </summary>
    public class ImageSprite : IDisposable
    {
        /// <summary>
        /// The prefix used in file names if a local resource should be used instead of a file.
        /// </summary>
        public static readonly string ResourcePrefix = "Res:#";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDxDeviceRenderContext context;
        private Sprite sprite;

        private IImageTexture texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSprite"/> class.
        /// </summary>
        /// <param name="context">
        /// The render context.
        /// </param>
        public ImageSprite(IDxDeviceRenderContext context)
        {
            this.context = context;
            this.sprite = new Sprite(this.context.Device);
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
                return this.texture == null ? Size.Empty : this.texture.Size;
            }
        }

        /// <summary>
        /// Sets this sprite up for later rendering.
        /// One of the Setup methods should be called before every rendering.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// A flag indicating if the image file was loaded successfully.
        /// </returns>
        public bool Setup(string filename)
        {
            if (filename == this.CurrentFilename)
            {
                return true;
            }

            Logger.Trace("Changing file name to {0}", filename);
            this.CurrentFilename = filename;

            if (this.texture != null)
            {
                this.context.ReleaseImageTexture(this.texture);
                this.texture = null;
            }

            if (filename.StartsWith(ResourcePrefix))
            {
                var resourceName = filename.Substring(ResourcePrefix.Length);
                var bitmap = Resources.ResourceManager.GetObject(resourceName, CultureInfo.InvariantCulture) as Bitmap;
                if (bitmap == null)
                {
                    Logger.Warn("Couldn't find resource {0}", resourceName);
                    return false;
                }

                this.texture = this.context.GetImageTexture(bitmap);
                Logger.Trace("Texture loaded from bitmap {0}", resourceName);
                return true;
            }

            this.texture = this.context.GetImageTexture(filename);
            if (this.texture == null)
            {
                return false;
            }

            Logger.Trace("Texture loaded from file {0}", filename);
            return true;
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="scaling">
        /// The scaling.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="renderContext">
        /// The context.
        /// </param>
        public virtual void Render(Rectangle bounds, ElementScaling scaling, int alpha, IDxRenderContext renderContext)
        {
            if (this.sprite == null || this.texture == null)
            {
                Logger.Trace("Image not rendered as sprite or texture not available");
                return;
            }

            bounds = RenderHelper.ApplyScaling(bounds, this.Size, scaling);
            this.sprite.Begin(SpriteFlags.AlphaBlend);

            this.context.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            this.context.Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

            this.DrawTexture(
                this.sprite,
                this.texture,
                new Rectangle(Point.Empty, this.texture.Size),
                bounds.Size,
                bounds.Location,
                Color.FromArgb(alpha, Color.White));
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
            if (this.sprite == null)
            {
                return;
            }

            if (this.texture != null)
            {
                this.context.ReleaseImageTexture(this.texture);
                this.texture = null;
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
        protected virtual void DrawTexture(
            Sprite destinationSprite,
            IImageTexture srcTexture,
            Rectangle srcRectangle,
            SizeF destinationSize,
            PointF position,
            Color color)
        {
            srcTexture.DrawTo(destinationSprite, srcRectangle, destinationSize, position, color);
        }
    }
}