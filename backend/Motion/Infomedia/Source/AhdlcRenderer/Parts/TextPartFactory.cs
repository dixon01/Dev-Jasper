// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextPartFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextPartFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Parts
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// The text part factory for <see cref="PartBase"/>.
    /// </summary>
    internal class TextPartFactory : TextFactoryBase<FormattedText<PartBase>, PartBase>
    {
        /// <summary>
        /// Creates a new formatted text object to be used in alternatives.
        /// </summary>
        /// <returns>
        /// The <see cref="FormattedText{TPart}"/>.
        /// </returns>
        protected override FormattedText<PartBase> CreateFormattedText()
        {
            return new FormattedText<PartBase>();
        }

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
        /// The new <see cref="PartBase"/> which has to implement <see cref="ITextPart"/>.
        /// </returns>
        protected override PartBase CreateTextPart(string text, Font font, bool blink)
        {
            return new TextPart(text, font);
        }

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
        /// The new <see cref="PartBase"/> which has to implement <see cref="ITextPart"/>.
        /// </returns>
        protected override PartBase CreateTimePart(string timeFormat, Font font, bool blink)
        {
            return new TextPart(TimeProvider.Current.Now.ToString(timeFormat), font);
        }

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
        /// The new <see cref="PartBase"/> which has to implement <see cref="IImagePart"/>.
        /// </returns>
        protected override PartBase CreateImagePart(Image image, bool blink)
        {
            return new ImagePart(image.FileName);
        }

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
        /// The new <see cref="PartBase"/> which has to implement <see cref="IVideoPart"/>.
        /// </returns>
        protected override PartBase CreateVideoPart(Video video, bool blink)
        {
            throw new NotSupportedException("Videos are not supported in AHDLC");
        }
    }
}