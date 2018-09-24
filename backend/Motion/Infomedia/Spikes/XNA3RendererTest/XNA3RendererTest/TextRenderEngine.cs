// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// </summary>
    public class TextRenderEngine : RenderEngineBase<TextItem, ITextRenderEngine<IXnaRenderContext>, TextRenderManager<IXnaRenderContext>>,
          ITextRenderEngine<IXnaRenderContext>
    {
        private TextSprite oldSprite;
        private TextSprite newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public TextRenderEngine(TextRenderManager<IXnaRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(double alpha, IXnaRenderContext context)
        {
            if (this.oldSprite == null)
            {
                this.oldSprite = new TextSprite(this.Manager, this.Device);
            }

            if (this.newSprite == null)
            {
                this.newSprite = new TextSprite(this.Manager, this.Device);
            }

            var oldText = this.Manager.Text.OldValue;
            var newText = this.Manager.Text.NewValue;

            if (oldText != null)
            {
                if (this.oldSprite.CurrentText != oldText
                    && this.newSprite.CurrentText == oldText)
                {
                    // swap sprites
                    var sprite = this.oldSprite;
                    this.oldSprite = this.newSprite;
                    this.newSprite = sprite;
                }

                this.oldSprite.Setup(oldText);
                this.oldSprite.Render((int)(255 * alpha * this.Manager.Text.OldAlpha), context);
            }

            if (newText != null)
            {
                this.newSprite.Setup(newText);
                this.newSprite.Render((int)(255 * alpha * this.Manager.Text.NewAlpha), context);
            }
        }

        /// <summary>
        /// </summary>
        protected override void OnCreateDevice()
        {
            this.Release();
        }

        /// <summary>
        /// </summary>
        protected override void OnResetDevice()
        {
            base.OnResetDevice();
            if (this.oldSprite != null)
            {
                this.oldSprite.OnResetDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnResetDevice();
            }
        }

        protected override void Release()
        {
            if (this.oldSprite != null)
            {
                this.oldSprite.Dispose();
                this.oldSprite = null;
            }

            if (this.newSprite != null)
            {
                this.newSprite.Dispose();
                this.newSprite = null;
            }
        }

    }
}
