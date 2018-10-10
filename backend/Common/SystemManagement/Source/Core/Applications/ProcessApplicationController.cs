// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;

    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Core.ResourceUsage;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Application controller that launches a separate process.
    /// </summary>
    public partial class ProcessApplicationController : ApplicationControllerBase
    {
        private static readonly TimeSpan WaitForExitingTimeout = TimeSpan.FromSeconds(10);

        private readonly ProcessConfig config;

        private readonly Process process;

        private readonly ITimer killTimer;

        private readonly ProcessResourcesObserver resourcesObserver;

        private bool processStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessApplicationController"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="manager">
        /// The manager that created this controller.
        /// </param>
        public ProcessApplicationController(ProcessConfig config, ApplicationManager manager)
            : base(config, manager)
        {
            this.config = config;

            this.process = new Process { EnableRaisingEvents = true };
            this.process.Exited += this.ProcessOnExited;

            this.killTimer = TimerFactory.Current.CreateTimer("Kill-" + this.Name);
            this.killTimer.AutoReset = false;
            this.killTimer.Elapsed += this.KillTimerOnElapsed;

            this.resourcesObserver = new ProcessResourcesObserver(this.process, this, config.CpuLimit, config.RamLimit);
        }

        /// <summary>
        /// Gets the process id or -1 if the process wasn't started.
        /// </summary>
        public int ProcessId
        {
            get
            {
                return this.processStarted ? this.process.Id : -1;
            }
        }

        /// <summary>
        /// Gets the file path (i.e. <see cref="ProcessConfig.ExecutablePath"/>).
        /// </summary>
        protected override string FilePath
        {
            get
            {
                return this.config.ExecutablePath;
            }
        }

        /// <summary>
        /// The create application info.
        /// </summary>
        /// <returns>
        /// The <see cref="ApplicationInfo"/>.
        /// </returns>
        public override ApplicationInfo CreateApplicationInfo()
        {
            var info = base.CreateApplicationInfo();
            info.CpuUsage = this.processStarted ? this.resourcesObserver.CpuUsage : 0;
            info.RamBytes = this.processStarted ? this.resourcesObserver.RamBytes : 0;

            try
            {
                info.WindowState = this.GetWindowState();
                if (info.WindowState != ProcessWindowStyle.Hidden)
                {
                    var foregroundWindow = NativeMethods.GetForegroundWindow();
                    info.HasFocus = foregroundWindow != IntPtr.Zero
                        && NativeMethods.GetWindowProcessId(foregroundWindow) == this.ProcessId;
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't get window state");
            }

            return info;
        }

        /// <summary>
        /// Implementation of the launch of the application.
        /// </summary>
        protected override void DoLaunch()
        {
            if (this.State != ApplicationState.AwaitingLaunch && this.State != ApplicationState.Exited)
            {
                this.Logger.Warn("Can't launch in state {0}", this.State);
                return;
            }

            var info = new ProcessStartInfo();
            info.FileName = this.config.ExecutablePath;
            info.Arguments = this.config.Arguments;
            if (!string.IsNullOrEmpty(this.config.WorkingDirectory))
            {
                info.WorkingDirectory = this.config.WorkingDirectory;
            }
            else
            {
                var dir = Path.GetDirectoryName(this.config.ExecutablePath);
                if (dir != null)
                {
                    info.WorkingDirectory = dir;
                }
            }

            this.PrepareProcessInfo(info);

            this.process.StartInfo = info;

            this.Logger.Info("Starting '{0}' {1}", info.FileName, info.Arguments);
            this.process.Start();
            this.processStarted = true;
            try
            {
                this.resourcesObserver.Start();

                if (this.config.Priority != null)
                {
                    this.SetProcessPriority(this.config.Priority.Value);
                }

                this.State = this.config.UseWatchdog ? ApplicationState.Launching : ApplicationState.Running;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Could not change priority class for a process");
            }
        }

        /// <summary>
        /// Implementation of the exit of the application.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        protected override void DoExit(ApplicationReason reason, string explanation)
        {
            if (!this.processStarted)
            {
                this.Logger.Debug("Trying to stop process that was never started");
                return;
            }

            if (this.process.HasExited)
            {
                this.Logger.Warn("Can't exit, was already exited with {0}", this.process.ExitCode);
                return;
            }

            if (this.config.UseWatchdog)
            {
                this.killTimer.Interval = WaitForExitingTimeout;
                this.killTimer.Enabled = true;
            }

            if (!this.config.UseWatchdog || !this.MessageHandler.SendExitCommand())
            {
                // reset the timer immediately to use the config's ExitTimeout
                this.killTimer.Enabled = false;
                this.killTimer.Interval = this.config.ExitTimeout;
                this.killTimer.Enabled = true;
                this.process.CloseMainWindow();
            }
        }

        /// <summary>
        /// Raises the <see cref="ApplicationControllerBase.StateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event argument.
        /// </param>
        protected override void RaiseStateChanged(EventArgs e)
        {
            if (this.State == ApplicationState.Exiting && this.killTimer.Enabled)
            {
                this.killTimer.Enabled = false;
                this.killTimer.Interval = this.config.ExitTimeout;
                this.killTimer.Enabled = true;
            }

            base.RaiseStateChanged(e);
        }

        partial void PrepareProcessInfo(ProcessStartInfo info);

        partial void SetProcessPriority(ProcessPriorityClass value);

        private ProcessWindowStyle GetWindowState()
        {
            if (!this.processStarted || this.process.MainWindowHandle == IntPtr.Zero)
            {
                return ProcessWindowStyle.Hidden;
            }

            var placement = NativeMethods.GetPlacement(this.process.MainWindowHandle);
            switch (placement.showCmd)
            {
                case NativeMethods.ShowWindowCommands.Hide:
                    return ProcessWindowStyle.Hidden;
                case NativeMethods.ShowWindowCommands.Normal:
                    return ProcessWindowStyle.Normal;
                case NativeMethods.ShowWindowCommands.Minimized:
                    return ProcessWindowStyle.Minimized;
                case NativeMethods.ShowWindowCommands.Maximized:
                    return ProcessWindowStyle.Maximized;
                default:
                    return ProcessWindowStyle.Normal;
            }
        }

        private void ProcessOnExited(object sender, EventArgs e)
        {
            this.resourcesObserver.Stop();

            this.killTimer.Enabled = false;
            this.processStarted = false;
            this.Logger.Info("{0} has exited with {1}", this.config.ExecutablePath, this.process.ExitCode);
            this.SetState(
                ApplicationState.Exited,
                ApplicationReason.ApplicationExit,
                string.Format("with exit code {0}", this.process.ExitCode));
        }

        private void KillTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                this.Logger.Debug("Killing {0}", this.config.ExecutablePath);
                this.process.Kill();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't kill process");
            }
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // ReSharper disable MemberCanBePrivate.Local
            public enum ShowWindowCommands
            {
                /// <summary>
                /// Hide the window.
                /// </summary>
                Hide = 0,

                /// <summary>
                /// Set the window to normal style.
                /// </summary>
                Normal = 1,

                /// <summary>
                /// Set the window to minimized.
                /// </summary>
                Minimized = 2,

                /// <summary>
                /// Set the window to maximized.
                /// </summary>
                Maximized = 3,
            }

            public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
            {
                var placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                GetWindowPlacement(hwnd, ref placement);
                return placement;
            }

            public static int GetWindowProcessId(IntPtr hwnd)
            {
                int processId = 0;
                GetWindowThreadProcessId(hwnd, ref processId);
                return processId;
            }

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            private static extern int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public ShowWindowCommands showCmd;
                public Point ptMinPosition;
                public Point ptMaxPosition;
                public Rectangle rcNormalPosition;
            }

            // ReSharper restore InconsistentNaming
            // ReSharper restore FieldCanBeMadeReadOnly.Local
            // ReSharper restore MemberCanBePrivate.Local
        }
    }
}