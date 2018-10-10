namespace HtmlRendererTest.Renderers
{
    using System;

    public class JsonUpdateEventArgs : EventArgs
    {
        public JsonUpdateEventArgs(JsonUpdate update)
        {
            this.Update = update;
        }

        public JsonUpdate Update { get; private set; }
    }
}
