//--------------------------------------------------------------------------
// <copyright file="FileEditorDialog.cs" company="Jeff Winn">
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

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.FileSystem;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors.Design;

    /// <summary>
    /// Provides a user interface for editing file based exclusions. This class cannot be inherited.
    /// </summary>
    internal sealed partial class FileEditorDialog : BasePathEditorDialog
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEditorDialog"/> class.
        /// </summary>
        public FileEditorDialog()
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
            return base.ValidateForm() && !string.IsNullOrEmpty(this.FileBrowser.FileName);
        }

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected override void OnSubmit(CancelEventArgs e)
        {
            this.Value[FilePolicyExclusion.PathProperty] = this.FileBrowser.FileName;

            base.OnSubmit(e);
        }

        /// <summary>
        /// Occurs when the <see cref="FileBrowser"/> text has changed.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void FileBrowser_TextChanged(object sender, EventArgs e)
        {
            this.EnableSubmitButton();
        }

        /// <summary>
        /// Occurs when the <see cref="FileEditorDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void FileEditorDialog_Load(object sender, EventArgs e)
        {
            if (this.EditMode == EditorMode.Edit)
            {
                this.FileBrowser.FileName = this.Value[FilePolicyExclusion.PathProperty];
                this.FileBrowser.SelectFileName();                
            }
        }
    }
}