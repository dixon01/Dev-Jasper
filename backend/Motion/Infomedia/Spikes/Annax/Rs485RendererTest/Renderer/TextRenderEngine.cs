namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Renderer
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    public class TextRenderEngine
        : RenderEngineBase<TextItem, ITextRenderEngine<IAnnaxRenderContext>, TextRenderManager<IAnnaxRenderContext>>,
          ITextRenderEngine<IAnnaxRenderContext>
    {
        public TextRenderEngine(TextRenderManager<IAnnaxRenderContext> manager)
            : base(manager)
        {
        }

        public override void Render(double alpha, IAnnaxRenderContext context)
        {
        }
    }
}