// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedItemPropertyChangedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnimatedItemPropertyChangedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// <see cref="AnimatedPropertyChangedEventArgs"/> subclass that also contains
    /// the item of which the property changed.
    /// </summary>
    public class AnimatedItemPropertyChangedEventArgs : AnimatedPropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedItemPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="animation">
        /// The animation.
        /// </param>
        public AnimatedItemPropertyChangedEventArgs(
            ScreenItemBase item, string propertyName, object value, PropertyChangeAnimation animation)
            : base(propertyName, value, animation)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the item of which the property has changed.
        /// </summary>
        public ScreenItemBase Item { get; private set; }
    }
}