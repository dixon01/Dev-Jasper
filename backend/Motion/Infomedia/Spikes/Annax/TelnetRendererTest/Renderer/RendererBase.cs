namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Renderer
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.AnnaxRendererTest.Commands;
    using Gorba.Motion.Infomedia.Entities.Screen;

    public abstract class RendererBase : IDisposable
    {
        protected RendererBase(DrawableItemBase item)
        {
            this.Item = item;
        }

        public DrawableItemBase Item { get; private set; }

        public static RendererBase Create(ScreenItemBase item)
        {
            var text = item as TextItem;
            if (text != null)
            {
                return new TextRenderer(text);
            }

            throw new NotSupportedException(item.GetType().Name + " can't be rendered for Annax");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        public abstract IEnumerable<CommandBase> Setup(IRenderContext context);

        public abstract IEnumerable<CommandBase> Update(ItemUpdate update, IRenderContext context);
    }
}
