// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileWatcher.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Class to watch for a file specified
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.IntegrationTests
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Class to watch for a file specified
    /// </summary>
    public class FileWatcher : IDisposable
    {
        private FileSystemWatcher watcher;

        private ITimer updateTimeout;

        private int numberOfFiles;

        /// <summary>
        /// Gets a value indicating whether file watching was completed or not.
        /// </summary>
        public bool FileWatchCompleted { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether update successful.
        /// </summary>
        public bool UpdateSuccessful { get; set; }

        /// <summary>
        /// Configures the file system watcher to check for files at a specific path
        /// Starts the file system watcher
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="timeout">
        /// The timeout to watch for the file.
        /// </param>
        /// <param name="numberOfInstalledFeedbackFiles">
        /// The number Of Installed Feedback Files.
        /// </param>
        public void Run(string path, TimeSpan timeout, int numberOfInstalledFeedbackFiles)
        {
            this.numberOfFiles = numberOfInstalledFeedbackFiles;
            this.watcher = new FileSystemWatcher();
            this.watcher.Path = path;
            this.watcher.IncludeSubdirectories = true;
            this.watcher.Filter = "*.*";
            this.watcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.LastWrite
                                        | NotifyFilters.FileName;
            this.watcher.Created += this.OnFileChanged;
            this.watcher.Changed += this.OnFileChanged;

            this.updateTimeout = TimerFactory.Current.CreateTimer("UpdateTimeout");
            this.updateTimeout.Elapsed += this.UpdateTimeoutOnElapsed;
            this.updateTimeout.Interval = timeout;
            this.updateTimeout.AutoReset = false;
            this.updateTimeout.Enabled = true;

            this.StartFileWatcher();
        }

        /// <summary>
        /// The wait for update completion.
        /// </summary>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool WaitForUpdateCompletion(string filePath)
        {
            while (!this.FileWatchCompleted)
            {
            }

            if (!this.UpdateSuccessful)
            {
                throw new Exception("Installed feedback not received");
            }

            if (!File.Exists(filePath))
            {
                throw new Exception(string.Format("File not updated at: {0}", filePath));
            }

            return true;
        }

        /// <summary>
        /// Starts the file system watcher to look for files at specific path
        /// </summary>
        public void StartFileWatcher()
        {
            this.watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stops the file system watcher
        /// </summary>
        public void StopFileWatcher()
        {
            this.watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Disposes the file system watcher
        /// </summary>
        public void Dispose()
        {
            this.StopFileWatcher();
            this.watcher.Dispose();
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            var files = Directory.GetFiles(this.watcher.Path, "*Installed.guf", SearchOption.AllDirectories);

            if (files.Length == this.numberOfFiles)
            {
                Console.WriteLine("File has been written at: {0}", this.watcher.Path);
                this.UpdateSuccessful = true;
                this.FileWatchCompleted = true;
                this.StopFileWatcher();
                this.updateTimeout.Enabled = false;
            }
        }

        private void UpdateTimeoutOnElapsed(object sender, EventArgs e)
        {
            this.FileWatchCompleted = true;
            this.updateTimeout.Enabled = false;
            this.StopFileWatcher();
        }
    }
}
