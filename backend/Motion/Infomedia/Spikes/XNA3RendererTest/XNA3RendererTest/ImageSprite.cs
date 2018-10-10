using System;

namespace XNA3RendererTest
{
    using System.IO;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework.Graphics;

    using Color = Microsoft.Xna.Framework.Graphics.Color;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;

    public class ImageSprite<TItem, TEngine> : IDisposable
        where TItem : ImageItem
        where TEngine : class, IRenderEngine<IXnaRenderContext>
    {
        private readonly DrawableRenderManagerBase<TItem, IXnaRenderContext, TEngine> manager;

        private GraphicsDevice device;

        private SpriteBatch spriteBatch;

        private Texture2D texture;

        public ImageSprite(DrawableRenderManagerBase<TItem, IXnaRenderContext, TEngine> manager, GraphicsDevice device)
        {
            this.device = device;
            this.manager = manager;
            this.spriteBatch = new SpriteBatch(this.device);
        }

        public string CurrentFilename { get; private set; }

        public void Setup(string filename)
        {
            if (filename == this.CurrentFilename)
            {
                return;
            }

            this.CurrentFilename = filename;
            this.ReleaseImage();

            if (!File.Exists(filename))
            {
                return;
            }

            this.texture = Texture2D.FromFile(this.device, File.OpenRead(filename));
        }

        public virtual void Render(int alpha, IXnaRenderContext context)
        {
            if (this.spriteBatch == null || this.texture == null)
            {
                return;
            }

            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            this.spriteBatch.Draw(this.texture, new Rectangle(this.manager.X, this.manager.Y, this.manager.Width, this.manager.Height), Color.White);
            this.spriteBatch.End();
        }

        private void ReleaseImage()
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }
        }

        public void Dispose()
        {
            this.ReleaseImage();

            if (this.spriteBatch == null)
            {
                return;
            }

            this.spriteBatch.Dispose();
            this.spriteBatch = null;
        }

        public void OnResetDevice()
        {
            if (this.spriteBatch == null)
            {
                return;
            }

            this.spriteBatch.Begin(); // TODO: To verify if correct
        }
    }
}
