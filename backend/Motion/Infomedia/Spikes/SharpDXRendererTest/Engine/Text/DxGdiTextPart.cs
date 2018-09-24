// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxGdiTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxGdiTextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using SharpDX.Direct3D9;

    using Font = Gorba.Motion.Infomedia.Entities.Font;
    using GdiFont = System.Drawing.Font;
    /*
    /// <summary>
    /// The text part that uses a generated bitmap for rendering text.
    /// This class has only a rough implementation since it was decided not to
    /// use it for now.
    /// </summary>
    public class DxGdiTextPart : DxTextPartBase
    {
        private static readonly Graphics MeasureStringGraphics;

        private readonly Sprite imageSprite;
        private readonly Bitmap bitmap;
        private readonly Texture texture;

        private float ascent;

        private SizeF textSize;

        static DxGdiTextPart()
        {
            MeasureStringGraphics = Graphics.FromImage(new Bitmap(1, 1, PixelFormat.Format32bppArgb));
            PrepareGraphics(MeasureStringGraphics);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DxGdiTextPart"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        public DxGdiTextPart(string text, Font font, bool blink, Device device)
            : base(text, font, blink, device)
        {
            this.imageSprite = new Sprite(device);
            this.bitmap = this.CreateBitmap();
            this.texture = new Texture(device, this.bitmap, Usage.None, Pool.Managed);
        }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new DxGdiTextPart(this.Text, this.Font, this.Blink, this.Device);
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
            this.Bounds = new Rectangle(
                x,
                y,
                (int)(this.textSize.Width * sizeFactor),
                (int)(this.textSize.Height * sizeFactor));

            this.Ascent = (int)(this.ascent * sizeFactor);
            return (int)(this.Font.Height * sizeFactor);
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

            this.imageSprite.Begin(SpriteFlags.AlphaBlend);
            try
            {
                // TODO: for some weird reason, the alpha value of the bitmap is lost
                // it is clearly available (e.g. when saving the bitmap to a PNG)
                this.imageSprite.Draw2D(
                    this.texture,
                    new Rectangle(Point.Empty, this.bitmap.Size),
                    bounds.Size,
                    bounds.Location,
                    Color.FromArgb(alpha, Color.White));
            }
            finally
            {
                this.imageSprite.End();
            }
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public override void OnLostDevice()
        {
            base.OnLostDevice();

            this.imageSprite.OnLostDevice();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public override void OnResetDevice()
        {
            base.OnResetDevice();

            this.imageSprite.OnResetDevice();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.imageSprite.Dispose();
            this.texture.Dispose();
            this.bitmap.Dispose();

            base.Dispose();
        }

        private static void PrepareGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
        }

        private Bitmap CreateBitmap()
        {
            var style = FontStyle.Regular;
            if (this.Font.Italic)
            {
                style |= FontStyle.Italic;
            }

            if (this.Font.Weight > 300)
            {
                style |= FontStyle.Bold;
            }

            var family = new FontFamily(this.Font.Face);
            var font = new GdiFont(family, this.Font.Height, style, GraphicsUnit.Pixel);

            this.textSize = MeasureStringGraphics.MeasureString(this.Text, font);
            var bmp = new Bitmap(
                (int)Math.Ceiling(this.textSize.Width),
                (int)Math.Ceiling(this.textSize.Height),
                PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bmp))
            {
                PrepareGraphics(graphics);
                graphics.Clear(Color.FromArgb(0, this.Color));
                graphics.DrawString(this.Text, font, new SolidBrush(this.Color), 0, 0);
            }

            this.ascent = font.Size * family.GetCellAscent(style) / family.GetEmHeight(FontStyle.Regular);

            ////bmp.Save("C:\\Temp\\gdi-renderer.png", ImageFormat.Png);
            return bmp;
        }
    }*/
}