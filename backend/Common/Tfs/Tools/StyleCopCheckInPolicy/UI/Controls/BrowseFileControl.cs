//--------------------------------------------------------------------------
// <copyright file="BrowseFileControl.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls.Design;

    /// <summary>
    /// Provides a user control for selecting a specific file. This class cannot be inherited.
    /// </summary>
    [ToolboxItem(true)]
    internal sealed partial class BrowseFileControl : BaseUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowseFileControl"/> class.
        /// </summary>
        public BrowseFileControl()
        {
            this.InitializeComponent();
        }

        #endregion        

        #region Events

        /// <summary>
        /// Occurs when the text has changed.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler<EventArgs> TextChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.ValueTextBox.Text;
            }

            set
            {
                this.ValueTextBox.Text = value;
            }
        }

        #endregion

        /// <summary>
        /// Selects the file name in the textbox.
        /// </summary>
        public void SelectFileName()
        {
            this.ValueTextBox.SelectAll();
        }

        /// <summary>
        /// Raises the <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private new void OnTextChanged(EventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="BrowseButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Resources.Filter_AllFiles;
                dialog.Multiselect = false;
                dialog.ValidateNames = true;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    FileInfo file = new FileInfo(dialog.FileName);
                    if (file != null)
                    {
                        this.ValueTextBox.Text = file.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="BrowseButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            this.OnTextChanged(e);
        }
    }
}