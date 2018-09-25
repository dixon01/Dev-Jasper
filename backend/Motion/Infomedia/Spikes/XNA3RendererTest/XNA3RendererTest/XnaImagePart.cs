namespace XNA3RendererTest
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.Xna.Framework.Graphics;

    public class XnaImagePart : XnaPart, IImagePart
    {
        private readonly Bitmap bitmap;

        public XnaImagePart(string fileName, bool blink)
            : base(blink)
        {
            this.FileName = fileName;
            this.bitmap = new Bitmap(fileName);
        }

        public string FileName { get; private set; }

        public override int UpdateBounds(SpriteBatch sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(SpriteBatch sprite, int x, int y, int alpha, IXnaRenderContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
