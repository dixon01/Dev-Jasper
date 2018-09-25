// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextComponent.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextComponent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Parts;
    using Gorba.Motion.Infomedia.RendererBase.Layouting;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;

    /// <summary>
    /// The text component.
    /// </summary>
    public class TextComponent : ComponentBase
    {
        private LayoutManager<PartBase> layoutManager;

        private AlternationList<FormattedText<PartBase>, PartBase> alternatives;

        private int lastAltIndex;

        private IFont[] fonts;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment Align { get; set; }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        public VerticalAlignment VAlign { get; set; }

        /// <summary>
        /// Gets or sets the text overflow.
        /// </summary>
        public TextOverflow Overflow { get; set; }

        /// <summary>
        /// Gets or sets the scroll speed.
        /// </summary>
        public int ScrollSpeed { get; set; }

        /// <summary>
        /// Gets or sets the plain text (including BB code).
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets a value indicating whether the text should be rendered inverted (black on amber).
        /// </summary>
        public bool IsInverted { get; private set; }

        /// <summary>
        /// Does the layout (if needed) of all parts of this <see cref="TextComponent"/>
        /// and returns the parts that were laid out.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <returns>
        /// The list of all <see cref="PartBase"/> with the correct coordinates.
        /// </returns>
        public IEnumerable<PartBase> GetLayoutParts(IAhdlcRenderContext context)
        {
            if (this.fonts == null && this.Font != null && !string.IsNullOrEmpty(this.Font.Face))
            {
                var fontFaces = this.Font.Face.Split(';');
                this.fonts = ArrayUtil.ConvertAll(
                    fontFaces,
                    f =>
                    context.GetFont(
                        new Font
                        {
                            Face = f,
                            CharSpacing = this.Font.CharSpacing,
                            Color = this.Font.Color,
                            Height = this.Font.Height,
                            Italic = this.Font.Italic,
                            OutlineColor = this.Font.OutlineColor,
                            Weight = this.Font.Weight
                        }));
            }

            if (this.alternatives == null)
            {
                var textFactory = new TextPartFactory();
                this.alternatives = textFactory.ParseAlternatives(this.Text, this.Font ?? new Font());
            }

            if (this.alternatives.Count == 0)
            {
                return new PartBase[0];
            }

            if (this.alternatives.Count > 1 && !context.AlternationInterval.HasValue)
            {
                context.AlternationInterval = this.alternatives.Interval ?? context.Config.Text.AlternationInterval;
            }

            var altIndex = context.AlternationCounter % this.alternatives.Count;
            if (this.layoutManager != null && this.lastAltIndex == altIndex)
            {
                return this.layoutManager.Items;
            }

            this.lastAltIndex = altIndex;
            var alternative = this.alternatives[altIndex];
            this.IsInverted = alternative.IsInverted;

            var verticalAlign = alternative.VerticalAlignment ?? this.VAlign;
            if (this.Overflow == TextOverflow.Scale || this.Overflow == TextOverflow.WrapScale)
            {
                this.layoutManager = this.LayoutBestFont(alternative.Lines, verticalAlign);
            }
            else if (this.fonts != null && this.fonts.Length > 0)
            {
                this.layoutManager = this.DoLayout(alternative.Lines, this.fonts[0], verticalAlign, this.Overflow);
            }
            else
            {
                return new PartBase[0];
            }

            return this.layoutManager.Items;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as TextComponent;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }

            return this.Align == other.Align
                   && this.VAlign == other.VAlign
                   && this.Overflow == other.Overflow
                   && this.ScrollSpeed == other.ScrollSpeed
                   && this.Text == other.Text
                   && AreEqual(this.Font, other.Font);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Text.GetHashCode() ^ base.GetHashCode();
        }

        private static bool AreEqual(Font left, Font right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            return left.Face == right.Face
                   && left.Height == right.Height
                   && left.CharSpacing == right.CharSpacing
                   && left.Color == right.Color
                   && left.OutlineColor == right.OutlineColor;
        }

        private LayoutManager<PartBase> LayoutBestFont(
            ICollection<FormattedTextLine<PartBase>> lines, VerticalAlignment verticalAlignment)
        {
            if (this.fonts.Length == 1)
            {
                // only one font is available, so we need to just work with that
                return this.DoLayout(
                    lines,
                    this.fonts[0],
                    verticalAlignment,
                    this.Overflow == TextOverflow.Scale ? TextOverflow.Clip : TextOverflow.Wrap);
            }

            var overflow = this.Overflow == TextOverflow.Scale ? TextOverflow.Extend : TextOverflow.Wrap;
            LayoutRating minValidRating = null;
            LayoutManager<PartBase> minValidLayout = null;
            LayoutRating maxRatingBelowZero = null;
            LayoutManager<PartBase> maxLayoutBelowZero = null;

            foreach (var font in this.fonts)
            {
                var layout = this.DoLayout(lines, font, verticalAlignment, overflow);
                var rating = this.RateLayout(layout);
                if (rating.Fits)
                {
                    if (minValidRating == null || rating.IsBetterThan(minValidRating))
                    {
                        minValidRating = rating;
                        minValidLayout = layout;
                    }
                }
                else
                {
                    if (maxRatingBelowZero == null || rating.IsBetterThan(maxRatingBelowZero))
                    {
                        maxRatingBelowZero = rating;
                        maxLayoutBelowZero = layout;
                    }
                }
            }

            // take the layout using the most space (i.e. smallest value above zero) or
            // take the layout using the least space outside the bounds (below zero) if none match
            return minValidLayout ?? maxLayoutBelowZero;
        }

        private LayoutRating RateLayout(LayoutManager<PartBase> layout)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;

            foreach (var part in layout.Items)
            {
                minX = Math.Min(part.X, minX);
                maxX = Math.Max(part.X + part.Width, maxX);
                minY = Math.Min(part.Y, minY);
                maxY = Math.Max(part.Y + part.Height, maxY);
            }

            var width = maxX - minX;
            var height = maxY - minY;
            if (width <= 0 || height <= 0)
            {
                return new LayoutRating(0, 0);
            }

            return new LayoutRating(this.Width - width, this.Height - height);
        }

        private LayoutManager<PartBase> DoLayout(
            IEnumerable<FormattedTextLine<PartBase>> lines,
            IFont font,
            VerticalAlignment verticalAlign,
            TextOverflow overflow)
        {
            var layout = new LayoutManager<PartBase>();
            foreach (var line in lines)
            {
                layout.AddLine(line.Duplicate().Parts, line.HorizontalAlignment);
            }

            foreach (var part in layout.Items)
            {
                var text = part as TextPart;
                if (text != null)
                {
                    text.BitmapFont = font;
                }

                part.SetScaling(1.0);
            }

            layout.Layout(
                new Rectangle(this.X, this.Y, this.Width, this.Height),
                this.Align,
                verticalAlign,
                overflow,
                true);
            return layout;
        }

        private class LayoutRating
        {
            private readonly int availableSpaceX;
            private readonly int availableSpaceY;

            public LayoutRating(int availableSpaceX, int availableSpaceY)
            {
                this.availableSpaceX = availableSpaceX;
                this.availableSpaceY = availableSpaceY;
            }

            public bool Fits
            {
                get
                {
                    return this.availableSpaceX >= 0 && this.availableSpaceY >= 0;
                }
            }

            public bool IsBetterThan(LayoutRating other)
            {
                if (this.availableSpaceX >= 0 && other.availableSpaceX >= 0)
                {
                    if (this.availableSpaceY >= 0 && other.availableSpaceY >= 0)
                    {
                        if (this.availableSpaceX == other.availableSpaceX)
                        {
                            return this.availableSpaceY < other.availableSpaceY;
                        }

                        return this.availableSpaceX < other.availableSpaceX;
                    }

                    if (this.availableSpaceY >= 0 || other.availableSpaceY >= 0)
                    {
                        return this.availableSpaceY >= 0;
                    }

                    return this.availableSpaceX - this.availableSpaceY < other.availableSpaceX - other.availableSpaceY;
                }

                if (this.availableSpaceX >= 0 || other.availableSpaceX >= 0)
                {
                    return this.availableSpaceX >= 0;
                }

                if (this.availableSpaceX == other.availableSpaceX)
                {
                    return this.availableSpaceY > other.availableSpaceY;
                }

                return this.availableSpaceX > other.availableSpaceX;
            }
        }
    }
}
