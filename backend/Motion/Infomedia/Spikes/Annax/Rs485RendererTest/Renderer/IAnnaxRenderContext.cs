namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Renderer
{
    using Gorba.Motion.Infomedia.RendererBase;

    public interface IAnnaxRenderContext : IRenderContext
    {
        int GetAvailableBitmapNumber();
    }
}