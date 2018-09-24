// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFactoryBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextFactoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.BbCode;

    /// <summary>
    /// Base class of a factory for parsing a text and figuring out all alternatives that
    /// can be shown and creating a font and formatting definition for each part.
    /// </summary>
    /// <typeparam name="TFormattedText">
    /// The type of <see cref="FormattedText{TPart}"/> that will be created by this factory.
    /// If no special handling is required, this can be <code>FormattedText&lt;TPart&gt;</code>.
    /// </typeparam>
    /// <typeparam name="TPart">
    /// The base type of parts.
    /// </typeparam>
    public abstract class TextFactoryBase<TFormattedText, TPart>
        where TFormattedText : FormattedText<TPart>
        where TPart : IPart
    {
        private const string DefaultTimeFormat = "HH:mm";

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFactoryBase{TFormattedText,TPart}"/> class.
        /// </summary>
        protected TextFactoryBase()
        {
            this.BoldWeight = 900;
        }

        /// <summary>
        /// Gets or sets the value to be used for <see cref="Font.Weight"/> when
        /// the text is to be rendered in bold. Default value is 900.
        /// </summary>
        public int BoldWeight { get; set; }

        /// <summary>
        /// Parses the given text and returns a list of all alternatives with
        /// their respective formatting.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="baseFont">
        /// The base font from which the text parts will inherit non-overridden
        /// properties.
        /// </param>
        /// <returns>
        /// a list of all alternatives.
        /// </returns>
        public virtual AlternationList<TFormattedText, TPart> ParseAlternatives(string text, Font baseFont)
        {
            var root = new BbParser().Parse(text);

            var rootList = this.CreateFormattedText();
            rootList.AddLine(new FormattedTextLine<TPart>());

            var alternatives = new List<TFormattedText> { rootList };
            var currentAlternatives = new List<TFormattedText> { rootList };
            var interval = this.FindAlternatives(root, currentAlternatives, baseFont, alternatives);

            return new AlternationList<TFormattedText, TPart>(alternatives, interval);
        }

        /// <summary>
        /// Creates a new formatted text object to be used in alternatives.
        /// </summary>
        /// <returns>
        /// The <see cref="TFormattedText"/>.
        /// </returns>
        protected abstract TFormattedText CreateFormattedText();

        /// <summary>
        /// Create a text part.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this part should blink.
        /// </param>
        /// <returns>
        /// The new <see cref="TPart"/> which has to implement <see cref="ITextPart"/>.
        /// </returns>
        protected abstract TPart CreateTextPart(string text, Font font, bool blink);

        /// <summary>
        /// Create a time part that will render the current date/time.
        /// </summary>
        /// <param name="timeFormat">
        /// The time format (see <see cref="DateTime.ToString(string)"/>).
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this part should blink.
        /// </param>
        /// <returns>
        /// The new <see cref="TPart"/> which has to implement <see cref="ITextPart"/>.
        /// </returns>
        protected abstract TPart CreateTimePart(string timeFormat, Font font, bool blink);

        /// <summary>
        /// Create an image part.
        /// </summary>
        /// <param name="image">
        /// The image tag.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this part should blink.
        /// </param>
        /// <returns>
        /// The new <see cref="TPart"/> which has to implement <see cref="IImagePart"/>.
        /// </returns>
        protected abstract TPart CreateImagePart(Image image, bool blink);

        /// <summary>
        /// Create a video part.
        /// </summary>
        /// <param name="video">
        /// The video tag.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this part should blink.
        /// </param>
        /// <returns>
        /// The new <see cref="TPart"/> which has to implement <see cref="IVideoPart"/>.
        /// </returns>
        protected abstract TPart CreateVideoPart(Video video, bool blink);

        private TimeSpan? FindAlternatives(
            BbNode node,
            List<TFormattedText> currentAlternatives,
            Font baseFont,
            List<TFormattedText> allAlternatives)
        {
            var alt = node as Alternating;
            if (alt != null)
            {
                return this.GetAlternatives(alt, currentAlternatives, baseFont, allAlternatives);
            }

            var text = node as BbText;
            if (text != null)
            {
                this.AddText(currentAlternatives, baseFont, text);
                return null;
            }

            var time = node as Time;
            if (time != null)
            {
                this.AddTime(currentAlternatives, baseFont, time);
                return null;
            }

            var image = node as Image;
            if (image != null)
            {
                this.AddImage(currentAlternatives, image);
                return null;
            }

            var video = node as Video;
            if (video != null)
            {
                this.AddVideo(currentAlternatives, video);
                return null;
            }

            if (node is NewLine)
            {
                this.AddNewLine(currentAlternatives);
                return null;
            }

            var invert = node as Invert;
            if (invert != null)
            {
                foreach (var currentAlternative in currentAlternatives)
                {
                    currentAlternative.IsInverted = true;
                }

                return this.FindAlternatives(invert, currentAlternatives, baseFont, allAlternatives);
            }

            var verticalAlign = node as VerticalAlign;
            if (verticalAlign != null)
            {
                this.SetVerticalAlignment(currentAlternatives, verticalAlign);
                return this.FindAlternatives(verticalAlign, currentAlternatives, baseFont, allAlternatives);
            }

            var horizontalAlign = node as HorizontalAlign;
            if (horizontalAlign != null)
            {
                this.SetHorizontalAlignment(currentAlternatives, horizontalAlign);
                return this.FindAlternatives(horizontalAlign, currentAlternatives, baseFont, allAlternatives);
            }

            var branch = node as BbBranch;
            return branch == null
                       ? null
                       : this.FindAlternatives(branch, currentAlternatives, baseFont, allAlternatives);
        }

        private TimeSpan? FindAlternatives(
            BbBranch branch,
            List<TFormattedText> currentAlternatives,
            Font baseFont,
            List<TFormattedText> allAlternatives)
        {
            TimeSpan? interval = null;
            foreach (var child in branch.Children)
            {
                var subInterval = this.FindAlternatives(child, currentAlternatives, baseFont, allAlternatives);
                if (!interval.HasValue)
                {
                    interval = subInterval;
                }
            }

            return interval;
        }

        private void AddNewLine(IEnumerable<TFormattedText> currentAlternatives)
        {
            foreach (var currentAlternative in currentAlternatives)
            {
                currentAlternative.AddLine(new FormattedTextLine<TPart>());
            }
        }

        private void AddVideo(IEnumerable<TFormattedText> currentAlternatives, Video video)
        {
            var blink = video.GetClosestParent<Blink>();
            var part = this.CreateVideoPart(video, blink != null);
            this.AddPart(currentAlternatives, part);
        }

        private void AddImage(IEnumerable<TFormattedText> currentAlternatives, Image image)
        {
            var blink = image.GetClosestParent<Blink>();
            var part = this.CreateImagePart(image, blink != null);
            this.AddPart(currentAlternatives, part);
        }

        private void AddTime(IEnumerable<TFormattedText> currentAlternatives, Font baseFont, Time time)
        {
            var newFont = this.CreateFont(time, baseFont);
            var blink = time.GetClosestParent<Blink>();
            var part = this.CreateTimePart(time.TimeFormat ?? DefaultTimeFormat, newFont, blink != null);
            this.AddPart(currentAlternatives, part);
        }

        private void AddText(IEnumerable<TFormattedText> currentAlternatives, Font baseFont, BbText text)
        {
            var newFont = this.CreateFont(text, baseFont);
            var blink = text.GetClosestParent<Blink>();
            var part = this.CreateTextPart(text.Text, newFont, blink != null);
            this.AddPart(currentAlternatives, part);
        }

        private void AddPart(IEnumerable<TFormattedText> currentAlternatives, TPart part)
        {
            bool first = true;
            foreach (var currentAlternative in currentAlternatives)
            {
                if (first)
                {
                    currentAlternative.LastLine.AddPart(part);
                    first = false;
                }
                else
                {
                    currentAlternative.LastLine.AddPart((TPart)part.Duplicate());
                }
            }
        }

        private void SetHorizontalAlignment(List<TFormattedText> currentAlternatives, HorizontalAlign horizontalAlign)
        {
            HorizontalAlignment? alignment;
            switch (horizontalAlign.Alignment)
            {
                case HorizontalAlign.Align.Left:
                    alignment = HorizontalAlignment.Left;
                    break;
                case HorizontalAlign.Align.Center:
                    alignment = HorizontalAlignment.Center;
                    break;
                case HorizontalAlign.Align.Right:
                    alignment = HorizontalAlignment.Right;
                    break;
                default:
                    alignment = null;
                    break;
            }

            foreach (var currentAlternative in currentAlternatives)
            {
                currentAlternative.LastLine.HorizontalAlignment = alignment;
            }
        }

        private void SetVerticalAlignment(List<TFormattedText> currentAlternatives, VerticalAlign verticalAlign)
        {
            VerticalAlignment? alignment;
            switch (verticalAlign.Alignment)
            {
                case VerticalAlign.Align.Top:
                    alignment = VerticalAlignment.Top;
                    break;
                case VerticalAlign.Align.Middle:
                    alignment = VerticalAlignment.Middle;
                    break;
                case VerticalAlign.Align.Bottom:
                    alignment = VerticalAlignment.Bottom;
                    break;
                default:
                    alignment = null;
                    break;
            }

            foreach (var currentAlternative in currentAlternatives)
            {
                currentAlternative.VerticalAlignment = alignment;
            }
        }

        private TimeSpan? GetAlternatives(
            Alternating alt,
            List<TFormattedText> currentAlternatives,
            Font baseFont,
            List<TFormattedText> allAlternatives)
        {
            allAlternatives.RemoveAll(currentAlternatives.Contains);
            int altCount = allAlternatives.Count;
            TimeSpan? interval = null;
            if (alt.IntervalSeconds.HasValue)
            {
                interval = TimeSpan.FromSeconds(alt.IntervalSeconds.Value);
            }

            foreach (Alternation alternation in alt.Children)
            {
                var innerAlternatives = new List<TFormattedText>();
                foreach (var currentAlternative in currentAlternatives)
                {
                    var alternative = this.CreateFormattedText();
                    alternative.IsInverted = currentAlternative.IsInverted;
                    alternative.VerticalAlignment = currentAlternative.VerticalAlignment;
                    foreach (var line in currentAlternative.Lines)
                    {
                        alternative.AddLine(line.Duplicate());
                    }

                    allAlternatives.Add(alternative);
                    innerAlternatives.Add(alternative);
                }

                var subInterval = this.FindAlternatives(alternation, innerAlternatives, baseFont, allAlternatives);
                if (!interval.HasValue)
                {
                    interval = subInterval;
                }
            }

            // add all newly createdd sub-alternatives to the current alternatives
            currentAlternatives.Clear();
            for (int i = altCount; i < allAlternatives.Count; i++)
            {
                currentAlternatives.Add(allAlternatives[i]);
            }

            return interval;
        }

        private Font CreateFont(BbNode text, Font baseFont)
        {
            var face = text.GetClosestParent<Face>();
            var color = text.GetClosestParent<Color>();
            var size = text.GetClosestParent<Size>();
            var italic = text.GetClosestParent<Italic>();
            var bold = text.GetClosestParent<Bold>();
            var newFont = new Font
                              {
                                  Face = face == null ? baseFont.Face : face.FaceName,
                                  Color = color == null ? baseFont.Color : color.ColorName,
                                  Height = size == null ? baseFont.Height : size.CalculateHeight(baseFont.Height),
                                  Italic = italic != null || baseFont.Italic,
                                  Weight = bold == null ? baseFont.Weight : this.BoldWeight
                              };
            return newFont;
        }
    }
}