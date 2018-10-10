namespace HtmlRendererTest.Renderers
{
    using Gorba.Motion.Infomedia.RendererBase;

    public interface IHtmlRenderContext : IRenderContext
    {
        string SessionId { get; }
    }
}
