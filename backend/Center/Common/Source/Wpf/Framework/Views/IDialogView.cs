// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogView.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDialogView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Defines a dialog view.
    /// </summary>
    public interface IDialogView
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
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        object DataContext { get; set; }

        /// <summary>
        /// Gets or sets the dialog owner <see cref="Window"/>.
        /// </summary>
        Window Owner { get; set; }

        /// <summary>
        /// Gets or sets the dialog result value, which is the value that is returned from the ShowDialog method.
        /// </summary>
        /// <value>
        /// The dialog result.
        /// </value>
        bool? DialogResult { get; set; }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>The result of the dialog.</returns>
        bool? ShowDialog();

        /// <summary>
        /// Closes this dialog.
        /// </summary>
        void Close();
    }
}
