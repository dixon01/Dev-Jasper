// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxFontTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxFontTextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text
{
    using Gorba.Motion.Infomedia.RendererBase.Text;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Font = Gorba.Motion.Infomedia.Entities.Font;
    using Rectangle = System.Drawing.Rectangle;

    /// <summary>
    /// Text part for DirectX.
    /// </summary>
    public class DxFontTextPart : DxTextPartBase
    {
        private readonly FontQuality fontQuality;

        private FontEx fontInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxFontTextPart"/> class.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="font">
        ///     The font.
        /// </param>
        /// <param name="fontQuality">
        /// The font quality;
        /// </param>
        /// <param name="blink">
        ///     The blink.
        /// </param>
        /// <param name="device">
        ///     The device.
        /// </param>
        public DxFontTextPart(string text, Font font, FontQuality fontQuality, bool blink, Device device)
            : base(text, font, blink, device)
        {
            this.fontQuality = fontQuality;
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
            return new DxFontTextPart(this.Text, this.Font, this.fontQuality, this.Blink, this.Device);
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
            this.fontInfo = FontCache.Instance.CreateFont(
                                        this.Device,
                                        (int)(this.Font.Height * sizeFactor),
                                        0,
                                        (FontWeight)this.Font.Weight,
                                        10,
                                        this.Font.Italic,
                                        FontCharacterSet.Default,
                                        FontPrecision.Default,
                                        this.fontQuality,
                                        FontPitchAndFamily.Default,
                                        this.Font.Face);

            var metrics = this.fontInfo.Metrics;
            int itemHeight = metrics.Height;
            this.Ascent = metrics.Ascent;

            var rect = (DrawingRectangle)this.fontInfo.Font.MeasureText(sprite, this.Text, FontDrawFlags.NoClip);

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

            this.Bounds = new Rectangle(x, y, rect.Width, rect.Height);
            return itemHeight;
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
            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            this.fontInfo.Font.DrawText(
                sprite,
                this.Text,
                x + this.Bounds.X,
                y + this.Bounds.Y,
                new ColorBGRA(this.Color.R, this.Color.G, this.Color.B, alpha));
        }
    }
}