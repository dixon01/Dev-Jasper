// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadableModelEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadableModelEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Event arguments that provide a <see cref="ReadableModelBase"/> implementation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of readable model returned by this event.
    /// </typeparam>
    public class ReadableModelEventArgs<T> : EventArgs
        where T : ReadableModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadableModelEventArgs{T}"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public ReadableModelEventArgs(T model)
        {
            this.Model = model;
        }

        /// <summary>
        /// Gets the readable model.
        /// </summary>
        public T Model { get; private set; }
    }
}
