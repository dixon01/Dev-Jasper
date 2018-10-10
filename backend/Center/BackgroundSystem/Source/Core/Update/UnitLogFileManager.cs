// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitLogFileManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitLogFileManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The manager that caters about saving unit log files to the database.
    /// </summary>
    public class UnitLogFileManager : IDisposable
    {
        private const int BatchSize = 16384; // 16K

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex LogLineRegex = new Regex(@"^([0-9:. \-]+) ([A-Z]+) ([^[]+) \[([^\]]+)\] (.*)$");

        private readonly IProducerConsumerQueue<LogEntrySet>
            logEntriesProducerConsumerQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitLogFileManager"/> class.
        /// </summary>
        public UnitLogFileManager()
        {
            this.logEntriesProducerConsumerQueue =
                ProducerConsumerQueueFactory<LogEntrySet>.Current.Create(
                    "UnitLogFileManager",
                    SaveLogEntries,
                    100);
            this.logEntriesProducerConsumerQueue.StartConsumer();
        }

        /// <summary>
        /// Asynchronously uploads a log file to the background system database.
        /// </summary>
        /// <param name="filename">
        /// The file name of the log file without path.
        /// </param>
        /// <param name="content">
        /// The content of the log file.
        /// </param>
        /// <param name="unitId">
        /// The unit id.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        public async Task SaveLogFileAsync(string filename, Stream content, int unitId)
        {
            var tempFile = Path.GetTempFileName();
            using (var stream = File.OpenWrite(tempFile))
            {
                await content.CopyToAsync(stream);

                if (stream.Length == 0)
                {
                    Logger.Debug("Received log file '{0}' for unit {1} is empty", filename, unitId);
                    return;
                }
            }

            var unitService = DependencyResolver.Current.Get<IUnitDataService>();
            var unit = await unitService.GetAsync(unitId);

            Logger.Trace(
                "Enqueued file '{0}' to be stored as log entries for unit '{1}' ({2})",
                filename,
                unitId,
                unit.Name);
            this.logEntriesProducerConsumerQueue.Enqueue(new LogEntrySet(unit, filename, tempFile));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.logEntriesProducerConsumerQueue.Dispose();
        }

        private static void SaveLogEntries(LogEntrySet logEntrySet)
        {
            var applicationName = GetApplicationName(logEntrySet.OriginalFileName);
            try
            {
                var entries = new List<LogEntry>(BatchSize);
                var logEntryDataService = DependencyResolver.Current.Get<ILogEntryDataService>();
                var stopwatch = Stopwatch.StartNew();
                var count = 0;
                using (var fileStream = new FileStream(logEntrySet.TempPath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        string line;
                        var additional = new StringBuilder();
                        LogEntry logEntry = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var match = LogLineRegex.Match(line);
                            if (!match.Success)
                            {
                                additional.AppendLine(line);
                                continue;
                            }

                            if (logEntry != null)
                            {
                                logEntry.AdditionalData = additional.ToString();
                                entries.Add(logEntry);
                                if (entries.Count == BatchSize)
                                {
                                    AddToService(logEntryDataService, entries, ref count);
                                }
                            }

                            additional.Clear();
                            logEntry = ParseLogEntry(match, logEntrySet.Unit, applicationName);
                        }

                        if (logEntry != null)
                        {
                            logEntry.AdditionalData = additional.ToString();
                            entries.Add(logEntry);
                        }
                    }
                }

                if (entries.Any())
                {
                    AddToService(logEntryDataService, entries, ref count);
                }

                stopwatch.Stop();
                Logger.Debug(
                    "Stored {0} item(s) from the log file '{1}' in {2} ms",
                    count,
                    logEntrySet.OriginalFileName,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Logger.Warn("Error while storing log entries of file '{0}' Cause {1}", logEntrySet.OriginalFileName, exception.Message);
            }
            finally
            {
                try
                {
                    File.Delete(logEntrySet.TempPath);
                }
                catch (Exception exception)
                {
                    Logger.Debug("Error while deleting the temp file '{0}' Cause {1}", logEntrySet.TempPath, exception.Message);
                }
            }
        }

        /// <summary>
        /// Sends the entries to the service, updates the count and clears the given collection.
        /// </summary>
        /// <param name="logEntryDataService">The service.</param>
        /// <param name="entries">The collection of entries.</param>
        /// <param name="count">The total count of items.</param>
        private static void AddToService(
            ILogEntryDataService logEntryDataService,
            ICollection<LogEntry> entries,
            ref int count)
        {
            logEntryDataService.AddRangeAsync(entries).Wait();
            count += entries.Count;
            entries.Clear();
        }

        private static string GetApplicationName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return string.Empty;
            }

            var applicationName = Path.GetFileNameWithoutExtension(filename);
            var index = applicationName.IndexOfAny(new[] { '_', '-' });
            if (index > 0)
            {
                while (index > 0 && char.IsDigit(applicationName[index]))
                {
                    index--;
                }

                applicationName = applicationName.Substring(0, index);
            }

            return applicationName;
        }

        private static LogEntry ParseLogEntry(
            Match match,
            Unit unit,
            string applicationName)
        {
            if (match == null)
            {
                return null;
            }

            var entry = new LogEntry
                            {
                                Unit = unit,
                                Application = applicationName,
                                Level = Level.Info
                            };

            DateTime timestamp;
            if (DateTime.TryParse(match.Groups[1].Value, out timestamp))
            {
                entry.Timestamp = timestamp;
            }

            Level level;
            if (Enum.TryParse(match.Groups[2].Value, true, out level))
            {
                entry.Level = level;
            }

            entry.Logger = match.Groups[3].Value;
            entry.Message = match.Groups[5].Value;
            entry.AdditionalData = string.Empty;

            return entry;
        }

        private class LogEntrySet
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogEntrySet"/> class.
            /// </summary>
            /// <param name="unit">The unit id.</param>
            /// <param name="originalFileName">
            ///     The original file name as provided by the caller of the service.
            /// </param>
            /// <param name="tempPath">
            ///     The temp path where the file was stored.
            /// </param>
            /// <exception cref="ArgumentNullException">An argument is null.</exception>
            public LogEntrySet(Unit unit, string originalFileName, string tempPath)
            {
                if (unit == null)
                {
                    throw new ArgumentNullException("unit", "The unit can't be null");
                }

                if (originalFileName == null)
                {
                    throw new ArgumentNullException("originalFileName", "The originalFileName can't be null");
                }

                if (tempPath == null)
                {
                    throw new ArgumentNullException("tempPath", "The tempPath can't be null");
                }

                this.Unit = unit;
                this.OriginalFileName = originalFileName;
                this.TempPath = tempPath;
            }

            public string OriginalFileName { get; private set; }

            public string TempPath { get; private set; }

            public Unit Unit { get; private set; }
        }
    }
}
