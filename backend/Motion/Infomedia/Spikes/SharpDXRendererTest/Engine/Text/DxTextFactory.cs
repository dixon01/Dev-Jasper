// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text
{
    using System;

    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.RendererBase.Text;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Config;

    using SharpDX.Direct3D9;

    using Font = Gorba.Motion.Infomedia.Entities.Font;

    /// <summary>
    /// Factory for texts creating objects of <see cref="DxPart"/> subclasses.
    /// </summary>
    public class DxTextFactory : TextFactoryBase<DxPart>
    {
        private static readonly TextConfig Config = ConfigService.Instance.Config.Text;

        private readonly Device device;

        private readonly NewLinePart newLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxTextFactory"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public DxTextFactory(Device device)
        {
            this.device = device;
            this.newLine = new NewLinePart(device);
        }

        /// <summary>
        /// Gets a unique <see cref="DxPart"/> that represents a new-line.
        /// </summary>
        public override DxPart NewLine
        {
            get
            {
                return this.newLine;
            }
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
            switch (Config.TextMode)
            {
                case TextMode.Font:
                case TextMode.FontSprite:
                    return new DxFontTextPart(text, font, Config.FontQuality, blink, this.device);
                case TextMode.Mesh:
                    //return new DxMeshTextPart(text, font, blink, this.device);
                case TextMode.Gdi:
                    //return new DxGdiTextPart(text, font, blink, this.device);
                default:
                    throw new NotSupportedException("Text mode not supported: " + Config.TextMode);
            }
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
            return new DxImagePart(image.FileName, blink, this.device);
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
            return new DxVideoPart(video.VideoUri, blink, this.device);
        }

        private class NewLinePart : DxPart
        {
            public NewLinePart(Device device)
                : base(false, device)
            {
            }

            public override bool IsNewLine
            {
                get
                {
                    return true;
                }
            }

            public override IPart Duplicate()
            {
                // we are singleton
                return this;
            }

            public override void Render(Sprite sprite, int x, int y, int alpha, IDxRenderContext context)
            {
                // nothing to render
            }

            public override int UpdateBounds(Sprite sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
            {
                // a new-line has no bounds
                return 0;
            }
        }
    }
}