namespace HtmlRendererTest.Renderers
{
    using Gorba.Motion.Infomedia.Entities.Screen;

    public class JsonDrawItem
    {
        public JsonDrawItem(DrawableItemBase item)
        {
            this.Item = item;
            this.Type = item.GetType().Name;
        }

        protected JsonDrawItem(string type)
        {
            this.Type = type;
        }

        public string Type { get; private set; }

        public DrawableItemBase Item { get; private set; }
    }
}