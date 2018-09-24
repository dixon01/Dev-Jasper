// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.TestApps.WinFormsTest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.ServiceModel;

    using NLog;

    /// <summary>
    /// The main form.
    /// </summary>
    public sealed partial class MainForm : Form
    {
        private readonly int processId;

        private readonly List<CpuUser> cpuUsers = new List<CpuUser>();

        private readonly List<byte[][]> usedMemory = new List<byte[][]>();

        private readonly List<IApplicationStateObserver> stateObservers = new List<IApplicationStateObserver>();

        private Logger logger;

        private IApplicationRegistration registration;

        private List<ApplicationInfo> applicationInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // IMPORTANT: don't put any additional initialization in the constructor,
            // but only in the OnLoad(EventArgs) method
            this.InitializeComponent();

            this.processId = Process.GetCurrentProcess().Id;

            this.Text = string.Format("{0} - PID: {1}", this.Text, this.processId);
            var args = Environment.GetCommandLineArgs();
            this.textBoxCommandLine.Text = string.Join(" ", args, 1, args.Length - 1);

            this.toolStrip1.Visible = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.logger = LogManager.GetCurrentClassLogger();
            this.logger.Debug("Loading main form");

            MessageDispatcher.Instance.Configure(
                new FileConfigurator("medi.config", Environment.MachineName, string.Format("GUI-{0}", this.processId)));

            this.registration = SystemManagerClient.Instance.CreateRegistration("GUI");
            this.registration.Registered += this.RegistrationOnRegistered;
            this.registration.WatchdogKicked += this.RegistrationOnWatchdogKicked;
            this.registration.ExitRequested += this.RegistrationOnExitRequested;
            this.registration.Register();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                this.cpuUsers.Add(new CpuUser());
            }

            this.registration.SetRunning();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnClosed(EventArgs e)
        {
            this.registration.Deregister();
            base.OnClosed(e);
        }

        private void RegistrationOnRegistered(object s, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.RegistrationOnRegistered));
                return;
            }

            this.checkBoxRegistered.Checked = true;
            this.propertyGridInfo.SelectedObject = new ApplicationInfoWrapper(this.registration.Info);
        }

        private void RegistrationOnWatchdogKicked(object s, CancelEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CancelEventHandler(this.RegistrationOnWatchdogKicked), s, e);
                return;
            }

            if (!this.checkBoxWatchdog.Checked)
            {
                e.Cancel = true;
            }
        }

        private void RegistrationOnExitRequested(object sender, EventArgs eventArgs)
        {
            var reg = this.registration;
            if (reg == null)
            {
                return;
            }

            reg.SetExiting();
            Application.Exit();
        }

        private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == this.tabControl1.TabPages.IndexOf(this.tabPage2)
                && this.dataGridView1.DataSource == null)
            {
                this.RequestApplicationInfos();
            }
        }

        private void ButtonEnvironmentExitClick(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }

        private void ButtonEnvironmentExitNormalClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ButtonApplicationExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonProcessKillClick(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void ButtonUnhandledExceptionClick(object sender, EventArgs e)
        {
            var thread = new Thread(s => { throw new InvalidOperationException("Button clicked"); });
            thread.Start();
        }

        private void ButtonOutOfMemoryClick(object sender, EventArgs e)
        {
            var thread = new Thread(this.GenerateOutOfMemory);
            thread.Start();
        }

        private void TrackBarCpuUsageValueChanged(object sender, EventArgs e)
        {
            foreach (var cpuUser in this.cpuUsers)
            {
                cpuUser.ExpectedUsage = this.trackBarCpuUsage.Value * 0.01;
            }
        }

        private void TrackBarRamUsageValueChanged(object sender, EventArgs e)
        {
            while (this.usedMemory.Count > this.trackBarRamUsage.Value * 10)
            {
                this.usedMemory.RemoveAt(this.usedMemory.Count - 1);
            }

            while (this.usedMemory.Count < this.trackBarRamUsage.Value * 10)
            {
                var data = new byte[300][];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new byte[1024];
                }

                this.usedMemory.Add(data);
            }

            GC.Collect();
        }

        private void ToolStripButtonRefreshClick(object sender, EventArgs e)
        {
            this.RequestApplicationInfos();
        }

        private void ToolStripButtonAutoRefreshCheckedChanged(object sender, EventArgs e)
        {
            if (this.toolStripButtonAutoRefresh.Checked)
            {
                this.AddStateObservers();
            }
            else
            {
                this.RemoveStateObservers();
            }
        }

        private void ToolStripButtonRelaunchClick(object sender, EventArgs e)
        {
            var selected = this.GetSelectedApplication();
            if (selected == null)
            {
                return;
            }

            SystemManagerClient.Instance.RelaunchApplication(selected, "GUI button clicked");
        }

        private void ToolStripButtonRebootClick(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    "Do you really want to reboot the system?",
                    "System Reboot",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            SystemManagerClient.Instance.Reboot("GUI button clicked");
        }

        private ApplicationInfo GetSelectedApplication()
        {
            if (this.dataGridView1.SelectedRows.Count != 1)
            {
                return null;
            }

            var appInfo = this.dataGridView1.SelectedRows[0].DataBoundItem as ApplicationInfoWrapper;
            return appInfo == null ? null : appInfo.Info;
        }

        private void ToolStripButtonExitClick(object sender, EventArgs e)
        {
            var selected = this.GetSelectedApplication();
            if (selected == null)
            {
                return;
            }

            SystemManagerClient.Instance.ExitApplication(selected, "GUI button clicked");
        }

        private void DataGridView1SelectionChanged(object sender, EventArgs e)
        {
            this.toolStripButtonRelaunch.Enabled = this.dataGridView1.SelectedRows.Count == 1;
            this.toolStripButtonExit.Enabled = this.dataGridView1.SelectedRows.Count == 1;
        }

        private void RequestApplicationInfos()
        {
            this.dataGridView1.UseWaitCursor = true;
            SystemManagerClient.Instance.BeginGetApplicationInfos(this.GotApplicationInfos, null);
        }

        private void GotApplicationInfos(IAsyncResult ar)
        {
            var infos = new List<ApplicationInfo>(SystemManagerClient.Instance.EndGetApplicationInfos(ar));
            this.Invoke(
                new MethodInvoker(() => this.UpdateApplicationList(infos)));
        }

        private void UpdateApplicationList(List<ApplicationInfo> infos)
        {
            this.applicationInfos = infos;
            this.dataGridView1.UseWaitCursor = false;
            this.RemoveStateObservers();
            this.dataGridView1.DataSource =
                new BindingList<ApplicationInfoWrapper>(infos.ConvertAll(i => new ApplicationInfoWrapper(i)));

            if (this.toolStripButtonAutoRefresh.Checked)
            {
                this.AddStateObservers();
            }
        }

        private void AddStateObservers()
        {
            if (this.applicationInfos == null)
            {
                return;
            }

            foreach (var info in this.applicationInfos)
            {
                var observer = SystemManagerClient.Instance.CreateApplicationStateObserver(info);
                observer.StateChanged += this.ObserverOnStateChanged;
                this.stateObservers.Add(observer);
            }
        }

        private void RemoveStateObservers()
        {
            foreach (var observer in this.stateObservers)
            {
                observer.StateChanged -= this.ObserverOnStateChanged;
                observer.Dispose();
            }

            this.stateObservers.Clear();
        }

        private void ObserverOnStateChanged(object sender, EventArgs eventArgs)
        {
            var observer = sender as IApplicationStateObserver;
            if (observer == null)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.ObserverOnStateChanged), sender, eventArgs);
                return;
            }

            var infos = this.dataGridView1.DataSource as BindingList<ApplicationInfoWrapper>;
            if (infos == null)
            {
                return;
            }

            foreach (var info in infos)
            {
                if (info.Name == observer.ApplicationName)
                {
                    info.SetState(observer.State);
                    return;
                }
            }
        }

        private void ButtonRefreshSystemInfoClick(object sender, EventArgs e)
        {
            this.propertyGridSystemInfo.SelectedObject = null;
            SystemManagerClient.Instance.BeginGetSystemInfo(this.GotSystemInfo, null);
        }

        private void GotSystemInfo(IAsyncResult ar)
        {
            var systemInfo = SystemManagerClient.Instance.EndGetSystemInfo(ar);
            this.Invoke(new MethodInvoker(() => this.UpdateSystemInfo(systemInfo)));
        }

        private void UpdateSystemInfo(SystemInfo systemInfo)
        {
            this.propertyGridSystemInfo.SelectedObject = systemInfo;
        }

        private void GenerateOutOfMemory()
        {
            var list = new List<string>();
            for (int i = 1; i < int.MaxValue; i++)
            {
                list.Add(new string('a', i));
            }
        }

        private class ApplicationInfoWrapper : INotifyPropertyChanged
        {
            // ReSharper disable UnusedMember.Local
            private ApplicationState state;

            public ApplicationInfoWrapper(ApplicationInfo info)
            {
                this.Info = info;
                this.state = info.State;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [Browsable(false)]
            public ApplicationInfo Info { get; private set; }

            public string Name
            {
                get
                {
                    return this.Info.Name;
                }
            }

            public string Version
            {
                get
                {
                    return this.Info.Version;
                }
            }

            public string Path
            {
                get
                {
                    return this.Info.Path;
                }
            }

            public ApplicationState State
            {
                get
                {
                    return this.state;
                }
            }

            public TimeSpan Uptime
            {
                get
                {
                    return this.Info.Uptime;
                }
            }

            public long RamBytes
            {
                get
                {
                    return this.Info.RamBytes;
                }
            }

            public double CpuUsage
            {
                get
                {
                    return this.Info.CpuUsage;
                }
            }

            public ProcessWindowStyle WindowState
            {
                get
                {
                    return this.Info.WindowState;
                }
            }

            public bool HasFocus
            {
                get
                {
                    return this.Info.HasFocus;
                }
            }

            public string LastLaunchReason
            {
                get
                {
                    return this.GetReasonString(this.Info.LastLaunchReason);
                }
            }

            public string LastExitReason
            {
                get
                {
                    return this.GetReasonString(this.Info.LastExitReason);
                }
            }

            public void SetState(ApplicationState value)
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RaisePropertyChanged(new PropertyChangedEventArgs("State"));
            }

            private void RaisePropertyChanged(PropertyChangedEventArgs e)
            {
                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            private string GetReasonString(ApplicationReasonInfo reason)
            {
                return reason == null ? string.Empty : reason.Reason + ": " + reason.Explanation;
            }

            // ReSharper restore UnusedMember.Local
        }
    }
}
