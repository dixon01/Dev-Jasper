// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VmCuWatchdogController.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Port80WatchdogController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Watchdog
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The implementation for the Gorba Topbox port 80 watchdog controller.
    /// </summary>
    public partial class VmCuWatchdogController : HardwareWatchdogControllerBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<VmCuWatchdogController>();

        private readonly ITimer watchdogTimer;

        private IntPtr watchdogHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="VmCuWatchdogController"/> class.
        /// </summary>
        internal VmCuWatchdogController()
        {
            this.watchdogTimer = TimerFactory.Current.CreateTimer("Watchdog");
            this.watchdogTimer.Elapsed += this.WatchdogTimerOnElapsed;
            this.watchdogTimer.AutoReset = true;
            this.watchdogTimer.Interval = TimeSpan.FromSeconds(2);
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareWatchdogControllerBase.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            Logger.Info("Initializing IHMI watchdog");
            int slaveAddress = 0x6E; // IHMI Z8 device

            this.watchdogHandle = NativeMethods.CreateFile(
                "I2C1:",
                NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
                0,
                IntPtr.Zero,
                NativeMethods.OPEN_EXISTING,
                NativeMethods.FILE_FLAG_WRITE_THROUGH,
                IntPtr.Zero);
            if (this.watchdogHandle == NativeMethods.INVALID_HANDLE_VALUE)
            {
                this.watchdogHandle = IntPtr.Zero;
                Logger.Error("Couldn't open watchdog: {0}", Marshal.GetLastWin32Error());
                return;
            }

            NativeMethods.DeviceIoControl(
                this.watchdogHandle,
                NativeMethods.IOCTL_I2C_SET_SLAVE_ADDRESS,
                ref slaveAddress,
                Marshal.SizeOf(typeof(int)),
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                IntPtr.Zero);

            this.watchdogTimer.Enabled = true;
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareWatchdogControllerBase.Stop"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.watchdogTimer.Enabled = false;
            if (this.watchdogHandle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(this.watchdogHandle);
                this.watchdogHandle = IntPtr.Zero;
            }
        }

        private void WatchdogTimerOnElapsed(object sender, EventArgs e)
        {
            if (this.watchdogHandle == IntPtr.Zero)
            {
                return;
            }

            int write = 0;
            var buffer = new byte[] { 30, 0 };
            if (NativeMethods.SetFilePointer(this.watchdogHandle, 0x18, IntPtr.Zero, NativeMethods.FILE_BEGIN) == -1)
            {
                Logger.Error("Failed to seek register address!");
                return;
            }

            NativeMethods.WriteFile(this.watchdogHandle, buffer, 1, ref write, IntPtr.Zero);

            Logger.Trace("Watchdog triggered");
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;

            public const uint OPEN_EXISTING = 3;

            public const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;

            public const uint FILE_DEVICE_BUS_EXTENDER = 0x0000002a;
            public const uint METHOD_BUFFERED = 0;
            public const uint FILE_ANY_ACCESS = 0;

            public const uint FILE_BEGIN = 0;

            public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            public static readonly uint IOCTL_I2C_SET_SLAVE_ADDRESS = CTL_CODE(
                FILE_DEVICE_BUS_EXTENDER, 3000, METHOD_BUFFERED, FILE_ANY_ACCESS);

            public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
            {
                return (DeviceType << 16) | (Access << 14) | (Function << 2) | Method;
            }

            [DllImport("coredll.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi,
                CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport("coredll.dll", SetLastError = true)]
            public static extern IntPtr CreateFile(
                string lpFileName,
                uint dwDesiredAccess,
                uint dwShareMode,
                IntPtr lpSecurityAttributes,
                uint dwCreationDisposition,
                uint dwFlagsAndAttributes,
                IntPtr hTemplateFile);

            [DllImport("coredll.dll", SetLastError = true)]
            internal static extern int DeviceIoControl(
                IntPtr hDevice,
                uint dwIoControlCode,
                ref int lpInBuffer,
                int nInBufferSize,
                IntPtr lpOutBuffer,
                int nOutBufferSize,
                IntPtr lpBytesReturned,
                IntPtr lpOverlapped);

            [DllImport("coredll.dll", SetLastError = true)]
            internal static extern int SetFilePointer(
              [In] IntPtr hFile,
              [In] int lDistanceToMove,
              [Out] IntPtr lpDistanceToMoveHigh,
              [In] uint dwMoveMethod);

            [DllImport("coredll.dll", SetLastError = true)]
            internal static extern int WriteFile(
                IntPtr hFile,
                byte[] lpBuffer,
                int nNumberOfBytesToWrite,
                ref int lpNumberOfBytesWritten,
                IntPtr lpOverlapped);

            // ReSharper restore InconsistentNaming
        }
    }
}