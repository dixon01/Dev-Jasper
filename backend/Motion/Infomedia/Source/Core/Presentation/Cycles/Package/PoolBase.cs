// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PoolBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for pools that implements all general methods and properties.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item in the pool.
    /// </typeparam>
    public abstract class PoolBase<T> : IPool<T>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private bool currentItemValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolBase{T}"/> class.
        /// </summary>
        protected PoolBase()
        {
            this.currentItemValid = true;

            this.Logger = LogHelper.GetLogger(this.GetType());
        }

        /// <summary>
        /// Event that is fired whenever <see cref="IPool{T}.CurrentItemValid"/> changes.
        /// </summary>
        public event EventHandler CurrentItemValidChanged;

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        public T CurrentItem { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current item is still valid.
        /// An item can become invalid because some conditions change. The pool won't
        /// move to the next item, so <see cref="IPool{T}.CurrentItem"/> will still be the same,
        /// but <see cref="CurrentItemValid"/> will become false (and <see cref="CurrentItemValidChanged"/>
        /// will be fired).
        /// </summary>
        public bool CurrentItemValid
        {
            get
            {
                return this.currentItemValid;
            }

            protected set
            {
                if (this.currentItemValid == value)
                {
                    return;
                }

                this.currentItemValid = value;
                this.RaiseCurrentItemValidChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Moves to the next item.
        /// </summary>
        /// <param name="wrapAround">
        /// A flag indicating if the method should wrap around
        /// when it gets to the end of the pool or return false.
        /// </param>
        /// <returns>
        /// A flag indicating if there was a next item found.
        /// If this method returns false, <see cref="IPool{T}.CurrentItem"/> is null.
        /// </returns>
        public abstract bool MoveNext(bool wrapAround);

        /// <summary>
        /// Raises the <see cref="CurrentItemValidChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCurrentItemValidChanged(EventArgs e)
        {
            var handler = this.CurrentItemValidChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}