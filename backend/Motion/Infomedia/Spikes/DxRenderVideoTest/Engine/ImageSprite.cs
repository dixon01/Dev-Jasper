// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSprite.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System;
    using System.Drawing;
    using System.IO;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The image sprite.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the item.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// The type of the engine.
    /// </typeparam>
    public class ImageSprite<TItem, TEngine> : IDisposable
        where TItem : ImageItem
        where TEngine : class, IRenderEngine<IDxRenderContext>
    {
        private readonly DrawableRenderManagerBase<TItem, IDxRenderContext, TEngine> manager;

        private Device device;
        private Bitmap bitmap;
        private Sprite sprite;

        private Texture texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSprite{TItem,TEngine}"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        public ImageSprite(DrawableRenderManagerBase<TItem, IDxRenderContext, TEngine> manager, Device device)
        {
            this.device = device;
            this.manager = manager;
            this.sprite = new Sprite(this.device);
        }

        /// <summary>
        /// Gets the file name of the currently displayed image.
        /// </summary>
        public string CurrentFilename { get; private set; }

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

            this.CurrentFilename = filename;

            this.ReleaseImage();

            if (!File.Exists(filename))
            {
                return;
            }

            this.bitmap = new Bitmap(filename);
            this.texture = new Texture(this.device, this.bitmap, Usage.None, Pool.Managed);
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public virtual void Render(int alpha, IDxRenderContext context)
        {
            if (this.sprite == null || this.texture == null)
            {
                return;
            }

            this.sprite.Begin(SpriteFlags.AlphaBlend);
            this.DrawTexture(
                this.sprite,
                this.texture,
                new Rectangle(Point.Empty, this.bitmap.Size),
                new SizeF(this.manager.Width, this.manager.Height),
                new PointF(this.manager.X, this.manager.Y),
                Color.FromArgb(alpha, Color.White));
            this.sprite.End();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        /// <param name="dev">
        /// The device.
        /// </param>
        public void OnResetDevice(Device dev)
        {
            this.device = dev;
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
        protected virtual void DrawTexture(Sprite destinationSprite, Texture srcTexture, Rectangle srcRectangle, SizeF destinationSize, PointF position, Color color)
        {
            destinationSprite.Draw2D(
                srcTexture,
                srcRectangle,
                destinationSize,
                position,
                color);
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