// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RendererBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest.Renderers
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Base class for rendering classes that render a given <see cref="DrawableItemBase"/>.
    /// </summary>
    public abstract class RendererBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        protected RendererBase(DrawableItemBase item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Event that is fired if the JSON properties of this renderer have changed.
        /// </summary>
        public event EventHandler<JsonUpdateEventArgs> JsonUpdated;

        /// <summary>
        /// Gets the draw item being rendered by this class.
        /// </summary>
        public DrawableItemBase Item { get; private set; }

        /// <summary>
        /// Creates a new <see cref="RendererBase"/> for the given
        /// <see cref="DrawableItemBase"/>.
        /// </summary>
        /// <param name="item">
        /// The item to be rendered.
        /// </param>
        /// <returns>
        /// a new renderer for the given item.
        /// </returns>
        public static RendererBase Create(ScreenItemBase item)
        {
            var text = item as TextItem;
            if (text != null)
            {
                return new TextRenderer(text);
            }

            var image = item as ImageItem;
            if (image != null)
            {
                return new ImageRenderer(image);
            }

            var analogClock = item as AnalogClockItem;
            if (analogClock != null)
            {
                return new AnalogClockRenderer(analogClock);
            }

            var include = item as IncludeItem;
            if (include != null)
            {
                return new IncludeRenderer(include);
            }

            throw new NotSupportedException("Could not create renderer for " + item.GetType().FullName);
        }

        /// <summary>
        /// Send an <see cref="ItemUpdate"/> to the
        /// <see cref="Item"/> if this update is meant for it.
        /// </summary>
        /// <param name="update">
        /// The update.
        /// </param>
        public virtual void UpdateItem(ItemUpdate update)
        {
            if (update.ScreenItemId != this.Item.Id)
            {
                return;
            }

            this.Item.Update(update);
        }

        /// <summary>
        /// Creates the JSON object that will be sent to the browser
        /// </summary>
        /// <returns>
        /// an object that is JSON-serializable that contains all information about
        /// this renderer.
        /// </returns>
        public virtual JsonDrawItem CreateJsonObject()
        {
            return new JsonDrawItem((DrawableItemBase)this.Item.Clone());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Prepares this renderer.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        /// Raises the <see cref="JsonUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseJsonUpdated(JsonUpdateEventArgs e)
        {
            var handler = this.JsonUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
