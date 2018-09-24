// -----------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.EdLtnRendererTest
{
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Protocol;
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Renderer;
    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    public class RenderManagerFactory : RenderManagerFactoryBase<ILtnRenderContext>
    {
        private readonly LtnSerialPort serialPort;

        public RenderManagerFactory(LtnSerialPort serialPort)
        {
            this.serialPort = serialPort;
        }

        protected override ITextRenderEngine<ILtnRenderContext> CreateEngine(TextRenderManager<ILtnRenderContext> manager)
        {
            var engine = new TextRenderEngine(manager);
            engine.Prepare(this.serialPort);
            return engine;
        }
    }
}
