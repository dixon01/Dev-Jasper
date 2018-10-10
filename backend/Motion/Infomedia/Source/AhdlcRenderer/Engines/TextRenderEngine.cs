// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The <see cref="ITextRenderEngine{TContext}"/> implementation for AHDLC.
    /// </summary>
    public class TextRenderEngine :
        RenderEngineBase<TextItem,
            ITextRenderEngine<IAhdlcRenderContext>,
            TextRenderManager<IAhdlcRenderContext>>,
        ITextRenderEngine<IAhdlcRenderContext>
    {
        private TextComponent lastComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public TextRenderEngine(TextRenderManager<IAhdlcRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.DisposeLastText();
            base.Dispose();
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(IAhdlcRenderContext context)
        {
            var component = this.CreateComponent<TextComponent>();
            component.Font = this.Manager.Font;
            component.Align = this.Manager.Align;
            component.VAlign = this.Manager.VAlign;
            component.Overflow = this.Manager.Overflow;
            component.ScrollSpeed = this.Manager.ScrollSpeed;
            component.Text = this.Manager.Text.NewValue;

            context.AddItem(component, !component.Equals(this.lastComponent));

            this.DisposeLastText();
            this.lastComponent = component;
        }

        private void DisposeLastText()
        {
            if (this.lastComponent != null)
            {
                this.lastComponent.Dispose();
            }
        }
    }
}