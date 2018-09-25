// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    using Font = Gorba.Motion.Infomedia.Entities.Font;

    /// <summary>
    /// Factory for texts creating objects of <see cref="DxPart"/> subclasses.
    /// </summary>
    public class DxTextFactory : TextFactoryBase<DxPart>
    {
        private readonly NewLinePart newLine = new NewLinePart();

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
        /// A new <see cref="DxTextPart"/>.
        /// </returns>
        protected override DxPart CreateTextPart(string text, Font font, bool blink)
        {
            return new DxTextPart(text, font, blink);
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
            return new DxImagePart(image.FileName, blink);
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
            return new DxVideoPart(video.VideoUri, blink);
        }

        private class NewLinePart : DxPart
        {
            public NewLinePart()
                : base(false)
            {
            }

            public override bool IsNewLine
            {
                get
                {
                    return true;
                }
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