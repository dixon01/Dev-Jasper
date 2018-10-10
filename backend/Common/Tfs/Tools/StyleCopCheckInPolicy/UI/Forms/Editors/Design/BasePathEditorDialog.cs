//--------------------------------------------------------------------------
// <copyright file="BasePathEditorDialog.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors.Design
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.FileSystem;

    /// <summary>
    /// Provides a base user interface for path editor dialog forms.
    /// </summary>
    internal partial class BasePathEditorDialog : BaseEditorDialog
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePathEditorDialog"/> class.
        /// </summary>
        public BasePathEditorDialog()
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
            return base.ValidateForm() && !string.IsNullOrEmpty(this.ExclusionTypeComboBox.Text);
        }

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected override void OnSubmit(CancelEventArgs e)
        {
            this.Value[PathPolicyExclusion.ExclusionTypeProperty] = this.ExclusionTypeComboBox.Text;

            base.OnSubmit(e);
        }

        /// <summary>
        /// Occurs when the <see cref="BasePathEditorDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void BasePathEditorDialog_Load(object sender, EventArgs e)
        {
            this.LoadExclusionTypesComboBox();

            if (this.EditMode == EditorMode.Edit)
            {
                this.ExclusionTypeComboBox.Text = this.Value[PathPolicyExclusion.ExclusionTypeProperty];
            }
            else if (this.EditMode == EditorMode.Add)
            {
                this.ExclusionTypeComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads the exclusion types combo box.
        /// </summary>
        private void LoadExclusionTypesComboBox()
        {
            this.ExclusionTypeComboBox.Items.Clear();

            FieldInfo[] fields = typeof(PathExclusionType).GetFields(BindingFlags.Public | BindingFlags.Static);
            if (fields != null && fields.Length > 0)
            {
                foreach (FieldInfo field in fields)
                {
                    this.ExclusionTypeComboBox.Items.Add(field.Name);
                }
            }
        }
    }
}