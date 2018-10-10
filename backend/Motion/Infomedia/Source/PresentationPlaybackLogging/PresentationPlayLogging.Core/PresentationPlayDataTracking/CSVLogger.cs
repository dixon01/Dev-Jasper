namespace Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.Utility.CsvFileHelper;
    using NLog;

    public class CSVLogger<T> : ICsvLogging<T>
        where T : class
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object csvFileLock = new object();

        /// <summary>The on file moved.</summary>
        public event EventHandler<string> OnFileMoved;

        public bool IsStarted { get; set; }

        /// <summary>Gets or sets the csv file logger.</summary>
        protected CsvFileHelper<T> CsvPlayLog { get; set; }

        public void Dispose()
        {
            this.CsvPlayLog?.Dispose();
        }

        /// <summary>
        /// Begin CSV Logging, using the given configuration
        /// </summary>
        /// <param name="logFileName">
        /// The log file name.
        /// </param>
        /// <param name="rolloverLogOutputFolder">
        /// The rollover log output folder.
        /// </param>
        /// <param name="filenameRolloverType">
        /// The filename rollover type.
        /// </param>
        /// <param name="maxFileSize">
        /// The max file size.
        /// </param>
        /// <param name="maxRecords">
        /// The max records.
        /// </param>
        /// <exception cref="ApplicationException">
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void Start(string logFileName, string rolloverLogOutputFolder, FileNameRolloverType filenameRolloverType, long maxFileSize, long maxRecords)
        {
            lock (this.csvFileLock)
            {
                if (this.IsStarted)
                {
                    throw new ApplicationException("Presentation Play Logging already started");
                }

                // create the Logging CSV helper for the file and type
                // Use the path given in the file if present else take the out folder path from the config
                if (string.IsNullOrEmpty(logFileName))
                {
                    throw new ArgumentNullException(nameof(logFileName));
                }

                // If we have an invalid or missing directory for the log file, use the rollover folder instead.
                var fileName = Path.GetFileName(logFileName);
                var directoryName = Path.GetDirectoryName(logFileName);
                var rolloverFolder = !string.IsNullOrEmpty(rolloverLogOutputFolder) ? rolloverLogOutputFolder : Directory.GetCurrentDirectory();

                if (string.IsNullOrEmpty(directoryName))
                {
                    directoryName = rolloverFolder;
                }
                fileName = Path.Combine(directoryName, fileName);

                CreateDirectory(directoryName);
                CreateDirectory(rolloverFolder);

                this.CsvPlayLog = new CsvFileHelper<T>(fileName)
                                      {
                                          RollOverFilePath = rolloverFolder,
                                          FileNameRolloverType = filenameRolloverType,
                                          MaxFileSize = maxFileSize,
                                          MaxRecords = maxRecords
                                      };

                Logger.Info("Created new  File created event handler");
                this.CsvPlayLog.OnFileMoved += this.CsvPlayLogOnFileMoved;

                this.IsStarted = true;
            }
        }

        private static void CreateDirectory(string directoryName)
        {
            // create target folder if missing
            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (Exception exception)
            {
                Logger.Error("Failed to create Presentation Play logging folder {0} {1}", directoryName, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Stops logging to the CSV file
        /// </summary>
        public void Stop()
        {
            lock (this.csvFileLock)
            {
                try
                {
                    var csvFileHelper = this.CsvPlayLog;
                    if (csvFileHelper != null)
                    {
                        csvFileHelper.OnFileMoved -= this.CsvPlayLogOnFileMoved;
                    }
                    this.CsvPlayLog?.Dispose();
                }
                finally
                {
                    this.IsStarted = false;
                }
            }
        }

        /// <summary>
        /// Write all records synchronously
        /// </summary>
        /// <param name="records">
        /// The records.
        /// </param>
        public void WriteAll(ICollection<T> records)
        {
            this.CsvPlayLog?.WriteAll(records);
        }

        /// <summary>
        /// Write the given record asynchronously
        /// </summary>
        /// <param name="record">
        /// The entry.
        /// </param>
        public void WriteAsync(T record)
        {
            this.CsvPlayLog?.WriteAsync(record);
        }

        /// <summary>
        /// Write all entries to the log
        /// </summary>
        /// <param name="records">The records to write to the log</param>
        public void WriteAllAsync(ICollection<T> records)
        {
            this.CsvPlayLog?.WriteAllAsync(records);
        }

        private void CsvPlayLogOnFileMoved(object sender, string e)
        {
            Logger.Info("Fire event New Log file Moved {0}", e);
            this.OnFileMoved?.Invoke(this, e);
        }
    }
}