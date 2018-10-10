//--------------------------------------------------------------------------
// <copyright file="BaseEditorDialog.cs" company="Jeff Winn">
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
    using System.Collections.Specialized;
    using System.ComponentModel;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Design;

    /// <summary>
    /// Provides a base user interface for editor dialog forms.
    /// </summary>
    internal partial class BaseEditorDialog : BaseDialog
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEditorDialog"/> class.
        /// </summary>
        public BaseEditorDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the edit mode.
        /// </summary>
        [Browsable(false)]
        public EditorMode EditMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value being modified.
        /// </summary>
        [Browsable(false)]
        public NameValueCollection Value
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Occurs when the <see cref="BaseEditorDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void BaseEditorDialog_Load(object sender, EventArgs e)
        {
            switch (this.EditMode)
            {
                case EditorMode.Add:
                    this.Text = Resources.Text_AddExclusion;
                    break;

                case EditorMode.Edit:
                    this.Text = Resources.Text_EditExclusion;
                    break;
            }
        }
    }
}