// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IM2Converter
{
    using System;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();
        }

        private void BrowseButtonOnClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.filenameTextBox.Text = this.openFileDialog.FileName;
        }

        private void ConvertButtonOnClick(object sender, EventArgs e)
        {
            var oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var converter = new Converter(this.filenameTextBox.Text);
                converter.CreateBackup = this.backupCheckBox.Checked;
                converter.Convert();

                MessageBox.Show(
                    this,
                    "Conversion was sucessful.",
                    "Conversion Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Coudln't convert file: \n" + ex,
                    "Conversion Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            this.Cursor = oldCursor;
        }
    }
}
