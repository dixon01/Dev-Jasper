//--------------------------------------------------------------------------
// <copyright file="BaseDialog.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Design
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Provides a base user interface for dialog forms.
    /// </summary>
    internal partial class BaseDialog : BaseForm
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDialog"/> class.
        /// </summary>
        public BaseDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the submit button has been clicked.
        /// </summary>
        public event EventHandler<CancelEventArgs> Submit;

        /// <summary>
        /// Occurs when the cancel button has been clicked.
        /// </summary>
        public event EventHandler<EventArgs> Cancelling;

        #endregion

        /// <summary>
        /// Attempts to enable the submit button on the form.
        /// </summary>
        protected void EnableSubmitButton()
        {
            this.SubmitButton.Enabled = this.ValidateForm();
        }

        /// <summary>
        /// Validates the form.
        /// </summary>
        /// <returns><b>true</b> if the form is valid; otherwise, <b>false</b>.</returns>
        protected virtual bool ValidateForm()
        {
            return true;
        }

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected virtual void OnSubmit(CancelEventArgs e)
        {
            if (this.Submit != null)
            {
                this.Submit(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Cancelling"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        protected virtual void OnCancelling(EventArgs e)
        {
            if (this.Cancelling != null)
            {
                this.Cancelling(this, e);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="SubmitButton"/> has been clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            CancelEventArgs cancelArgs = new CancelEventArgs(false);
            this.OnSubmit(cancelArgs);

            if (!cancelArgs.Cancel)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Occurs when the <see cref="AbortButton"/> has been clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void AbortButton_Click(object sender, EventArgs e)
        {
            this.OnCancelling(e);

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}