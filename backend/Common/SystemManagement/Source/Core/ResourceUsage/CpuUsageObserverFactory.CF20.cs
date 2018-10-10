// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CpuUsageObserverFactory.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CpuUsageObserverFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using OpenNETCF.ToolHelp;

    using FILETIME = System.Int64;

    /// <summary>
    /// Implementation of <see cref="ICpuUsageObserverFactory"/> using a single timer
    /// to observe all processes.
    /// </summary>
    public partial class CpuUsageObserverFactory
    {
        private readonly Dictionary<uint, ThreadStatistics> statistics = new Dictionary<uint, ThreadStatistics>();

        private bool dataReady;

        private uint lastTickCount;

        private uint currentTickCount;

        private long totalTime;

        private double GetUsage(long threadTime)
        {
            var delta = this.currentTickCount - this.lastTickCount;
            if (delta == 0)
            {
                return 0;
            }

            return (double)threadTime / delta;
        }

        private void UpdateTimerOnElapsed(object sender, EventArgs e)
        {
            this.totalTime = 0L;
            this.currentTickCount = NativeMethods.GetTickCount();
            lock (this.statistics)
            {
                var oldPermissions = NativeMethods.SetProcPermissions(0xffffffff);
                try
                {
                    var threads = ThreadEntry.GetThreads();
                    foreach (var thread in threads)
                    {
                        ThreadStatistics stats;
                        if (!this.statistics.TryGetValue(thread.ThreadID, out stats))
                        {
                            stats = new ThreadStatistics(thread.ThreadID, thread.OwnerProcessID);
                            this.statistics.Add(thread.ThreadID, stats);
                        }

                        stats.Update();
                        this.totalTime += stats.ThreadTime;
                    }
                }
                finally
                {
                    NativeMethods.SetProcPermissions(oldPermissions);
                }
            }

            this.UpdateCpuUsages();

            this.lastTickCount = this.currentTickCount;
            this.dataReady = true;
        }

        private static class NativeMethods
        {
            [DllImport("coredll.dll", SetLastError = true)]
            public static extern bool GetThreadTimes(
                uint hThread,
                out FILETIME lpCreationTime,
                out FILETIME lpExitTime,
                out FILETIME lpKernelTime,
                out FILETIME lpUserTime);

            [DllImport("coredll.dll")]
            public static extern uint GetTickCount();

            [DllImport("coredll.dll")]
            public static extern uint SetProcPermissions(uint uPerm);
        }

        private class ThreadStatistics
        {
            private long oldTime;

            public ThreadStatistics(uint threadId, uint ownerProcessId)
            {
                this.ThreadId = threadId;
                this.OwnerProcessId = ownerProcessId;
            }

            public uint ThreadId { get; private set; }

            public uint OwnerProcessId { get; private set; }

            public long ThreadTime { get; private set; }

            public void Update()
            {
                FILETIME creation;
                FILETIME exit;
                FILETIME kernel;
                FILETIME user;
                if (!NativeMethods.GetThreadTimes(this.ThreadId, out creation, out exit, out kernel, out user))
                {
                    return;
                }

                var newTime = GetTicks(kernel) + GetTicks(user);
                this.ThreadTime = newTime - this.oldTime;
                this.oldTime = newTime;
            }

            private static long GetTicks(FILETIME time)
            {
                return time / 10000;
            }
        }

        private class ProcessCpuUsageObserver : CpuUsageObserverBase
        {
            private readonly Process process;

            internal ProcessCpuUsageObserver(Process process, CpuUsageObserverFactory factory)
                : base(factory)
            {
                this.process = process;
            }

            public override bool Update()
            {
                var total = 0L;
                foreach (var stat in this.Factory.statistics.Values)
                {
                    if (stat.OwnerProcessId == this.process.Id)
                    {
                        total += stat.ThreadTime;
                    }
                }

                this.Usage = this.Factory.GetUsage(total);
                return this.Factory.dataReady;
            }
        }

        private class SystemCpuUsageObserver : CpuUsageObserverBase
        {
            public SystemCpuUsageObserver(CpuUsageObserverFactory factory)
                : base(factory)
            {
            }

            public override bool Update()
            {
                this.Usage = this.Factory.GetUsage(this.Factory.totalTime);
                return this.Factory.dataReady;
            }
        }
    }
}
