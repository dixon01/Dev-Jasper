// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ShellApp
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Wrapper;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Motion.SystemManager.Core;

    using NLog;

    /// <summary>
    /// The main form that is shown as a shell replacement.
    /// </summary>
    internal sealed partial class MainForm : Form
    {
        private const char ExplorerHotKey = 'E';
        private const char TaskManagerHotKey = 'T';
        private const char CommandLineHotKey = 'C';
        private const char RunWindowHotKey = 'R';
        private const char RebootWindowHotKey = 'Q';
        private const char ExitWindowHotKey = 'X';

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly SystemManagerApplication application;

        private int currentHotKeyId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="application">
        /// The underlying shell application.
        /// </param>
        public MainForm(SystemManagerApplication application)
        {
            this.application = application;
            this.InitializeComponent();
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
            var cursor = Cursors.No;
            cursor.Dispose();
            this.Cursor = cursor;

            this.RegisterHotKeys();
        }

        /// <summary>
        /// Gets the control creation parameters.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE, prevent the form from being activated
                return param;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.SendToBackground();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            e.Cancel = true;
        }

        /// <summary>
        /// The Windows message processor.
        /// </summary>
        /// <param name="m">
        /// The message.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case NativeMethods.WM_HOTKEY:
                    // get the keys.
                    var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    var modifier = (KeyModifier)((int)m.LParam & 0xFFFF);
                    this.HandleHotKey(key, modifier);
                    break;
                case NativeMethods.WM_MOUSEACTIVATE:
                    // prevent the form from being clicked and gaining focus
                    m.Result = (IntPtr)NativeMethods.MA_NOACTIVATEANDEAT;
                    break;
                case NativeMethods.WM_ACTIVATE:
                    // if a message gets through to activate the form somehow
                    if (((int)m.WParam & 0xFFFF) != NativeMethods.WA_INACTIVE)
                    {
                        NativeMethods.SetActiveWindow(m.LParam != IntPtr.Zero ? m.LParam : IntPtr.Zero);
                        this.SendToBackground();
                    }

                    break;
            }
        }

        private void SendToBackground()
        {
            NativeMethods.SetWindowPos(
                this.Handle,
                NativeMethods.HWND_BOTTOM,
                0,
                0,
                0,
                0,
                NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
        }

        private void RegisterHotKeys()
        {
            this.RegisterHotKey(ExplorerHotKey);
            this.RegisterHotKey(TaskManagerHotKey);
            this.RegisterHotKey(CommandLineHotKey);
            this.RegisterHotKey(RunWindowHotKey);
            this.RegisterHotKey(RebootWindowHotKey);
            this.RegisterHotKey(ExitWindowHotKey);
        }

        private void RegisterHotKey(char key)
        {
            if (key < 'A' || key > 'Z')
            {
                throw new ArgumentOutOfRangeException("key", "Hotkey must be between A and Z");
            }

            if (!User32.RegisterHotKey(this.Handle, ++this.currentHotKeyId, KeyModifier.Win, (uint)(Keys)key))
            {
                Logger.Warn("Couldn't register Windows-{0} hotkey", key);
            }
        }

        private void HandleHotKey(Keys key, KeyModifier modifiers)
        {
            if (modifiers != KeyModifier.Win)
            {
                return;
            }

            try
            {
                switch ((char)key)
                {
                    case ExplorerHotKey:
                        Process.Start("explorer.exe");
                        break;
                    case TaskManagerHotKey:
                        Process.Start("taskmgr.exe");
                        break;
                    case CommandLineHotKey:
                        Process.Start("cmd");
                        break;
                    case RunWindowHotKey:
                        Process.Start("rundll32.exe", "shell32.dll,#61");
                        break;
                    case RebootWindowHotKey:
                        this.application.Controller.RequestReboot("User pressed Windows-" + RebootWindowHotKey);
                        break;
                    case ExitWindowHotKey:
                        this.application.Controller.RequestExit("User pressed Windows-" + ExitWindowHotKey);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("Couldn't handle Windows-{0} hotkey", (char)key));
            }
        }

        private void DeregisterHotKeys()
        {
            for (int i = this.currentHotKeyId; i > 0; i--)
            {
                User32.UnregisterHotKey(this.Handle, i);
            }
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            public const int WM_HOTKEY = 0x0312;

            public const int WM_ACTIVATE = 6;
            public const int WA_INACTIVE = 0;

            public const int WM_MOUSEACTIVATE = 0x0021;
            public const int MA_NOACTIVATEANDEAT = 0x0004;

            public const uint SWP_NOSIZE = 0x0001;
            public const uint SWP_NOMOVE = 0x0002;

            public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

            [DllImport("user32.dll")]
            public static extern IntPtr SetActiveWindow(IntPtr handle);

            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(
                IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
            // ReSharper restore InconsistentNaming
        }
    }
}
