// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWindowController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Defines a controller for a Window.
    /// </summary>
    public interface IWindowController
    {
        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        event EventHandler WindowClosed;

        /// <summary>
        /// Occurs when the window is being closed.
        /// </summary>
        event CancelEventHandler WindowClosing;

        /// <summary>
        /// Gets the associated window view.
        /// </summary>
        IWindowViewModel Window { get; }

        /// <summary>
        /// Shows the window and returns without waiting for the opened window to close.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}