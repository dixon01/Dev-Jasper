// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Ntd;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        private NtdFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
        }

        private void OpenBitmap(IntPtr address)
        {
            var bitmap = this.file.GetBitmap(address);
            this.ShowBitmapForm(bitmap, string.Format("Bitmap @ 0x{0:X6}", address.ToInt32()));
        }

        private void OpenFont(int fontIndex)
        {
            var font = this.file.GetFont(fontIndex);
            if (font == null)
            {
                throw new FileNotFoundException("Couldn't find font " + fontIndex);
            }

            this.ShowBitmapForm(new FontExampleBitmap(font), string.Format("Font {0}: {1}", fontIndex, font.Name));
        }

        private void ShowBitmapForm(IBitmap bitmap, string name)
        {
            var bitmapForm = new BitmapForm();
            bitmapForm.Text = name;
            bitmapForm.Bitmap = bitmap;
            bitmapForm.Show();
            bitmapForm.ResizeToFit();
            bitmapForm.BringToFront();
            bitmapForm.Focus();
        }

        private void FindText()
        {
            if (this.textBoxFind.TextLength == 0)
            {
                return;
            }

            var index = this.textBoxOutput.Find(
                this.textBoxFind.Text,
                this.textBoxOutput.SelectionStart + this.textBoxOutput.SelectionLength,
                RichTextBoxFinds.None);
            if (index < 0)
            {
                index = this.textBoxOutput.Find(this.textBoxFind.Text, RichTextBoxFinds.None);
            }

            if (index < 0)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            this.textBoxOutput.Select(index, this.textBoxFind.TextLength);
        }

        private void ShowError(Exception exception)
        {
            MessageBox.Show(
                this, exception.Message, exception.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ButtonBrowseClick(object sender, System.EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var originalCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            this.textBoxOutput.Clear();
            this.file = null;
            try
            {
                var filename = this.openFileDialog.FileName;
                this.textBoxFilename.Text = filename;
                this.buttonSaveAs.Enabled = false;
                this.file = new NtdFile(filename);
                var provider = new NtdInfoProvider(this.file);
                using (var writer = new RichTextBoxExWriter(this.textBoxOutput))
                {
                    provider.WriteTo(writer);
                    writer.Flush();
                }

                this.buttonSaveAs.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }

            this.Cursor = originalCursor;
        }

        private void ButtonSaveAsClick(object sender, System.EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using (
                var writer = new ExtendedTextWriter(File.Create(this.saveFileDialog.FileName), new UTF8Encoding(false)))
            {
                var provider = new NtdInfoProvider(this.file);
                provider.WriteTo(writer);
                writer.Flush();
            }
        }

        private void TextBoxOutputLinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                var parts = e.LinkText.Split('#', ':');
                if (parts.Length != 3)
                {
                    throw new ArgumentException("Link unknown");
                }

                var type = parts[1];
                int value;
                if (!int.TryParse(parts[2], out value))
                {
                    throw new ArgumentException("Bad link data");
                }

                switch (type)
                {
                    case "fnt":
                        this.OpenFont(value);
                        break;
                    case "bmp":
                        this.OpenBitmap(new IntPtr(value));
                        break;
                    default:
                        throw new ArgumentException("Unknown link type");
                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        private void ButtonFindClick(object sender, EventArgs e)
        {
            this.FindText();
        }

        private void TextBoxFindPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                this.FindText();
            }
        }
    }
}
