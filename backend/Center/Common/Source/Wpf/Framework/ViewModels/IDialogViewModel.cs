// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDialogViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Defines a ViewModel specific for dialogs.
    /// </summary>
    public interface IDialogViewModel : IDisplayViewModel
    {
        /// <summary>
        /// The occurs when the dialog is being closed.
        /// </summary>
        event EventHandler Closing;

        /// <summary>
        /// Gets or sets the dialog result value, which is the value that is returned from the ShowDialog method.
        /// </summary>
        /// <value>
        /// The dialog result.
        /// </value>
        bool? DialogResult { get; set; }

        /// <summary>
        /// Gets the dialog.
        /// </summary>
        IDialogView Dialog { get; }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="parent">
        /// The parent view model.
        /// </param>
        /// <returns>
        /// A nullable value of type Boolean that specifies whether the activity was accepted (true)
        /// or canceled (false). The return value is the value of the DialogResult property before a window closes.
        /// </returns>
        bool? ShowDialog(DialogParentViewModelBase parent = null);

        /// <summary>
        /// Closes the associated dialog.
        /// </summary>
        void Close();
    }
}