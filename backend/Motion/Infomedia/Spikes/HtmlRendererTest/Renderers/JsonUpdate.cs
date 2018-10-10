// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest.Renderers
{
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// A property update that can be JSON-serialized.
    /// </summary>
    public class JsonUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonUpdate"/> class.
        /// </summary>
        /// <param name="item">
        /// The item that was updated.
        /// </param>
        /// <param name="property">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public JsonUpdate(DrawableItemBase item, string property, object value)
        {
            this.Id = item.Id;
            this.Type = item.GetType().Name;
            this.Property = property;
            this.Value = value;
        }

        /// <summary>
        /// Gets the id of the item.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Property { get; private set; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public object Value { get; private set; }
    }
}
