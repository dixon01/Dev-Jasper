namespace XNA3RendererTest
{
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class XnaVideoPart : XnaPart, IVideoPart
    {
        public XnaVideoPart(string videoUri, bool blink)
            : base(blink)
        {
            this.VideoUri = videoUri;
        }

        public string VideoUri { get; private set; }

        public override int UpdateBounds(SpriteBatch sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            // TODO: implement
            alignOnBaseline = true;
            this.Bounds = new Rectangle(x, y, 0, 0);
            this.Ascent = this.Bounds.Height;
            return this.Bounds.Height;
        }

        public override void Render(SpriteBatch sprite, int x, int y, int alpha, IXnaRenderContext context)
        {
        }
    }
}
