// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CpuUsageObserverFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CpuUsageObserverFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Implementation of <see cref="ICpuUsageObserverFactory"/> using a single timer
    /// to observe all processes.
    /// </summary>
    public partial class CpuUsageObserverFactory
    {
        private void UpdateTimerOnElapsed(object sender, EventArgs e)
        {
            this.UpdateCpuUsages();
        }

        private sealed class ProcessCpuUsageObserver : CpuUsageObserverBase
        {
            private readonly Process process;

            private readonly int processorCount;

            private long lastTickCount;

            private TimeSpan lastProcessorTime;

            internal ProcessCpuUsageObserver(Process process, CpuUsageObserverFactory factory)
                : base(factory)
            {
                this.process = process;

                this.processorCount = Environment.ProcessorCount;

                this.Update();
            }

            public override bool Update()
            {
                var notify = false;
                this.process.Refresh();
                var currentProcessorTime = this.process.TotalProcessorTime;
                var currentTicks = TimeProvider.Current.TickCount;

                if (this.lastTickCount > 0)
                {
                    var timeDiff = currentProcessorTime - this.lastProcessorTime;
                    var tickDiff = currentTicks - this.lastTickCount;
                    this.Usage = timeDiff.TotalMilliseconds / this.processorCount / tickDiff;
                    notify = true;
                }

                this.lastTickCount = currentTicks;
                this.lastProcessorTime = currentProcessorTime;
                return notify;
            }
        }

        private sealed class SystemCpuUsageObserver : CpuUsageObserverBase
        {
            private const int SystemPerformanceinformation = 2;
            private const int SystemTimeinformation = 3;
            private const int NoError = 0;

            private long lastIdleTime;

            private long lastSystemTime;

            public SystemCpuUsageObserver(CpuUsageObserverFactory factory)
                : base(factory)
            {
                this.Update();
            }

            public unsafe override bool Update()
            {
                int ret;
                var notify = false;
                var timeInfo = new byte[32]; // SYSTEM_TIME_INFORMATION structure
                var perfInfo = new byte[312]; // SYSTEM_PERFORMANCE_INFORMATION structure

                // get new system time
                fixed (byte* timePtr = timeInfo)
                {
                    ret = NtQuerySystemInformation(SystemTimeinformation, timePtr, timeInfo.Length, IntPtr.Zero);
                }

                if (ret != NoError)
                {
                    throw new NotSupportedException(
                        string.Format("NtQuerySystemInformation({0}) returned {1}", SystemTimeinformation, ret));
                }

                // get new CPU's idle time
                fixed (byte* perfPtr = perfInfo)
                {
                    ret = NtQuerySystemInformation(SystemPerformanceinformation, perfPtr, perfInfo.Length, IntPtr.Zero);
                }

                if (ret != NoError)
                {
                    throw new NotSupportedException(
                        string.Format("NtQuerySystemInformation({0}) returned {1}", SystemPerformanceinformation, ret));
                }

                // CurrentValue = NewValue - OldValue
                var idleTime = BitConverter.ToInt64(perfInfo, 0);
                var systemTime = BitConverter.ToInt64(timeInfo, 8);

                if (this.lastIdleTime >= 0)
                {
                    double deltaIdleTime = idleTime - this.lastIdleTime;
                    double deltaSystemTime = systemTime - this.lastSystemTime;

                    // CurrentCpuIdle = IdleTime / SystemTime
                    var cpuIdle = deltaSystemTime > 0 ? deltaIdleTime / deltaSystemTime : 0;

                    // CurrentCpuUsage% = 100 - (CurrentCpuIdle * 100) / NumberOfProcessors
                    var cpuUsage = 1.0 - (cpuIdle / Environment.ProcessorCount);

                    this.Usage = cpuUsage;

                    notify = true;
                }

                // store new CPU's idle and system time
                this.lastIdleTime = idleTime;
                this.lastSystemTime = systemTime;

                return notify;
            }

            /// <summary>
            /// This is an internal Windows function that retrieves various kinds
            /// of system information.
            /// </summary>
            /// <param name="dwInfoType">
            /// One of the values enumerated in SYSTEM_INFORMATION_CLASS,
            /// indicating the kind of system information to be retrieved.
            /// </param>
            /// <param name="lpStructure">
            /// Points to a buffer where the requested information is to be returned.
            /// The size and structure of this information varies depending on the
            /// value of the SystemInformationClass parameter.
            /// </param>
            /// <param name="dwSize">
            /// Length of the buffer pointed to by the SystemInformation parameter.
            /// </param>
            /// <param name="returnLength">
            /// Optional pointer to a location where the function writes the
            /// actual size of the information requested.
            /// </param>
            /// <returns>
            /// Returns a success NTSTATUS if successful, and an NTSTATUS error code otherwise.
            /// </returns>
            [SuppressMessage(
                "StyleCop.CSharp.NamingRules",
                "SA1305:FieldNamesMustNotUseHungarianNotation",
                Justification = "Native Method")]
            [DllImport("ntdll", EntryPoint = "NtQuerySystemInformation")]
            private static unsafe extern int NtQuerySystemInformation(
                int dwInfoType, byte* lpStructure, int dwSize, IntPtr returnLength);
        }
    }
}
