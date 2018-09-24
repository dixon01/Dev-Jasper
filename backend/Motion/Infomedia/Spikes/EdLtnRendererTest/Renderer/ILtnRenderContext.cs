namespace Gorba.Motion.Infomedia.EdLtnRendererTest.Renderer
{
    using Gorba.Motion.Infomedia.RendererBase;

    public interface ILtnRenderContext : IRenderContext
    {
        RenderState State { get; }

        byte GetNextCellNumber();
    }

    public enum RenderState
    {
        Setup,
        Update
    }
}
