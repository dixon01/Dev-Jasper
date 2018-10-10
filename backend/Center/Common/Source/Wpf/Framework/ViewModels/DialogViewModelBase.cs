// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DialogViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Defines the base ViewModel for dialogs.
    /// </summary>
    public class DialogViewModelBase : DialogParentViewModelBase, IDialogViewModel
    {
        private readonly IDialogFactory dialogFactory;

        private bool opened;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModelBase"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        protected DialogViewModelBase(IDialogFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            this.dialogFactory = factory;
        }

        /// <summary>
        /// The occurs when the dialog is being closed.
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// Gets or sets the dialog result value, which is the value that is returned from the ShowDialog method.
        /// </summary>
        /// <value>
        /// The dialog result.
        /// </value>
        public bool? DialogResult
        {
            get
            {
                return this.Dialog.DialogResult;
            }

            set
            {
                this.Dialog.DialogResult = value;
            }
        }

        /// <summary>
        /// Gets the dialog.
        /// </summary>
        public IDialogView Dialog { get; private set; }

        /// <summary>
        /// Shows the window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="parent">
        /// The parent window.
        /// </param>
        /// <returns>
        /// The result of the dialog.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The window is already being shown.
        /// </exception>
        public virtual bool? ShowDialog(DialogParentViewModelBase parent = null)
        {
            if (this.opened)
            {
                throw new InvalidOperationException("You must close the dialog before showing it again.");
            }

            this.CreateDialog(parent);
            this.opened = true;
            this.ResizeDialog();
            return this.Dialog.ShowDialog();
        }

        /// <summary>
        /// Closes the associated dialog.
        /// </summary>
        public virtual void Close()
        {
            this.Dialog.Close();
        }

        /// <summary>
        /// Creates the dialog.
        /// </summary>
        /// <param name="parent">
        ///     The parent view model.
        /// </param>
        protected virtual void CreateDialog(DialogParentViewModelBase parent)
        {
            this.Dialog = this.dialogFactory.Create(this);
            this.Window = this.Dialog as Window;
            if (this.Window == null)
            {
                throw new ApplicationException("The Dialog must inherit from type Window.");
            }

            if (parent != null)
            {
                this.Dialog.Owner = parent.Window;
            }

            this.Dialog.Closed += this.DialogOnClosed;
            this.Dialog.Closing += this.DialogOnClosing;
        }

        /// <summary>
        /// Destroys the dialog.
        /// </summary>
        protected void DestroyDialog()
        {
            this.Dialog.Closing -= this.DialogOnClosing;
            this.Dialog.Closed -= this.DialogOnClosed;
            this.Dialog = null;
        }

        /// <summary>
        /// Raises the <see cref="Closing"/> event.
        /// </summary>
        protected virtual void RaiseClosing()
        {
            var handler = this.Closing;
            if (handler == null)
            {
                return;
            }

            handler(this, EventArgs.Empty);
        }

        private void DialogOnClosed(object sender, EventArgs eventArgs)
        {
            this.DestroyDialog();
            this.opened = false;
        }

        private void DialogOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            this.RaiseClosing();
        }

        private void ResizeDialog()
        {
            if (this.DisplayOptions == null)
            {
                return;
            }

            var window = this.Dialog as Window;
            if (window == null)
            {
                return;
            }

            this.DisplayOptions.Screen.MoveWindow(window);
        }
    }
}
