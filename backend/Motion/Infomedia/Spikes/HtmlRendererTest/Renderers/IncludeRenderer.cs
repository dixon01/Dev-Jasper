// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncludeRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IncludeRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest.Renderers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Renderer for a <see cref="IncludeItem"/>.
    /// It contains all the renderers of the included screen 
    /// and delegates all methods to them.
    /// </summary>
    public class IncludeRenderer : RendererBase
    {
        private readonly List<RendererBase> renderers = new List<RendererBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeRenderer"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public IncludeRenderer(IncludeItem item)
            : base(item)
        {
            this.InitRenderers(item.Include);
            item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;

            this.ClearRenderers();
        }

        public override void Prepare()
        {
            foreach (var renderer in this.renderers)
            {
                renderer.Prepare();
            }
        }

        /// <summary>
        /// Creates the JSON object that will be sent to the browser
        /// </summary>
        /// <returns>
        /// an object that is JSON-serializable that contains all information about
        /// this renderer.
        /// </returns>
        public override JsonDrawItem CreateJsonObject()
        {
            var json = new JsonIncludeItem((IncludeItem)this.Item);
            json.Items = this.CreateIncludeItems();

            return json;
        }

        /// <summary>
        /// Send an <see cref="ItemUpdate"/> to the 
        /// <see cref="RendererBase.Item"/> if this update is meant for it.
        /// </summary>
        /// <param name="update">
        /// The update.
        /// </param>
        public override void UpdateItem(ItemUpdate update)
        {
            base.UpdateItem(update);

            lock (((ICollection)this.renderers).SyncRoot)
            {
                foreach (var renderer in this.renderers)
                {
                    renderer.UpdateItem(update);
                }
            }
        }

        private void InitRenderers(RootItem root)
        {
            lock (((ICollection)this.renderers).SyncRoot)
            {
                foreach (var item in root.Items)
                {
                    var renderer = RendererBase.Create(item);
                    renderer.JsonUpdated += this.RendererOnJsonUpdated;
                    this.renderers.Add(renderer);
                }
            }
        }

        private void ClearRenderers()
        {
            RendererBase[] oldRenderers;

            lock (((ICollection)this.renderers).SyncRoot)
            {
                oldRenderers = this.renderers.ToArray();
                this.renderers.Clear();
            }

            foreach (var renderer in oldRenderers)
            {
                renderer.JsonUpdated -= this.RendererOnJsonUpdated;
                renderer.Dispose();
            }
        }

        private List<JsonDrawItem> CreateIncludeItems()
        {
            var items = new List<JsonDrawItem>(this.renderers.Count);
            foreach (var renderer in this.renderers)
            {
                items.Add(renderer.CreateJsonObject());
            }

            return items;
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Include")
            {
                return;
            }

            this.ClearRenderers();
            this.InitRenderers((RootItem)e.Value);
            this.Prepare();
            this.RaiseJsonUpdated(
                new JsonUpdateEventArgs(new JsonUpdate(this.Item, "Items", this.CreateIncludeItems())));
        }

        private void RendererOnJsonUpdated(object sender, JsonUpdateEventArgs e)
        {
            // forward the event
            this.RaiseJsonUpdated(e);
        }

        private class JsonIncludeItem : JsonDrawItem
        {
            public JsonIncludeItem(IncludeItem item)
                : base(item.GetType().Name)
            {
                this.Id = item.Id;
                this.Items = new List<JsonDrawItem>();
            }

            public int Id { get; private set; }

            public List<JsonDrawItem> Items { get; set; }
        }
    }
}