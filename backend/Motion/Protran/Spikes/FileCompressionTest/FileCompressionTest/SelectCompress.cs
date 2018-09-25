// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectCompress.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectCompress type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileCompressionTest
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Windows.Forms;

    using ICSharpCode.SharpZipLib.Zip;

    using NLog;

    /// <summary>
    /// The select compress.
    /// </summary>
    public partial class SelectCompress : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string directory;

        private byte[] buffer;

        private GZipStream compress;

        private FileStream outFile;

        private int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCompress"/> class.
        /// </summary>
        public SelectCompress()
        {
            this.InitializeComponent();
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            if (this.ZipfolderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.directory = this.ZipfolderBrowserDialog.SelectedPath;
        }

        private void CompressButtonClick(object sender, EventArgs e)
        {
            if (this.textBoxOutputFilename.Text == string.Empty)
            {
                MessageBox.Show("Enter output file path first and then click CompressButton");
                return;
            }

            var zipFileName = string.Format("{0}{1}", this.textBoxOutputFilename.Text, ".zip");
            var zip = new FastZip();
            zip.CreateZip(zipFileName, this.directory, true, string.Empty);
            MessageBox.Show("File(s) zipped!");
        }

        private void ButtonGzCompressClick(object sender, EventArgs e)
        {
            this.buffer = Encoding.ASCII.GetBytes(this.textBoxBuffer.Text);

            try
            {
                this.compress.Write(this.buffer, 0, this.buffer.Length);
                this.length += this.buffer.Length;
                this.textBoxInputBufferSize.Text = this.length.ToString(CultureInfo.InvariantCulture);
                this.textBoxOutputBufferSize.Text = this.outFile.Length.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in compressing file using Gz due to: {0}", ex);
            }
        }

        private void ButtonOpenGzClick(object sender, EventArgs e)
        {
            if (this.buttonOpenGz.Text == "Open Gz Stream")
            {
                MessageBox.Show("Gz Stream opened. Start compression by clicking on Compress button");
                this.outFile = File.Create(this.textBoxGzOutputFile.Text);
                this.compress = new GZipStream(this.outFile, CompressionMode.Compress, false);
                this.buttonOpenGz.Text = "Close Gz Stream";
            }
            else
            {
                this.compress.Close();
                this.length = 0;
                this.buttonOpenGz.Text = "Open Gz Stream";
                MessageBox.Show("Buffers compressed");
            }
        }
    }
}
