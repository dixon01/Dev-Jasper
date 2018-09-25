// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedPropertyChangedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnimatedPropertyChangedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities
{
    using System.ComponentModel;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// Property changed event argument that can have an animation
    /// associated to the change of the property.
    /// </summary>
    public class AnimatedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="animation">
        /// The animation.
        /// </param>
        public AnimatedPropertyChangedEventArgs(string propertyName, object value, PropertyChangeAnimation animation)
            : base(propertyName)
        {
            this.Value = value;
            this.Animation = animation;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the animation.
        /// </summary>
        public PropertyChangeAnimation Animation { get; private set; }
    }
}