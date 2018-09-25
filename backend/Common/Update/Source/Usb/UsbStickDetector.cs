// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbStickDetector.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UsbStickDetector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Usb
{
    using System;

    /// <summary>
    /// Class that detects the insertion of a USB stick.
    /// </summary>
    public partial class UsbStickDetector
    {
        /// <summary>
        /// Event that is fired when a USB stick is inserted in the local system.
        /// </summary>
        public event EventHandler Inserted;

        /// <summary>
        /// Event that is fired when a USB stick is removed from the local system.
        /// </summary>
        public event EventHandler Removed;

        /// <summary>
        /// Raises the <see cref="Inserted"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseInserted(EventArgs e)
        {
            var handler = this.Inserted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Removed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseRemoved(EventArgs e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
