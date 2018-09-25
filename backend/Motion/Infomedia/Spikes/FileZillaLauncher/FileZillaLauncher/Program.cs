// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileZillaLauncher
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            var app = new Launcher();
            app.Run();

            do
            {
                Console.WriteLine("Press q to exit.");
            }
            while (Console.Read() != 'q');
            app.Dispose();
        }
    }

    public class Launcher
    {
        private Process process;

        private ProcessStartInfo info;

        private Job job;

        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        public Launcher()
        {
            this.info = new ProcessStartInfo("FileZilla server.exe");
            this.info.Arguments = "/compat /start";
            this.info.WorkingDirectory = @"D:\PROGS\Protran\FileZillaServer";
            
        }

        /// <summary>
        /// The register.
        /// </summary>
        public void Run()
        {
            this.process = Process.Start(this.info);
            this.job = new Job();
            this.job.AddProcess(Process.GetProcessById(this.process.Id).Handle);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        internal void Dispose()
        {
            if (this.process != null)
            {
                this.process.Kill();
            }

            if (this.job != null)
            {
                this.job.Dispose();
            }
        }
    }

    /// <summary>
    /// The job.
    /// </summary>
    public class Job : IDisposable
    {
        private IntPtr ptrHandle;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        public Job()
        {
            this.ptrHandle = CreateJobObject(null, null);

            var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION();
            info.LimitFlags = 0x2000;

            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
            extendedInfo.BasicLimitInformation = info;

            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (
                !SetInformationJobObject(
                    this.ptrHandle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
            {
                throw new Exception(
                    String.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
            }
        }

        /// <summary>
        /// The job object info type.
        /// </summary>
        public enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,
            BasicLimitInformation = 2,
            BasicUIRestrictions = 4,
            EndOfJobTimeInformation = 6,
            ExtendedLimitInformation = 9,
            SecurityLimitInformation = 5,
            GroupInformation = 11
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            CloseHandle(this.ptrHandle);
            this.ptrHandle = IntPtr.Zero;
        }

        /// <summary>
        /// The add process.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddProcess(IntPtr handle)
        {
            return AssignProcessToJobObject(this.ptrHandle, handle);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateJobObject(object a, string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetInformationJobObject(
            IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            this.Close();
            this.disposed = true;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public Int64 PerProcessUserTimeLimit;
            public Int64 PerJobUserTimeLimit;
            public Int16 LimitFlags;
            public UInt32 MinimumWorkingSetSize;
            public UInt32 MaximumWorkingSetSize;
            public Int16 ActiveProcessLimit;
            public Int64 Affinity;
            public Int16 PriorityClass;
            public Int16 SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IO_COUNTERS
        {
            public UInt64 ReadOperationCount;
            public UInt64 WriteOperationCount;
            public UInt64 OtherOperationCount;
            public UInt64 ReadTransferCount;
            public UInt64 WriteTransferCount;
            public UInt64 OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UInt32 ProcessMemoryLimit;
            public UInt32 JobMemoryLimit;
            public UInt32 PeakProcessMemoryUsed;
            public UInt32 PeakJobMemoryUsed;
        }
    }
}
