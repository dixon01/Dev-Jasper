// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageTextureBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageTextureBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Implementation base of <see cref="IImageTexture"/>.
    /// </summary>
    public abstract class ImageTextureBase : IImageTexture, IDisposable
    {
        private readonly Device device;

        private readonly Control focusWindow;

        private Texture texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageTextureBase"/> class.
        /// </summary>
        /// <param name="device">
        /// The DirectX device.
        /// </param>
        protected ImageTextureBase(Device device)
        {
            this.device = device;

            this.focusWindow = this.device.CreationParameters.FocusWindow;
            if (this.focusWindow != null)
            {
                this.focusWindow.Disposed += this.FocusWindowOnDisposed;
            }
        }

        /// <summary>
        /// Event that is fired when this object is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets or sets the last used milliseconds counter (needed for caching).
        /// </summary>
        public long LastUsed { get; set; }

        /// <summary>
        /// Gets or sets the number usages (references to this texture).
        /// </summary>
        public int Usages { get; set; }

        /// <summary>
        /// Gets the approximate size of the underlying bitmap in bytes.
        /// </summary>
        public int BitmapBytes
        {
            get
            {
                return this.Size.Width * this.Size.Height * 4; // approximate number of bytes
            }
        }

        /// <summary>
        /// Gets the size of the original image.
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Draws this image to the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
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
        public virtual void DrawTo(
            Sprite sprite, Rectangle srcRectangle, SizeF destinationSize, PointF position, Color color)
        {
            this.PrepareTexture(sprite);
            sprite.Draw2D(this.texture, srcRectangle, destinationSize, position, color);
        }

        /// <summary>
        /// Draws this image with the given rotation to the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="srcRectangle">
        /// The source rectangle.
        /// </param>
        /// <param name="destinationSize">
        /// The destination size.
        /// </param>
        /// <param name="rotationCenter">
        /// The rotation center.
        /// </param>
        /// <param name="rotationAngle">
        /// The rotation angle.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public virtual void DrawTo(
            Sprite sprite,
            Rectangle srcRectangle,
            SizeF destinationSize,
            PointF rotationCenter,
            float rotationAngle,
            PointF position,
            Color color)
        {
            this.PrepareTexture(sprite);
            sprite.Draw2D(this.texture, srcRectangle, destinationSize, rotationCenter, rotationAngle, position, color);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            try
            {
                var handler = this.Disposing;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                if (this.texture.Disposed)
                {
                    return;
                }

                if (this.focusWindow != null)
                {
                    this.focusWindow.Disposed -= this.FocusWindowOnDisposed;
                }

                if (this.texture != null)
                    this.texture.Dispose();
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Bitmap[{0}x{1}]", this.Size.Width, this.Size.Height);
        }

        /// <summary>
        /// Initializes this texture with the given bitmap.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        protected void Initialize(Bitmap bitmap)
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
            }

            this.Size = bitmap.Size;
            this.texture = new Texture(this.device, bitmap, Usage.None, Pool.Managed);
        }

        private void PrepareTexture(Sprite sprite)
        {
            sprite.Device.SetTexture(0, this.texture);

            sprite.Device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
            sprite.Device.SetTextureStageState(0, TextureStageStates.ColorArgument2, (int)TextureArgument.Diffuse);
            sprite.Device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.Modulate);

            sprite.Device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
            sprite.Device.SetTextureStageState(0, TextureStageStates.AlphaArgument2, (int)TextureArgument.Diffuse);
            sprite.Device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
        }

        private void FocusWindowOnDisposed(object sender, EventArgs eventArgs)
        {
            this.Dispose();
        }
    }
}