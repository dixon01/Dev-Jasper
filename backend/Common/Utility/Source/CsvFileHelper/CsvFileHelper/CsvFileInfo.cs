// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator LTG" file="CsvFileInfo.cs">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;

    using CsvHelper.Configuration;

    using NLog;

    /// <summary>The csv file information.</summary>
    public abstract class CsvFileInfo
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private string fileName;

        private int fileLockCount = 0;

        /// <summary>The created.</summary>
        private bool MutexCreated;

        /// <summary>Initializes a new instance of the <see cref="CsvFileInfo"/> class.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="csvConfiguration">The csv configuration.</param>
        /// <param name="csvMapClassType">The csv map class type.</param>
        protected CsvFileInfo(string fileName, FileStream fileStream, Configuration csvConfiguration, Type csvMapClassType)
        {
            this.FileName = fileName;
            this.FileStream = fileStream;
            this.CsvConfiguration = csvConfiguration;
            this.CsvClassMapType = csvMapClassType;
        }

        /// <summary>Initializes a new instance of the <see cref="CsvFileInfo" /> class.</summary>
        protected CsvFileInfo()
        {
        }

        /// <summary>Gets or sets the csv helper class map type.</summary>
        public Type CsvClassMapType { get; set; }

        /// <summary>Gets or sets the csv helper's configuration.</summary>
        public Configuration CsvConfiguration { get; set; }

        /// <summary>Gets the file mutex use for file resource management.</summary>
        public Mutex FileMutex { get; private set; }

        /// <summary>Gets or sets the file name.</summary>
        public string FileName
        {
            get => this.fileName;
            set
            {
                if (value != this.fileName && value != null)
                {
                    this.fileName = value;
                    this.FileMutex = this.CreateFileMutex(this.fileName, out this.MutexCreated);
                }
            }
        }

        /// <summary>Gets the file stream.</summary>
        public FileStream FileStream { get; set; }

        /// <summary>Gets a value indicating whether the file is locked.</summary>
        public bool IsFileLocked { get; private set; }

        /// <summary>Release the file lock.</summary>
        public void ReleaseFileLock()
        {
#if DEBUG
            var st = new StackTrace();
            var method = st.GetFrame(2).GetMethod();
            Debug.WriteLine("- ReleaseFileLock File:{0} at {1}", this.FileName, method.ToString());
#endif
            if (this.IsFileLocked)
            {
                this.fileLockCount--;
                Logger.Debug($"Lock released, new count: {this.fileLockCount}");
                Monitor.Exit(this.FileMutex);
                this.IsFileLocked = false;
            }
        }

        /// <summary>Wait for a file lock between processes.</summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds to wait for the lock.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WaitForFileLock(int millisecondsTimeout = Timeout.Infinite)
        {
#if DEBUG
            var st = new StackTrace();
            var method = st.GetFrame(1).GetMethod();
            if (method.Name.StartsWith("WaitForFileLock"))
            {
                method = st.GetFrame(3).GetMethod();
            }

            Debug.WriteLine("+ WaitForFileLock File:{0} at {1}", this.FileName, method.ToString());
#endif


#if DEBUG_MUTEX 
            if (this.FileMutex == null)
            {
                Debugger.Launch();
            }
#else
            if (this.FileMutex == null)
            {
                throw new NullReferenceException("WaitForFileLock-FileMutex not yet created!");
            }
#endif
            if (Monitor.TryEnter(this.FileMutex, millisecondsTimeout))
            {
                this.IsFileLocked = true;
                this.fileLockCount++;
                Logger.Debug($"Lock obtained, new lock count: {this.fileLockCount}");
                return true;
            }

            return false;
        }

        private Mutex CreateFileMutex(string sourceFileName, out bool mutexCreated)
        {
            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            var mutexId = string.Format($"Global\\{{{0}}}", Path.GetFileNameWithoutExtension(sourceFileName));

            var mutex = new Mutex(false, mutexId, out mutexCreated, securitySettings);
            return mutex;
        }
    }
}