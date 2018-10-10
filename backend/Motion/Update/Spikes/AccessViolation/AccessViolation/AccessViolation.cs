// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessViolation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Form1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AccessViolation
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.Win32;

    /// <summary>
    /// Form to create access violation
    /// </summary>
    public partial class AccessViolation : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessViolation"/> class.
        /// </summary>
        public AccessViolation()
        {
            this.InitializeComponent();
        }

        public delegate uint LPTHREAD_START_ROUTINE(uint lpParam);

        public static uint ThreadProc(uint lpParam)
        {
            var ptr = new IntPtr(123);
            Marshal.StructureToPtr(123, ptr, true);
            return 0;
        }

        private void ButtonAccessViolationClick(object sender, EventArgs e)
        {
            uint dwThread;
            LPTHREAD_START_ROUTINE lpThread = ThreadProc;
            uint hThreadHandle = CreateThread(0, 0, lpThread, 0, 0, out dwThread);

            CloseHandle(hThreadHandle);
        }

        private void ButtonSupressMessagesClick(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Windows\", true);
            if (key != null)
            {
                key.SetValue("ErrorMode ", 2, RegistryValueKind.DWord);
                MessageBox.Show(string.Format("Value of error mode: {0}", key.GetValue("ErrorMode")));
            }
        }

        [DllImport("Kernel32.dll")]
        public static extern uint CreateThread(uint lpThreadAttributes, uint dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, uint lpParameter, uint dwCreationFlags, out uint lpThreadId);

        [DllImport("Kernel32.dll")]
        public static extern int CloseHandle(uint hObject);

        [DllImport("Kernel32.dll")]
        public static extern void Sleep(uint dwMilliseconds);
    }
}
