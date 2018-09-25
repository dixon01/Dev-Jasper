// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxImagePart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxImagePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Image;

    using SharpDX.Direct3D9;

    /// <summary>
    /// Image part for DirectX.
    /// </summary>
    public class DxImagePart : DxPart, IImagePart
    {
        private readonly ImageSprite imageSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxImagePart"/> class.
        /// </summary>
        /// <param name="fileName">
        ///     The image file name.
        /// </param>
        /// <param name="blink">
        ///     The blink.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        public DxImagePart(string fileName, bool blink, Device device)
            : base(blink, device)
        {
            this.FileName = fileName;

            this.imageSprite = new ImageSprite(device);
            this.imageSprite.Setup(this.FileName);
        }

        /// <summary>
        /// Gets the image file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new DxImagePart(this.FileName, this.Blink, this.Device);
        }

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
            var size = this.imageSprite.Size;
            this.Bounds = new Rectangle(
                x,
                y,
                (int)(size.Width * sizeFactor),
                (int)(size.Height * sizeFactor));

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
            var bounds = this.Bounds;
            bounds.X += x;
            bounds.Y += y;

            this.imageSprite.Render(bounds, alpha, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.imageSprite != null)
            {
                this.imageSprite.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public override void OnResetDevice()
        {
            base.OnResetDevice();

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