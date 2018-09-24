// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text
{
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The text render engine.
    /// </summary>
    public class TextRenderEngine
        : RenderEngineBase<TextItem, ITextRenderEngine<IDxRenderContext>, TextRenderManager<IDxRenderContext>>,
          ITextRenderEngine<IDxRenderContext>
    {
        private TextSprite oldSprite;
        private TextSprite newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public TextRenderEngine(TextRenderManager<IDxRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Test if this engine should render.
        /// Returns only true if this object is within the virtual 
        /// viewport of the screen.
        /// </summary>
        /// <returns>
        /// True if this object is within the virtual viewport of the screen.
        /// </returns>
        protected override bool ShouldRender()
        {
            if (this.Manager.Overflow == TextOverflow.Clip
                || this.Manager.Overflow == TextOverflow.Scroll
                || this.Manager.Overflow == TextOverflow.ScrollAlways)
            {
                return base.ShouldRender();
            }

            // special case: we don't know our width and height,
            // therefore we have to render if we might be visible
            return this.Manager.X <= this.Viewport.Right && this.Manager.Y <= this.Viewport.Bottom;
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(double alpha, IDxRenderContext context)
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
                this.oldSprite.Render(this.Viewport, (int)(255 * alpha * this.Manager.Text.OldAlpha), context);
            }

            if (newText != null)
            {
                this.newSprite.Setup(newText);
                this.newSprite.Render(this.Viewport, (int)(255 * alpha * this.Manager.Text.NewAlpha), context);
            }
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
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

        /// <summary>
        /// Override this method to be notified when a device is lost.
        /// If you have any DirectX resources, make sure to call
        /// OnLostDevice() on them if available.
        /// </summary>
        protected override void OnLostDevice()
        {
            base.OnLostDevice();
            if (this.oldSprite != null)
            {
                this.oldSprite.OnLostDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnLostDevice();
            }
        }

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
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