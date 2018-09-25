// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPool.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPool type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    /// <summary>
    /// A pool that allows to cycle through its items.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item in the pool.
    /// </typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Event that is fired whenever <see cref="CurrentItemValid"/> changes.
        /// </summary>
        event EventHandler CurrentItemValidChanged;

        /// <summary>
        /// Gets the current item.
        /// </summary>
        T CurrentItem { get; }

        /// <summary>
        /// Gets a value indicating whether the current item is still valid.
        /// An item can become invalid because some conditions change. The pool won't
        /// move to the next item, so <see cref="CurrentItem"/> will still be the same,
        /// but <see cref="CurrentItemValid"/> will become false (and <see cref="CurrentItemValidChanged"/>
        /// will be fired).
        /// </summary>
        bool CurrentItemValid { get; }

        /// <summary>
        /// Moves to the next item.
        /// </summary>
        /// <param name="wrapAround">
        /// A flag indicating if the method should wrap around
        /// when it gets to the end of the pool or return false.
        /// </param>
        /// <returns>
        /// A flag indicating if there was a next item found.
        /// If this method returns false, <see cref="CurrentItem"/> is null.
        /// </returns>
        bool MoveNext(bool wrapAround);
    }
}