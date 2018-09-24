// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowView.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWindowView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Views
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines a view specific for windows.
    /// </summary>
    public interface IWindowView : IView
    {
        /// <summary>
        /// Occurs when the window has been closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Occurs when the window is being closed.
        /// </summary>
        event CancelEventHandler Closing;

        /// <summary>
        /// Activates this window (bring to front).
        /// </summary>
        /// <returns><c>true</c> if the window is in the front; otherwise, <c>false</c>.</returns>
        bool Activate();

        /// <summary>
        /// Shows this window.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes this window.
        /// </summary>
        void Close();

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>The result of the dialog.</returns>
        bool? ShowDialog();

        /// <summary>
        /// Hides this window.
        /// </summary>
        void Hide();
    }
}
