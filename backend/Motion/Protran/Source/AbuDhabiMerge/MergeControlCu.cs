// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeControlCu.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MergeControlCu : MergeControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeControlCu"/> class.
        /// </summary>
        public MergeControlCu()
        {
            this.labelSelectBrowse.Text = "Select Files to Merge:";
            this.labelCount.Text = "Number of files selected:";
            this.labelClear.Text = "Clear all selected files:";
            this.FilesInFolderTreeView.Visible = false;

            this.SelectedFilesCheckedListBox.Size = new Size(494, 484);
        }

        /// <summary>
        /// Configures this control
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public override void Configure(AbuDhabiMergeConfig config)
        {
            base.Configure(config);

            if (this.Config.Extensions.CuData == string.Empty)
            {
                this.radioButtonData.Enabled = false;
                this.radioButtonSoftware.Checked = true;
            }

            if (this.Config.Extensions.CuSoftware == string.Empty)
            {
                this.radioButtonSoftware.Enabled = false;
                this.radioButtonData.Checked = true;
            }
        }

        /// <summary>
        /// Browse Button Click
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void BrowseButtonClick(object sender, EventArgs e)
        {
            if (this.SelectFilesOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.CollectFilesToMerge();
        }

        /// <summary>
        /// Collect Files To Merge
        /// </summary>
        protected override void CollectFilesToMerge()
        {
            foreach (var filename in this.SelectFilesOpenFileDialog.FileNames)
            {
                if (!this.SelectedFilesCheckedListBox.Items.Contains(filename))
                {
                    this.SelectedFilesCheckedListBox.DisplayMember = filename;
                    this.SelectedFilesCheckedListBox.Items.Add(filename);
                    this.SelectedFilesCheckedListBox.SetSelected(this.Index, true);
                    this.SelectedFilesCheckedListBox.SetItemChecked(this.Index, true);
                    this.Index++;
                }
                else
                {
                    MessageBox.Show(
                        this, "File already added.", "AbuDhabiMerge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Process Files For Merge
        /// </summary>
        protected override void ProcessFilesForMerge()
        {
            this.ClearMergeStatus();

            if (this.ListToMerge.Count <= 0)
            {
                this.AppendLog("No files selected for merge!");
                return;
            }

            var mergeThread = new Thread(this.MergeFiles);
            mergeThread.IsBackground = true;
            mergeThread.Start();
        }

        /// <summary>
        /// Get Extension
        /// </summary>
        /// <returns>
        /// the extension
        /// </returns>
        protected override string GetExtension()
        {
            var extension = string.Empty;

            if (this.radioButtonData.Checked)
            {
                extension = string.Format("Cu5 Data(*{0})|*{0}", this.Config.Extensions.CuData);
            }
            else if (this.radioButtonSoftware.Checked)
            {
                extension = string.Format("Cu5 Software(*{0})|*{0}", this.Config.Extensions.CuSoftware);
            }

            return extension;
        }

        /// <summary>
        /// Merge files
        /// </summary>
        protected override void MergeFiles()
        {
            var origCursor = this.Cursor;
            this.SetCursor(Cursors.WaitCursor);

            try
            {
                var localPath = Path.GetTempPath();
                var localDir = Path.Combine(localPath, LocalTempFolder);

                this.ClearDirectory(localDir);

                // Create a local directory to move the files to be zipped
                if (!Directory.Exists(localDir))
                {
                    Directory.CreateDirectory(localDir);
                }

                if (!this.VerifyCorrectFilesSelected(this.ListToMerge))
                {
                    return;
                }

                // Move all the files in the selected list to the new directory
                foreach (var info in this.ListToMerge.Select(file => new FileInfo(file)))
                {
                    info.CopyTo(Path.Combine(localDir, info.Name));
                }

                // Zip all the files in the local directory and store the zipped file at the zipPath specified
                this.TotalFileCount = this.ListToMerge.Count;
                this.CurrentFileCount = 0;

                // Show the final tree view of destination folder
                this.ShowFinalFolderStructure(localDir);

                var events = new FastZipEvents { ProcessFile = this.ProcessingFile };

                var zip = new FastZip(events);
                zip.CreateZip(this.MergedFileName, localDir, true, string.Empty);

                this.ClearDirectory(localDir);

                this.AppendLog("Selected files have been merged.");
                this.AppendLog("Merged file is located at:");
                this.AppendLog(this.MergedFileName);
            }
            catch (Exception ex)
            {
                this.AppendLog("Merge for selected files failed!");
                this.AppendLog(ex.Message);
            }
            finally
            {
                this.SetCursor(origCursor);
            }
        }

        private bool VerifyCorrectFilesSelected(List<string> list)
        {
            foreach (var info in list.Select(file => new FileInfo(file)))
            {
                if (info.Extension == ".exe" || info.Extension == ".dll" || info.Extension == ".xml" || info.Extension == ".config")
                {
                    this.AppendLog("File {0} has invalid extension. Merge failed!", info.Name);
                    return false;
                }
            }

            return true;
        }

        private void ShowFinalFolderStructure(string localDir)
        {
            if (this.InvokeRequired)
            {
                var d = new ShowFinalFolderStructureCallBack(this.ShowFinalFolderStructure);
                this.Invoke(d, new object[] { localDir });
                return;
            }

            this.MergedFileTreeView.BeginUpdate();
            var directory = new DirectoryInfo(localDir);
            this.MergedFileTreeView.Nodes.Add(new TreeNode(directory.Name));

            var files = directory.GetFiles();
            foreach (var file in files)
            {
                if (file.IsReadOnly)
                {
                    file.IsReadOnly = false;
                }

                this.MergedFileTreeView.Nodes[0].Nodes.Add(file.Name);
            }

            this.MergedFileTreeView.EndUpdate();
        }
    }
}
