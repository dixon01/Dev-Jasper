// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XnaTextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.Xna.Framework.Graphics;

    public class XnaTextFactory : TextFactoryBase<XnaPart>
    {
        private readonly NewLinePart newLine = new NewLinePart();

        public override XnaPart NewLine
        {
            get
            {
                return this.newLine;
            }
        }

        protected override XnaPart CreateTextPart(string text, Font font, bool blink)
        {
            return new XnaTextPart(text, font, blink);
        }

        protected override XnaPart CreateImagePart(Image image, bool blink)
        {
            return new XnaImagePart(image.FileName, blink);
        }

        protected override XnaPart CreateVideoPart(Video video, bool blink)
        {
            return new XnaVideoPart(video.VideoUri, blink);
        }

        private class NewLinePart : XnaPart
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

            public override void Render(SpriteBatch sprite, int x, int y, int alpha, IXnaRenderContext context)
            {
                // nothing to render
            }

            public override int UpdateBounds(SpriteBatch sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
            {
                // a new-line has no bounds
                return 0;
            }
        }
    }
}
