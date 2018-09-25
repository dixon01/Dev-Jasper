//--------------------------------------------------------------------------
// <copyright file="WorkItemFieldEditorDialog.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.WorkItem;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors.Design;

    /// <summary>
    /// Provides a user interface for editing work item field exclusions. This class cannot be inherited.
    /// </summary>
    internal sealed partial class WorkItemFieldEditorDialog : BaseEditorDialog
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemFieldEditorDialog"/> class.
        /// </summary>
        public WorkItemFieldEditorDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Validates the form.
        /// </summary>
        /// <returns><b>true</b> if the form is valid; otherwise, <b>false</b>.</returns>
        protected override bool ValidateForm()
        {
            return base.ValidateForm() && !string.IsNullOrEmpty(this.FieldNameTextBox.Text) && !string.IsNullOrEmpty(this.FieldValueTextBox.Text);
        }

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected override void OnSubmit(CancelEventArgs e)
        {
            this.Value[WorkItemFieldExclusion.FieldNameProperty] = this.FieldNameTextBox.Text;
            this.Value[WorkItemFieldExclusion.FieldValueProperty] = this.FieldValueTextBox.Text;
        }

        /// <summary>
        /// Occurs when the <see cref="WorkItemFieldEditorDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void WorkItemFieldEditorDialog_Load(object sender, EventArgs e)
        {
            if (this.EditMode == EditorMode.Edit)
            {
                this.FieldNameTextBox.Text = this.Value[WorkItemFieldExclusion.FieldNameProperty];
                this.FieldValueTextBox.Text = this.Value[WorkItemFieldExclusion.FieldValueProperty];

                this.FieldNameTextBox.SelectAll();
            }
        }

        /// <summary>
        /// Occurs when the <see cref="FieldNameTextBox"/> text changes.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void FieldNameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.EnableSubmitButton();
        }

        /// <summary>
        /// Occurs when the <see cref="FieldValueTextBox"/> text changes.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void FieldValueTextBox_TextChanged(object sender, EventArgs e)
        {
            this.EnableSubmitButton();
        }
    }
}