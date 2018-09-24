// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxFontTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxFontTextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;

    /// <summary>
    /// Text part for DirectX.
    /// </summary>
    public class DxFontTextPart : DxTextPartBase
    {
        private readonly FontQuality fontQuality;

        private readonly Sprite sprite;

        private IFontInfo fontInfo;

        private double currentScaling;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxFontTextPart"/> class.
        /// </summary>
        /// <param name="sprite">
        /// The sprite (might be null).
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="fontQuality">
        /// The font quality;
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DxFontTextPart(
            Sprite sprite,
            string text,
            Font font,
            FontQualities fontQuality,
            bool blink,
            IDxDeviceRenderContext context)
            : base(text, font, blink, context)
        {
            this.sprite = sprite;
            this.currentScaling = 1.0;

            switch (fontQuality)
            {
                case FontQualities.ClearTypeNatural:
                    this.fontQuality = FontQuality.ClearTypeNatural;
                    break;
                case FontQualities.ClearType:
                    this.fontQuality = FontQuality.ClearType;
                    break;
                case FontQualities.AntiAliased:
                    this.fontQuality = FontQuality.AntiAliased;
                    break;
                case FontQualities.NonAntiAliased:
                    this.fontQuality = FontQuality.NonAntiAliased;
                    break;
                case FontQualities.Proof:
                    this.fontQuality = FontQuality.Proof;
                    break;
                case FontQualities.Draft:
                    this.fontQuality = FontQuality.Draft;
                    break;
                default:
                    this.fontQuality = FontQuality.Default;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DxFontTextPart"/> class.
        /// </summary>
        /// <param name="sprite">
        /// The sprite (might be null).
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="fontQuality">
        /// The font quality;
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private DxFontTextPart(
            Sprite sprite, string text, Font font, FontQuality fontQuality, bool blink, IDxDeviceRenderContext context)
            : base(text, font, blink, context)
        {
            this.sprite = sprite;
            this.fontQuality = fontQuality;
            this.currentScaling = 1.0;
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
            return new DxFontTextPart(this.sprite, this.Text, this.Font, this.fontQuality, this.Blink, this.Context);
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
            this.currentScaling = factor;
            this.fontInfo = this.Context.GetFontInfo(
                                        (int)(this.Font.Height * factor),
                                        0,
                                        (FontWeight)this.Font.Weight,
                                        10,
                                        this.Font.Italic,
                                        CharacterSet.Default,
                                        Precision.Default,
                                        this.fontQuality,
                                        PitchAndFamily.DefaultPitch,
                                        this.Font.Face);

            var rect = this.fontInfo.Font.MeasureString(this.sprite, this.Text, DrawTextFormat.NoClip, this.Color);

            // fix spacing at the end
            if (this.Text.EndsWith(" "))
            {
                for (int j = this.Text.Length - 1; j >= 0; j--)
                {
                    if (this.Text[j] != ' ')
                    {
                        break;
                    }

                    rect.Width += this.fontInfo.SpaceWidth;
                }
            }

            var metrics = this.fontInfo.Metrics;
            this.SetSize(rect.Width, metrics.Height, metrics.Ascent);
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

            left = new DxFontTextPart(this.sprite, leftText, this.Font, this.fontQuality, this.Blink, this.Context);
            right = new DxFontTextPart(this.sprite, rightText, this.Font, this.fontQuality, this.Blink, this.Context);
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
            this.fontInfo.Font.DrawText(
                this.sprite,
                this.Text,
                x + this.X,
                y + this.Y,
                Color.FromArgb(alpha, this.Color));
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
            var rect = this.fontInfo.Font.MeasureString(this.sprite, text, DrawTextFormat.NoClip, this.Color);
            return rect.Size;
        }
    }
}