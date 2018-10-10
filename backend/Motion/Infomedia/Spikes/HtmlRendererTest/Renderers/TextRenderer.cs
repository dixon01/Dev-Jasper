// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest.Renderers
{
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// Renderer for a text.
    /// </summary>
    public class TextRenderer : RendererBase
    {
        private List<FormattedText<SimplePartBase>> alternatives;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderer"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public TextRenderer(TextItem item)
            : base(item)
        {
            item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();
            this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;
        }

        /// <summary>
        /// Prepares this renderer.
        /// </summary>
        public override void Prepare()
        {
            this.LoadText();
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
            return new JsonTextItem((TextItem)this.Item.Clone(), this.alternatives);
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Visible":
                    this.RaiseJsonUpdated(new JsonUpdateEventArgs(new JsonUpdate(this.Item, e.PropertyName, e.Value)));
                    break;
                case "Text":
                case "Font":
                    this.LoadText();
                    this.RaiseJsonUpdated(new JsonUpdateEventArgs(new JsonUpdate(this.Item, "Text", this.alternatives)));
                    break;
            }
        }

        private void LoadText()
        {
            var text = (TextItem)this.Item;
            this.alternatives = new SimpleTextFactory().ParseAlternatives(text.Text, text.Font);
        }

        private class JsonTextItem : JsonDrawItem
        {
            public JsonTextItem(TextItem item, List<FormattedText<SimplePartBase>> alternatives)
                : base(item)
            {
                this.Alternatives = alternatives;

                // these two properties are only used from alternatives, not the parent object
                item.Text = null;
                item.Font = null;
            }

            /// <summary>
            /// Gets the alternative formatted texts.
            /// This property is only used by JSON and JavaScript.
            /// </summary>
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            // ReSharper disable MemberCanBePrivate.Local
            public List<FormattedText<SimplePartBase>> Alternatives { get; private set; }
            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}