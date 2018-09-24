// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxGdiTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
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

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    using Blend = Microsoft.DirectX.Direct3D.Blend;
    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;
    using GdiFont = System.Drawing.Font;

    /// <summary>
    /// The text part that uses a generated bitmap for rendering text.
    /// </summary>
    public class DxGdiTextPart : DxTextPartBase
    {
        private static Graphics measureStringGraphics;

        private readonly FontQualities fontQuality;

        private readonly Sprite imageSprite;
        private IImageTexture texture;

        private float ascent;

        private double currentScaling;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxGdiTextPart"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="fontQuality">
        /// The font quality.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DxGdiTextPart(
            string text, Font font, FontQualities fontQuality, bool blink, IDxDeviceRenderContext context)
            : base(text, font, blink, context)
        {
            this.fontQuality = fontQuality;
            this.imageSprite = new Sprite(context.Device);
        }

        private Graphics MeasureStringGraphics
        {
            get
            {
                if (measureStringGraphics == null)
                {
                    measureStringGraphics = Graphics.FromImage(new Bitmap(1, 1, PixelFormat.Format32bppArgb));
                    this.PrepareGraphics(measureStringGraphics);
                }

                return measureStringGraphics;
            }
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
            return new DxGdiTextPart(this.Text, this.Font, this.fontQuality, this.Blink, this.Context);
        }

        /// <summary>
        /// Sets the scaling factor of this part.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public override void SetScaling(double factor)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (this.currentScaling == factor)
            {
                return;
            }

            // ReSharper restore CompareOfFloatsByEqualityOperator
            if (this.texture != null)
            {
                this.Context.ReleaseImageTexture(this.texture);
                this.texture = null;
            }

            this.currentScaling = factor;
            Size size;
            using (var bitmap = this.CreateBitmap())
            {
                size = bitmap.Size;
                this.texture = this.Context.GetImageTexture(bitmap);
            }

            this.SetSize(size.Width, size.Height, (int)Math.Ceiling(this.ascent));
        }

        /// <summary>
        /// Tries to split the part into two parts at the given offset.
        /// </summary>
        /// <param name="offset">
        /// The offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left part of the split operation. This is never null.
        /// If the part couldn't be split, this return parameter might be the object this method was called on.
        /// </param>
        /// <param name="right">
        /// The right part of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        public override bool Split(int offset, out DxPart left, out DxPart right)
        {
            string leftText;
            string rightText;
            if (!this.Split(offset, out leftText, out rightText))
            {
                left = this;
                right = null;
                return false;
            }

            left = new DxGdiTextPart(leftText, this.Font, this.fontQuality, this.Blink, this.Context);
            right = new DxGdiTextPart(rightText, this.Font, this.fontQuality, this.Blink, this.Context);
            left.SetScaling(this.currentScaling);
            right.SetScaling(this.currentScaling);
            this.Dispose();
            return true;
        }

        /// <summary>
        /// Renders this part using the given sprite.
        /// </summary>
        /// <param name="x">
        /// The absolute x position of the parent.
        /// </param>
        /// <param name="y">
        /// The absolute y position of the parent.
        /// </param>
        /// <param name="rotationCenter">
        /// The absolut position around which this part should be rotated.
        /// </param>
        /// <param name="rotationAngle">
        /// The angle in radian at which should be rotated.
        /// </param>
        /// <param name="alpha">
        /// The alpha value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(
            int x, int y, PointF rotationCenter, float rotationAngle, int alpha, IDxRenderContext context)
        {
            this.imageSprite.Begin(SpriteFlags.AlphaBlend);
            this.Context.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            this.Context.Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

            try
            {
                this.texture.DrawTo(
                    this.imageSprite,
                    new Rectangle(0, 0, this.Width, this.Height),
                    new SizeF(this.Width, this.Height),
                    new PointF(rotationCenter.X - (this.X + x), rotationCenter.Y - (this.Y + y)),
                    rotationAngle,
                    rotationCenter,
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

            if (this.texture != null)
            {
                this.Context.ReleaseImageTexture(this.texture);
            }

            base.Dispose();
        }

        /// <summary>
        /// Measures the size of the given text using the current <see cref="DxTextPartBase.Font"/>.
        /// </summary>
        /// <param name="text">
        /// The text to be measured.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureString(string text)
        {
            FontFamily family;
            GdiFont font;
            var size = this.MeasureText(text, out family, out font);
            font.Dispose();
            family.Dispose();
            return size;
        }

        private void PrepareGraphics(Graphics graphics)
        {
            switch (this.fontQuality)
            {
                case FontQualities.ClearTypeNatural:
                case FontQualities.ClearType:
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    break;
                case FontQualities.AntiAliased:
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.High;
                    break;
                case FontQualities.NonAntiAliased:
                case FontQualities.Draft:
                case FontQualities.Default:
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                    graphics.CompositingQuality = CompositingQuality.Default;
                    graphics.InterpolationMode = InterpolationMode.Low;
                    break;
                default: // Proof
                    graphics.SmoothingMode = SmoothingMode.Default;
                    graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                    graphics.CompositingQuality = CompositingQuality.Default;
                    graphics.InterpolationMode = InterpolationMode.Default;
                    break;
            }
        }

        private Bitmap CreateBitmap()
        {
            FontFamily family;
            GdiFont font;
            var textSize = this.MeasureText(this.Text, out family, out font);
            var bmp = new Bitmap(textSize.Width, textSize.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bmp))
            {
                this.PrepareGraphics(graphics);
                graphics.Clear(Color.FromArgb(0, this.Color));
                graphics.DrawString(this.Text, font, new SolidBrush(this.Color), 0, 0);
            }

            this.ascent = font.Size * family.GetCellAscent(font.Style) / family.GetEmHeight(font.Style);
            font.Dispose();
            family.Dispose();

            ////bmp.Save("C:\\Temp\\gdi-renderer.png", ImageFormat.Png);
            return bmp;
        }

        private Size MeasureText(string text, out FontFamily family, out GdiFont font)
        {
            var style = FontStyle.Regular;
            if (this.Font.Italic)
            {
                style |= FontStyle.Italic;
            }

            if (this.Font.Weight > 500)
            {
                style |= FontStyle.Bold;
            }

            family = new FontFamily(this.Font.Face);
            font = new GdiFont(family, (float)(this.Font.Height * this.currentScaling), style, GraphicsUnit.Pixel);

            var textSize = this.MeasureStringGraphics.MeasureString(text, font);
            return new Size(
                (int)Math.Ceiling(textSize.Width),
                (int)Math.Ceiling(textSize.Height));
        }
    }
}