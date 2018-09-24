// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPresentableComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPresentableComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Interface to be implemented by all <see cref="IComposer"/> subclasses that have
    /// a <see cref="ScreenItemBase"/> as output. Composite composers usually don't implement this interface.
    /// </summary>
    internal interface IPresentableComposer : IComposer
    {
        /// <summary>
        /// Event that is fired whenever a property of the <see cref="Item"/>
        /// changes.
        /// </summary>
        event EventHandler<AnimatedItemPropertyChangedEventArgs> ItemPropertyValueChanged;

        /// <summary>
        /// Gets the item that is output by this composer.
        /// This shouldn't change after the composer has been constructed and it should never return null.
        /// </summary>
        ScreenItemBase Item { get; }
    }
}