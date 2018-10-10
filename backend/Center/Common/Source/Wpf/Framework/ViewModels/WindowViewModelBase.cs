// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Defines the base abstract ViewModel class for windows.
    /// </summary>
    public abstract class WindowViewModelBase : DialogParentViewModelBase, IWindowViewModel
    {
        private readonly IWindowFactory windowFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewModelBase"/> class.
        /// </summary>
        /// <param name="factory">The view.</param>
        protected WindowViewModelBase(IWindowFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            this.windowFactory = factory;
        }

        /// <summary>
        /// Occurs when the window is created.
        /// </summary>
        public event EventHandler Created;

        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when the window is being closed.
        /// </summary>
        public event CancelEventHandler Closing;

        /// <summary>
        /// Gets the window.
        /// </summary>
        protected IWindowView WindowView { get; private set; }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Shows the window and returns without waiting for the newly opened window to close.
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Raises the Created event.
        /// </summary>
        protected virtual void RaiseCreated()
        {
            var handler = this.Created;
            if (handler == null)
            {
                return;
            }

            handler.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the Closed event.
        /// </summary>
        protected virtual void RaiseClosed()
        {
            var handler = this.Closed;
            if (handler == null)
            {
                return;
            }

            handler.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="cancelEventArgs">The <see cref="CancelEventArgs"/>.</param>
        protected virtual void RaiseClosing(CancelEventArgs cancelEventArgs)
        {
            if (cancelEventArgs.Cancel)
            {
                return;
            }

            var handler = this.Closing;
            if (handler == null)
            {
                return;
            }

            handler.Invoke(this, cancelEventArgs);
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        protected virtual void CreateWindow()
        {
            this.WindowView = this.windowFactory.Create(this);
            this.Window = (Window)this.WindowView;
            this.Window.SizeChanged += this.WindowOnSizeChanged;

            if (!object.ReferenceEquals(this, this.WindowView.DataContext))
            {
                throw new InvalidOperationException("The created window must have this model as data context");
            }
        }

        /// <summary>
        /// Destroys the window.
        /// </summary>
        protected virtual void DestroyWindow()
        {
            this.Window = null;
        }

        private void WindowOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (this.DisplayOptions == null || sizeChangedEventArgs.PreviousSize.Height > 0
                || sizeChangedEventArgs.PreviousSize.Width > 0)
            {
                return;
            }

            this.DisplayOptions.Screen.MoveWindow((Window)sender);
            this.Window.SizeChanged -= this.WindowOnSizeChanged;
        }
    }
}