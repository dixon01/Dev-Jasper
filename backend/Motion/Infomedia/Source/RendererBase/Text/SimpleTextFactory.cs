// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleTextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.BbCode;

    /// <summary>
    /// Factory for parsing a text and figuring out all alternatives that
    /// can be shown and creating a font and formatting definition for each part.
    /// </summary>
    public class SimpleTextFactory : TextFactoryBase<FormattedText<SimplePartBase>, SimplePartBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTextFactory"/> class.
        /// </summary>
        public SimpleTextFactory()
        {
            this.BoldWeight = 900;
        }

        /// <summary>
        /// Creates a new formatted text object to be used in alternatives.
        /// </summary>
        /// <returns>
        /// A new <code>FormattedText&lt;SimplePartBase&gt;</code>
        /// </returns>
        protected override FormattedText<SimplePartBase> CreateFormattedText()
        {
            return new FormattedText<SimplePartBase>();
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
        /// A new <see cref="SimpleTextPart"/>.
        /// </returns>
        protected override SimplePartBase CreateTextPart(string text, Font font, bool blink)
        {
            return new SimpleTextPart(text, font, blink);
        }

        /// <summary>
        /// Create a time part.
        /// </summary>
        /// <param name="timeFormat">
        /// The time format.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this part should blink.
        /// </param>
        /// <returns>
        /// A new <see cref="SimpleTimePart"/>.
        /// </returns>
        protected override SimplePartBase CreateTimePart(string timeFormat, Font font, bool blink)
        {
            return new SimpleTimePart(timeFormat, font, blink);
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
        /// A new <see cref="SimpleImagePart"/>.
        /// </returns>
        protected override SimplePartBase CreateImagePart(Image image, bool blink)
        {
            return new SimpleImagePart(image.FileName, blink);
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
        /// A new <see cref="SimpleVideoPart"/>.
        /// </returns>
        protected override SimplePartBase CreateVideoPart(Video video, bool blink)
        {
            return new SimpleVideoPart(video.VideoUri, blink);
        }
    }
}