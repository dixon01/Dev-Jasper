// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest.Renderers
{
    using System.IO;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Renderer for an image.
    /// </summary>
    public class ImageRenderer : RendererBase
    {
        private string imageUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderer"/> class.
        /// </summary>
        /// <param name="image">
        /// The image item.
        /// </param>
        public ImageRenderer(ImageItem image)
            : base(image)
        {
            image.PropertyValueChanged += this.ItemOnPropertyValueChanged;
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

        public override void Prepare()
        {
            this.LoadImage();
        }

        public override JsonDrawItem CreateJsonObject()
        {
            // replace the image URL
            var json = base.CreateJsonObject();
            var item = (ImageItem)json.Item;
            item.Filename = this.imageUrl;
            return json;
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Visible":
                    this.RaiseJsonUpdated(new JsonUpdateEventArgs(new JsonUpdate(this.Item, e.PropertyName, e.Value)));
                    break;
                case "Filename":
                    this.LoadImage();
                    this.RaiseJsonUpdated(new JsonUpdateEventArgs(new JsonUpdate(this.Item, e.PropertyName, this.imageUrl)));
                    break;
            }
        }

        private void LoadImage()
        {
            var image = (ImageItem)this.Item;
            this.imageUrl = string.IsNullOrEmpty(image.Filename)
                                ? string.Empty
                                : ImageProvider.Instance.GetPathFor(new FileInfo(image.Filename));
        }
    }
}