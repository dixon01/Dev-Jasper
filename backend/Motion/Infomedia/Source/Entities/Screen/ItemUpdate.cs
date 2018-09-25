// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// A single update of a property of a screen item.
    /// </summary>
    public class ItemUpdate
    {
        /// <summary>
        /// Gets or sets the unique id of the screen item that is being updated.
        /// </summary>
        public int ScreenItemId { get; set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets value of the property.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the animation to be used when this property changes.
        /// If this is null, the property should not be animated.
        /// </summary>
        public PropertyChangeAnimation Animation { get; set; }
    }
}