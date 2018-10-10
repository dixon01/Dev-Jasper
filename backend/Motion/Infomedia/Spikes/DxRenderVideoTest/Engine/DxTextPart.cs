// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;
    using Microsoft.Samples.DirectX.UtilityToolkit;
    using Microsoft.Samples.DirectX.UtilityToolkit.Extensions;

    using Font = Gorba.Motion.Infomedia.Entities.Font;

    /// <summary>
    /// Text part for DirectX.
    /// </summary>
    public class DxTextPart : DxPart, ITextPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxTextPart"/> class.
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
        public DxTextPart(string text, Gorba.Motion.Infomedia.Entities.Font font, bool blink)
            : base(blink)
        {
            this.Text = text;
            this.Font = font;

            if (font == null)
            {
                return;
            }

            this.Color = font.GetColor();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Gets or sets the DirectX font.
        /// </summary>
        public Microsoft.DirectX.Direct3D.Font DxFont { get; set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color { get; private set; }

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
            var font = ResourceCache.GetGlobalInstance()
                                    .CreateFont(
                                        sprite.Device,
                                        (int)(this.Font.Height * sizeFactor),
                                        0,
                                        (FontWeight)this.Font.Weight,
                                        10,
                                        this.Font.Italic,
                                        CharacterSet.Default,
                                        Precision.Default,
                                        FontQuality.AntiAliased,
                                        PitchAndFamily.DefaultPitch,
                                        this.Font.Face);

            var info = FontInfoExtension.GetFor(font);
            var metrics = info.Metrics;
            int itemHeight = metrics.Height;
            this.Ascent = metrics.Ascent;

            var rect = font.MeasureString(sprite, this.Text, DrawTextFormat.NoClip, this.Font.GetColor());

            // fix spacing at the end
            if (this.Text.EndsWith(" "))
            {
                for (int j = this.Text.Length - 1; j >= 0; j--)
                {
                    if (this.Text[j] != ' ')
                    {
                        break;
                    }

                    rect.Width += info.SpaceWidth;
                }
            }

            this.Bounds = new Rectangle(x, y, rect.Width, rect.Height);
            this.DxFont = font;
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
            this.DxFont.DrawText(
                sprite,
                this.Text,
                x + this.Bounds.X,
                y + this.Bounds.Y,
                Color.FromArgb(alpha, this.Color));
        }

        private class FontInfoExtension
        {
            private static readonly Dictionary<Microsoft.DirectX.Direct3D.Font, FontInfoExtension> Extensions = new Dictionary<Microsoft.DirectX.Direct3D.Font, FontInfoExtension>();

            private readonly Microsoft.DirectX.Direct3D.Font font;

            private int? spaceWidth;
            private TextMetric metric;

            private FontInfoExtension(Microsoft.DirectX.Direct3D.Font font)
            {
                this.font = font;
            }

            public int SpaceWidth
            {
                get
                {
                    if (!this.spaceWidth.HasValue)
                    {
                        var withSpace = this.font.MeasureString(null, "| |", DrawTextFormat.NoClip, Color.Black);
                        var withoutSpace = this.font.MeasureString(null, "||", DrawTextFormat.NoClip, Color.Black);
                        this.spaceWidth = withSpace.Width - withoutSpace.Width;
                    }

                    return this.spaceWidth.Value;
                }
            }

            public TextMetric Metrics
            {
                get
                {
                    return this.metric ?? (this.metric = TextMetric.GetTextMetricsFor(this.font));
                }
            }

            public static FontInfoExtension GetFor(Microsoft.DirectX.Direct3D.Font font)
            {
                FontInfoExtension extension;
                if (!Extensions.TryGetValue(font, out extension))
                {
                    extension = new FontInfoExtension(font);
                    Extensions.Add(font, extension);
                }

                return extension;
            }
        }
    }
}