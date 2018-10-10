//--------------------------------------------------------------------------
// <copyright file="DisplayViolationsDialog.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Design;

    using StyleCop;

    /// <summary>
    /// Provides a user interface to display policy violations. This class cannot be inherited.
    /// </summary>
    internal partial class DisplayViolationsDialog : BaseForm
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayViolationsDialog"/> class.
        /// </summary>
        public DisplayViolationsDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the violations to display.
        /// </summary>
        public Collection<Violation> Violations
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Builds a new <see cref="ListViewItem"/> for the violation.
        /// </summary>
        /// <param name="violation">The violation to use.</param>
        /// <returns>A new <see cref="ListViewItem"/> object.</returns>
        private static ListViewItem BuildListViewItem(Violation violation)
        {
            ListViewItem item = new ListViewItem();

            item.Text = violation.Rule.CheckId;
            item.SubItems.Add(violation.Message);

            return item;
        }

        /// <summary>
        /// Populates the controls on the form.
        /// </summary>
        private void PopulateControls()
        {
            this.PopulateViolationsListView();
        }

        /// <summary>
        /// Populates the violations list view.
        /// </summary>
        private void PopulateViolationsListView()
        {
            this.ViolationsListView.Items.Clear();

            if (this.Violations != null && this.Violations.Count > 0)
            {
                foreach (Violation violation in this.Violations)
                {
                    this.ViolationsListView.Items.Add(BuildListViewItem(violation));
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="CloseButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Occurs when the <see cref="DisplayViolationsDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void DisplayViolationsDialog_Load(object sender, EventArgs e)
        {
            this.PopulateControls();
        }
    }
}