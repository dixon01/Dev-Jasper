// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.Ftp;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Update.Usb;
    using Gorba.Motion.Update.UsbUpdateManager.Data;
    using Gorba.Motion.Update.UsbUpdateManager.Properties;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The control that contains the unit configuration (unit tree and details view).
    /// </summary>
    public partial class UnitConfigControl : UserControl
    {
        private readonly IProjectManager projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfigControl"/> class.
        /// </summary>
        public UnitConfigControl()
        {
            this.InitializeComponent();

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
                this.projectManager.CurrentProjectChanged += this.ProjectManagerOnCurrentProjectChanged;
            }
            catch (NullReferenceException)
            {
            }
        }

        private void ImportAllFrom(IManualUpdateProvider updateProvider, string name)
        {
            var gotFeedback = false;
            updateProvider.FeedbackReceived += (s, e) =>
                {
                    gotFeedback = true;
                    this.Invoke(
                        new MethodInvoker(
                            () =>
                                {
                                    var progress = new ProgressDialog
                                                       {
                                                           Text = "Import Feedback",
                                                           Label = string.Format(
                                                                       "Importing feedback from {0}...", name),
                                                           Task = new ImportTask(
                                                                      e.ReceivedLogFiles, e.ReceivedUpdateStates),
                                                       };
                                    if (progress.ShowDialog(this) == DialogResult.OK)
                                    {
                                        var msg = string.Format(
                                            "Sucessfully imported {0} log files and {1} states",
                                            e.ReceivedLogFiles.Length,
                                            e.ReceivedUpdateStates.Length);
                                        MessageBox.Show(
                                            this, msg, "Add Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show(
                                            this,
                                            "User cancelled import of feedback",
                                            "Import Feedback",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                                    }
                                }));
                };

            var monitor = new DownloadFeedbackProgress(this);
            ThreadPool.QueueUserWorkItem(
                s =>
                    {
                        updateProvider.CheckForFeedback(monitor);
                        this.BeginInvoke(
                            new MethodInvoker(
                                () =>
                                    {
                                        if (gotFeedback)
                                        {
                                            this.UpdateDetailsControls();
                                            return;
                                        }

                                        MessageBox.Show(
                                            this,
                                            "No feedback found on " + name,
                                            "Import Feedback",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                                    }));
                    });
        }

        private void UpdateDetailsControls()
        {
            if (this.unitTree.SelectedUnitGroup != null)
            {
                this.unitGroupDetailsControl.UnitGroup = this.unitTree.SelectedUnitGroup;
                this.unitGroupDetailsControl.Visible = true;
            }
            else
            {
                this.unitGroupDetailsControl.Visible = false;
            }

            if (this.unitTree.SelectedUnit != null)
            {
                this.unitDetailsControl.Unit = this.unitTree.SelectedUnit;
                this.unitDetailsControl.Visible = true;
            }
            else
            {
                this.unitDetailsControl.Visible = false;
            }
        }

        private void ProjectManagerOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            this.unitTree.Groups = this.projectManager.CurrentProject.UnitGroups;
            this.unitGroupDetailsControl.Visible = false;
            this.unitDetailsControl.Visible = false;
            this.buttonImport.Enabled = !this.projectManager.CurrentProjectReadOnly;
            this.unitTree.ReadOnly = this.projectManager.CurrentProjectReadOnly;
        }

        private void UnitTreeSelectedItemChanged(object sender, EventArgs e)
        {
            this.UpdateDetailsControls();
        }

        private void ButtonImportClick(object sender, EventArgs e)
        {
            this.contextMenuStripImport.Items.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.Removable || !drive.IsReady)
                {
                    continue;
                }

                var item =
                    this.contextMenuStripImport.Items.Add(
                        string.Format("{0} ({1})", drive.VolumeLabel, drive.Name.TrimEnd('\\')));
                item.Tag = drive.RootDirectory;
                item.Image = FileIconManager.CreateFolderIcon(drive.RootDirectory.FullName, true).ToBitmap();
                item.Click += this.ImportItemOnClick;
            }

            if (this.contextMenuStripImport.Items.Count == 0)
            {
                this.contextMenuStripImport.Items.Add("No USB stick found").Enabled = false;
            }

            this.contextMenuStripImport.Items.Add(new ToolStripSeparator());
            foreach (var ftpServer in this.projectManager.CurrentProject.FtpServers)
            {
                var item =
                    this.contextMenuStripImport.Items.Add(
                        string.Format("{0}@{1}", ftpServer.Username, ftpServer.Host), Resources.FtpServer);
                item.Tag = ftpServer;
                item.Click += this.ImportItemOnClick;
            }

            this.contextMenuStripImport.Items.Add("Manage FTP Servers...").Click += this.AddFtpServerOnClick;

            this.contextMenuStripImport.Show(this.buttonImport, 0, this.buttonImport.Height);
        }

        private void AddFtpServerOnClick(object sender, EventArgs e)
        {
            var ftpServersDialog = new FtpServersDialog();
            ftpServersDialog.ShowDialog(this);
        }

        private void ImportItemOnClick(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            if (item == null)
            {
                return;
            }

            var usbRoot = item.Tag as DirectoryInfo;
            if (usbRoot != null)
            {
                this.ImportFromUsb(usbRoot);
                return;
            }

            var ftpConfig = item.Tag as FtpUpdateProviderConfig;
            if (ftpConfig != null)
            {
                this.ImportFromFtp(ftpConfig);
            }
        }

        private void ImportFromFtp(FtpUpdateProviderConfig ftpConfig)
        {
            var msg =
                string.Format(
                    "Do you really want to import all feedback from {0}?\r\n"
                    + "All feedback files will be deleted from the FTP server after the import.",
                    ftpConfig.Host);
            if (MessageBox.Show(
                this,
                msg,
                "Import Feedback",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var updateProvider = new FtpUpdateProvider();
                updateProvider.Configure(
                    ftpConfig,
                    new GuiUpdateContext(this.projectManager.CurrentResourceService));

                this.ImportAllFrom(updateProvider, ftpConfig.Host);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    string.Format("Couldn't import update feedback:\r\n{0}", ex),
                    "Import Feedback",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ImportFromUsb(DirectoryInfo usbRoot)
        {
            var msg =
                string.Format(
                    "Do you really want to import all feedback from {0}?\r\n"
                    + "All feedback files will be deleted from the USB stick after the import.",
                    usbRoot.FullName);
            if (MessageBox.Show(
                this,
                msg,
                "Import Feedback",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var root = usbRoot.CreateSubdirectory("Gorba").CreateSubdirectory("Update");
                var updateProvider = new UsbUpdateProvider();
                updateProvider.Configure(
                    new UsbUpdateProviderConfig { RepositoryBasePath = root.FullName },
                    new GuiUpdateContext(this.projectManager.CurrentResourceService));

                this.ImportAllFrom(updateProvider, root.Root.FullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    string.Format("Couldn't import update feedback:\r\n{0}", ex),
                    "Import Feedback",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private class DownloadFeedbackProgress : IProgressMonitor
        {
            private readonly UnitConfigControl parent;

            private DownloadFeedbackTask task;

            public DownloadFeedbackProgress(UnitConfigControl parent)
            {
                this.parent = parent;
            }

            public bool IsCancelled { get; set; }

            public void Start()
            {
                this.task = new DownloadFeedbackTask(this);
                this.parent.BeginInvoke(new ThreadStart(this.ShowDialog));
            }

            public void Progress(double value, string note)
            {
                if (this.task != null)
                {
                    this.task.Progress(value, note);
                }
            }

            public IPartProgressMonitor CreatePart(double startValue, double endValue)
            {
                return new PartProgressMonitor(this, startValue, endValue);
            }

            public void Complete(string errorMessage, string successMessage)
            {
                if (this.task != null)
                {
                    this.task.Complete(errorMessage, successMessage);
                }
            }

            private void ShowDialog()
            {
                var progressDialog = new ProgressDialog
                {
                    Text = "Import Feedback",
                    Label = "Importing feedback...",
                    Task = this.task
                };
                if (progressDialog.ShowDialog(this.parent) != DialogResult.OK)
                {
                    MessageBox.Show(
                        this.parent,
                        "User cancelled importing feedback",
                        "Import Feedback",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private class DownloadFeedbackTask : ProgressTaskBase
        {
            private readonly DownloadFeedbackProgress updateProgress;

            private readonly AutoResetEvent waitCompleted = new AutoResetEvent(false);

            private string errorMessage;

            public DownloadFeedbackTask(DownloadFeedbackProgress updateProgress)
            {
                this.updateProgress = updateProgress;
            }

            public override void Run()
            {
                this.waitCompleted.WaitOne();
                if (this.errorMessage != null)
                {
                    throw new UpdateException(this.errorMessage);
                }
            }

            public override void Cancel()
            {
                base.Cancel();
                this.updateProgress.IsCancelled = true;
            }

            public void Complete(string message, string successMessage)
            {
                this.errorMessage = message;
                this.waitCompleted.Set();
            }

            public void Progress(double value, string note)
            {
                this.State = note;
                this.Value = value;
            }
        }

        private class ImportTask : ProgressTaskBase
        {
            private readonly IReceivedLogFile[] logFiles;

            private readonly UpdateStateInfo[] updateStates;

            public ImportTask(IReceivedLogFile[] logFiles, UpdateStateInfo[] updateStates)
            {
                this.logFiles = logFiles;
                this.updateStates = updateStates;
            }

            public override void Run()
            {
                var maxProgress = this.logFiles.Length + this.updateStates.Length + 1.0;
                var projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
                var updateProject = projectManager.CurrentProject;

                var progress = 0;

                foreach (var logFile in this.logFiles)
                {
                    if (this.IsCancelled)
                    {
                        return;
                    }

                    this.Value = (++progress) / maxProgress;
                    this.State = string.Format("Log file '{0}' from {1}", logFile.FileName, logFile.UnitName);

                    projectManager.ImportLogFile(logFile);
                }

                foreach (var updateState in this.updateStates)
                {
                    if (this.IsCancelled)
                    {
                        return;
                    }

                    this.Value = (++progress) / maxProgress;
                    this.State = string.Format(
                        "State {0} from {1} for update {2}",
                        updateState.State,
                        updateState.UnitId.UnitName,
                        updateState.UpdateId.UpdateIndex);

                    var unit = this.FindUnit(updateProject, updateState.UnitId);
                    if (unit == null)
                    {
                        continue;
                    }

                    var update = unit.Updates.Find(u => u.Command.UpdateId.Equals(updateState.UpdateId));
                    if (update == null)
                    {
                        continue;
                    }

                    if (update.States.Find(s => s.Equals(updateState)) == null)
                    {
                        update.States.Add(updateState);
                    }
                }

                projectManager.Save();
            }

            private Unit FindUnit(UpdateProject updateProject, UnitId unitId)
            {
                foreach (var unitGroup in updateProject.UnitGroups)
                {
                    foreach (var unit in unitGroup.Units)
                    {
                        if (unit.Name.Equals(unitId.UnitName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return unit;
                        }
                    }
                }

                return null;
            }
        }
    }
}
