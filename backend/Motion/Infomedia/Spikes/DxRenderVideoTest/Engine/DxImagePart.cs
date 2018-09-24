// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxImagePart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxImagePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Image part for DirectX.
    /// </summary>
    public class DxImagePart : DxPart, IImagePart
    {
        private readonly Bitmap bitmap;

        private Texture texture;

        // we need a separate sprite for the image because rendering text on the 
        // same sprite will offset the text randomly
        private Sprite imageSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxImagePart"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The image file name.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        public DxImagePart(string fileName, bool blink)
            : base(blink)
        {
            this.FileName = fileName;
            this.bitmap = new Bitmap(fileName);
        }

        /// <summary>
        /// Gets the image file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Updates the bounds of this part.
        /// </summary>
        /// <param name="sprite">
        ///     The sprite to use for calculations.
        /// </param>
        /// <param name="x">
        ///     The x position of the part within its parent.
        /// </param>
        /// <param name="y">
        ///     The y position of the part within its parent.
        /// </param>
        /// <param name="sizeFactor">
        ///     The resizing factor.
        /// </param>
        /// <param name="alignOnBaseline">
        ///     Value indicating if the current line of text has to be realigned on the base line.
        /// </param>
        /// <returns>
        /// The entire height of this part (even if it is not using all space).
        /// </returns>
        public override int UpdateBounds(Sprite sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            alignOnBaseline = true;
            this.Bounds = new Rectangle(
                x,
                y,
                (int)(this.bitmap.Width * sizeFactor),
                (int)(this.bitmap.Height * sizeFactor));

            this.Ascent = this.Bounds.Height; // TODO: is this correct?
            return this.Bounds.Height;
        }

        /// <summary>
        /// Renders this part using the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="x">
        /// The absolute x position of the parent.
        /// </param>
        /// <param name="y">
        /// The absolute y position of the parent.
        /// </param>
        /// <param name="alpha">
        /// The alpha value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(Sprite sprite, int x, int y, int alpha, IDxRenderContext context)
        {
            if (this.texture == null)
            {
                this.texture = new Texture(sprite.Device, this.bitmap, Usage.None, Pool.Managed);
                this.imageSprite = new Sprite(sprite.Device);
            }

            this.imageSprite.Begin(SpriteFlags.SortTexture | SpriteFlags.AlphaBlend);
            this.imageSprite.Draw2D(
                this.texture,
                new Rectangle(Point.Empty, this.bitmap.Size),
                new SizeF(this.Bounds.Width, this.Bounds.Height),
                new PointF(x + this.Bounds.X, y + this.Bounds.Y),
                Color.FromArgb(alpha, Color.White));
            this.imageSprite.End();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.imageSprite.Dispose();
            }

            this.bitmap.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public override void OnResetDevice(Device device)
        {
            base.OnResetDevice(device);

            if (this.imageSprite != null)
            {
                this.imageSprite.OnResetDevice();
            }
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public override void OnLostDevice()
        {
            base.OnLostDevice();

            if (this.imageSprite != null)
            {
                this.imageSprite.OnLostDevice();
            }
        }
    }
}