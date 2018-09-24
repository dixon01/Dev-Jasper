// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;

    /// <summary>
    /// Base class for items in the <see cref="AudioPlayer"/>.
    /// </summary>
    internal abstract class AudioItemBase : IDisposable
    {
        /// <summary>
        /// Event that is fired when this item has completed playing.
        /// This event might also be risen when <see cref="Stop"/> is called.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Start playing this item.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Immediately stop playing this item.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();

        /// <summary>
        /// Raises the <see cref="Completed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCompleted(EventArgs e)
        {
            var handler = this.Completed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}