// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Provides a base implementation of the <see cref="IWindowController"/>.
    /// </summary>
    public abstract class WindowControllerBase : SynchronizableControllerBase, IWindowController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowControllerBase"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        protected WindowControllerBase(IWindowViewModel window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            this.Window = window;
        }

        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        public event EventHandler WindowClosed;

        /// <summary>
        /// Occurs when the window is being closed.
        /// </summary>
        public event CancelEventHandler WindowClosing;

        /// <summary>
        /// Gets the window view model.
        /// </summary>
        public virtual IWindowViewModel Window { get; private set; }

        /// <summary>
        /// Shows the window and returns without waiting for the opened window to close.
        /// </summary>
        public virtual void Show()
        {
            this.Window.Show();
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public virtual void Close()
        {
            var cancelEventArgs = new CancelEventArgs();
            this.RaiseWindowClosing(cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return;
            }

            this.Window.Close();
            this.RaiseWindowClosed();
        }

        /// <summary>
        /// Raises the <see cref="WindowClosed"/> event.
        /// </summary>
        protected virtual void RaiseWindowClosed()
        {
            var handler = this.WindowClosed;
            if (handler == null)
            {
                return;
            }

            handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="WindowClosing"/> event.
        /// </summary>
        /// <param name="cancelEventArgs">
        /// The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void RaiseWindowClosing(CancelEventArgs cancelEventArgs)
        {
            var handler = this.WindowClosing;
            if (handler == null)
            {
                return;
            }

            handler(this, cancelEventArgs);
        }
    }
}
