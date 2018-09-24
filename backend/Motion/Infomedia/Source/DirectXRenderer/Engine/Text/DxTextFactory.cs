// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;
    using Image = Gorba.Motion.Infomedia.BbCode.Image;

    /// <summary>
    /// Factory for texts creating objects of <see cref="DxPart"/> subclasses.
    /// </summary>
    public class DxTextFactory : TextFactoryBase<DxFormattedText, DxPart>
    {
        private readonly IDxDeviceRenderContext context;

        private readonly Sprite sprite;

        private readonly TextConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxTextFactory"/> class.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DxTextFactory(Sprite sprite, IDxDeviceRenderContext context)
        {
            this.sprite = sprite;
            this.context = context;

            this.config = context.Config.Text;
        }

        /// <summary>
        /// Creates a new formatted text object to be used in alternatives.
        /// </summary>
        /// <returns>
        /// A new <see cref="DxFormattedText"/>.
        /// </returns>
        protected override DxFormattedText CreateFormattedText()
        {
            return new DxFormattedText();
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
        /// A new <see cref="DxFontTextPart"/>.
        /// </returns>
        protected override DxPart CreateTextPart(string text, Font font, bool blink)
        {
            switch (this.config.TextMode)
            {
                case TextMode.Font:
                case TextMode.FontSprite:
                    return new DxFontTextPart(this.sprite, text, font, this.config.FontQuality, blink, this.context);
                case TextMode.Gdi:
                    return new DxGdiTextPart(text, font, this.config.FontQuality, blink, this.context);
                default:
                    throw new NotSupportedException("Text mode not supported: " + this.config.TextMode);
            }
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
        /// The new <see cref="DxTimePart"/>.
        /// </returns>
        protected override DxPart CreateTimePart(string timeFormat, Font font, bool blink)
        {
            return new DxTimePart(
                timeFormat, t => (DxTextPartBase)this.CreateTextPart(t, font, blink), blink, this.context);
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
        /// A new <see cref="DxImagePart"/>.
        /// </returns>
        protected override DxPart CreateImagePart(Image image, bool blink)
        {
            return new DxImagePart(image.FileName, blink, this.context);
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
        /// A new <see cref="DxVideoPart"/>.
        /// </returns>
        protected override DxPart CreateVideoPart(Video video, bool blink)
        {
            return new DxVideoPart(video.VideoUri, blink, this.context);
        }
    }
}