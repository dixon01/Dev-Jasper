// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowViewModelCloseStrategyBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowViewModelCloseStrategyBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.ComponentModel;

    using NLog;

    /// <summary>
    /// Extends the <see cref="WindowViewModelBase"/> closing the Window (Close method of the WPF framework) in the
    /// <see cref="Close"/> implementation.
    /// If a window is currently shown, any call to <see cref="Show"/> will throw
    /// a <see cref="InvalidOperationException"/>.
    /// </summary>
    public abstract class WindowViewModelCloseStrategyBase : WindowViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool opened;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewModelCloseStrategyBase"/> class.
        /// </summary>
        /// <param name="factory">The view.</param>
        protected WindowViewModelCloseStrategyBase(IWindowFactory factory)
            : base(factory)
        {
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public override void Close()
        {
            if (this.Window == null)
            {
                Logger.Warn("Attempt to close a null Window.");
                return;
            }

            if (!this.opened)
            {
                Logger.Warn("Attempt to close a non-opened window.");
                return;
            }

            this.WindowView.Close();
        }

        /// <summary>
        /// Shows the window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <exception cref="InvalidOperationException">The window is already being shown.</exception>
        public override void Show()
        {
            if (this.opened)
            {
                throw new InvalidOperationException("You must close the window before showing it again.");
            }

            this.CreateWindow();
            this.opened = true;
            this.WindowView.Show();
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        protected override void CreateWindow()
        {
            base.CreateWindow();
            this.WindowView.Closed += this.WindowOnClosed;
            this.WindowView.Closing += this.WindowOnClosing;
            this.RaiseCreated();
        }

        private void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            this.DestroyWindow();
            this.opened = false;
            this.RaiseClosed();
        }

        private void WindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            this.RaiseClosing(cancelEventArgs);
        }
    }
}