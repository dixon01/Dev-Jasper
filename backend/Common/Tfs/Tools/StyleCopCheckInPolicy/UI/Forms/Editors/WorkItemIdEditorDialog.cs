//--------------------------------------------------------------------------
// <copyright file="WorkItemIdEditorDialog.cs" company="Jeff Winn">
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
    using System.Globalization;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.WorkItem;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors.Design;

    /// <summary>
    /// Provides a user interface for editing work item id exclusions. This class cannot be inherited.
    /// </summary>
    internal sealed partial class WorkItemIdEditorDialog : BaseEditorDialog
    {        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemIdEditorDialog"/> class.
        /// </summary>
        public WorkItemIdEditorDialog()
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
            return base.ValidateForm() && !string.IsNullOrEmpty(this.IdTextBox.Text);
        }

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected override void OnSubmit(CancelEventArgs e)
        {
            int workItemId = int.MinValue;
            if (!int.TryParse(this.IdTextBox.Text, out workItemId))
            {
                MessageBox.Show(this, Resources.Message_InvalidWorkItemId, Resources.Message_PolicyType, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else
            {
                this.Value[WorkItemIdExclusion.WorkItemIdProperty] = workItemId.ToString(CultureInfo.CurrentCulture);
            }

            base.OnSubmit(e);
        }

        /// <summary>
        /// Occurs when the <see cref="IdTextBox"/> text changes.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void IdTextBox_TextChanged(object sender, EventArgs e)
        {
            this.EnableSubmitButton();
        }

        /// <summary>
        /// Occurs when the <see cref="WorkItemIdEditorDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void WorkItemIdEditorDialog_Load(object sender, EventArgs e)
        {
            if (this.EditMode == EditorMode.Edit)
            {
                this.IdTextBox.Text = this.Value[WorkItemIdExclusion.WorkItemIdProperty];
                this.IdTextBox.SelectAll();
            }
        }
    }
}