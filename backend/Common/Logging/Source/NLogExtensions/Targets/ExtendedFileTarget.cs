// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedFileTarget.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtendedFileTarget type.
//
//   Partially based on FileTarget and related classes from NLog:
// =============================================================================
// Copyright (c) 2004-2011 Jaroslaw Kowalski [jaak@jkowalski.net]
//
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Jaroslaw Kowalski nor the names of its
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// =============================================================================
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Logging.NLogExtensions.Targets
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Xml;

    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;

    /// <summary>
    /// Extended file target that meets the needs of Gorba.
    /// This target allows to archive every 'x'.
    /// </summary>
    [Target("FileEx")]
    public class ExtendedFileTarget : TargetWithLayoutHeaderAndFooter
    {
        private const int MaxPathCache = 50;

        private readonly object writeLock = new object();

        private readonly Dictionary<string, LogFile> fullPaths =
            new Dictionary<string, LogFile>(StringComparer.InvariantCultureIgnoreCase);

        private readonly string basePath;

        private LineEndingMode lineEndingMode;
        private string newLineChars;

        private TimeSpan maximumCacheTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedFileTarget"/> class.
        /// </summary>
        public ExtendedFileTarget()
        {
            this.basePath = Environment.CurrentDirectory;

            this.maximumCacheTime = TimeSpan.FromMinutes(1);

            this.LineEnding = LineEndingMode.Default;
            this.ArchiveEvery = FileArchivePeriod.None;
            this.Encoding = Encoding.Default;
            this.ArchiveCompression = CompressionType.None;
        }

        /// <summary>
        /// Gets or sets the name of the file to write to.
        /// </summary>
        /// <remarks>
        /// This FileName string is a layout which may include instances of layout renderers.
        /// This lets you use a single target to write to multiple files.
        /// </remarks>
        /// <example>
        /// The following value makes NLog write logging events to files based on the log level in the directory where
        /// the application runs.
        /// <code>${basedir}/${level}.log</code>
        /// All <c>Debug</c> messages will go to <c>Debug.log</c>,
        /// all <c>Info</c> messages will go to <c>Info.log</c> and so on.
        /// You can combine as many of the layout renderers as you want to produce an arbitrary log file name.
        /// </example>
        [RequiredParameter]
        public Layout FileName { get; set; }

        /// <summary>
        /// Gets or sets the line ending mode.
        /// </summary>
        [Advanced]
        public LineEndingMode LineEnding
        {
            get
            {
                return this.lineEndingMode;
            }

            set
            {
                this.lineEndingMode = value;
                switch (value)
                {
                    case LineEndingMode.Default:
                        this.newLineChars = Environment.NewLine;
                        return;
                    case LineEndingMode.CRLF:
                        this.newLineChars = "\r\n";
                        return;
                    case LineEndingMode.CR:
                        this.newLineChars = "\r";
                        return;
                    case LineEndingMode.LF:
                        this.newLineChars = "\n";
                        return;
                    case LineEndingMode.None:
                        this.newLineChars = string.Empty;
                        return;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Gets or sets the file encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically archive log
        /// files every time the specified time passes.
        /// </summary>
        /// <remarks>
        /// Files are moved to the archive as part of the write operation if the
        /// current period of time changes. For example if the current <c>hour</c>
        /// changes from 10 to 11, the first write that will occur
        /// on or after 11:00 will trigger the archiving.
        /// <p>
        /// Caution: Enabling this option can considerably slow down your file
        /// logging in multi-process scenarios. If only one process is going to
        /// be writing to the file, consider setting <c>ConcurrentWrites</c>
        /// to <c>false</c> for maximum performance.
        /// </p>
        /// </remarks>
        public FileArchivePeriod ArchiveEvery { get; set; }

        /// <summary>
        /// Gets or sets the name of the file to be used for an archive.
        /// </summary>
        /// <remarks>
        /// Contrary to <see cref="FileTarget"/>, this file name should not
        /// contain the {#####} placeholder.
        /// </remarks>
        public Layout ArchiveFileName { get; set; }

        /// <summary>
        /// Gets or sets the compression method to be used when archiving log files.
        /// By default this is <see cref="CompressionType.None"/>.
        /// </summary>
        public CompressionType ArchiveCompression { get; set; }

        /// <summary>
        /// Gets or sets the cache size kilobytes.
        /// </summary>
        /// <remarks>
        /// If the size is set to 0, caching is disabled.
        /// Otherwise for each log file, there will be a cache in memory of the given size
        /// and all writes to the log file will be cached until the cache is full or
        /// the <see cref="MaximumCacheTime"/> has been reached.
        /// </remarks>
        [DefaultValue(0)]
        public int CacheSizeKb { get; set; }

        /// <summary>
        /// Gets or sets the maximum cache time as an XML duration.
        /// </summary>
        /// <remarks>
        /// If the <see cref="CacheSizeKb"/> is set to 0, this property is ignored.
        /// Otherwise for each log file, there will be a cache in memory of the given size
        /// and all writes to the log file will be cached until the cache is full or
        /// the given time has been reached.
        /// </remarks>
        [DefaultValue("PT1M")]
        public string MaximumCacheTime
        {
            get
            {
                return XmlConvert.ToString(this.maximumCacheTime);
            }

            set
            {
                this.maximumCacheTime = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Writes logging event to the log target.
        /// </summary>
        /// <param name="logEvent">
        /// Logging event to be written out.
        /// </param>
        protected override void Write(LogEventInfo logEvent)
        {
            var logFile = this.GetLogFile(logEvent);
            lock (this.writeLock)
            {
                this.WriteAndArchive(logFile, logEvent);
            }
        }

        /// <summary>
        /// Writes an array of logging events to the log target. By default it iterates on all
        /// events and passes them to "Write" method. Inheriting classes can use this method to
        /// optimize batch writes.
        /// </summary>
        /// <param name="logEvents">
        /// Logging events to be written out.
        /// </param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            if (logEvents == null || logEvents.Length == 0)
            {
                return;
            }

            if (logEvents.Length == 1)
            {
                // this seems to be rather normal, so we handle this more efficiently
                this.Write(logEvents[0]);
                return;
            }

            var mapping = new Dictionary<LogFile, List<AsyncLogEventInfo>>(2);
            foreach (var logEvent in logEvents)
            {
                var logFile = this.GetLogFile(logEvent.LogEvent);

                List<AsyncLogEventInfo> events;
                if (!mapping.TryGetValue(logFile, out events))
                {
                    events = new List<AsyncLogEventInfo>(logEvents.Length);
                    mapping.Add(logFile, events);
                }

                events.Add(logEvent);
            }

            lock (this.writeLock)
            {
                this.Write(mapping);
            }
        }

        /// <summary>
        /// Flush any pending log messages asynchronously (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">
        /// The asynchronous continuation.
        /// </param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            base.FlushAsync(asyncContinuation);

            lock (this.writeLock)
            {
                foreach (var file in this.fullPaths.Values)
                {
                    file.FlushCache();
                }
            }
        }

        private void Write(Dictionary<LogFile, List<AsyncLogEventInfo>> logEvents)
        {
            foreach (var pair in logEvents)
            {
                var logFile = pair.Key;
                var events = pair.Value;
                Exception exception = null;
                try
                {
                    var last = events[events.Count - 1];
                    if (!this.ShouldAutoArchive(logFile, last.LogEvent))
                    {
                        // nothing to archive, let's just add the entire batch
                        this.WriteToFile(logFile, events.ConvertAll(a => a.LogEvent));
                    }
                    else
                    {
                        // if the last event has to be archived, we manually add one after the other
                        // this will then archive properly at the right moment
                        foreach (var async in events)
                        {
                            this.WriteAndArchive(logFile, async.LogEvent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                foreach (var async in events)
                {
                    async.Continuation(exception);
                }
            }
        }

        private void WriteAndArchive(LogFile logFile, LogEventInfo logEvent)
        {
            if (this.ShouldAutoArchive(logFile, logEvent))
            {
                this.DoAutoArchive(logFile, logEvent);
            }

            this.WriteToFile(logFile, new[] { logEvent });
        }

        private void WriteToFile(LogFile logFile, IEnumerable<LogEventInfo> logEvents)
        {
            if (this.CacheSizeKb > 0)
            {
                this.WriteToCache(logFile, logEvents);
                return;
            }

            using (var writer = logFile.AppendText(this.Encoding))
            {
                if (this.Header != null && logFile.RequiresHeader())
                {
                    writer.Write(this.Header.Render(LogEventInfo.CreateNullEvent()) + this.newLineChars);
                }

                foreach (var logEvent in logEvents)
                {
                    writer.Write(this.Layout.Render(logEvent) + this.newLineChars);
                    logFile.LastWriteTimeStamp = logEvent.TimeStamp;
                }
            }
        }

        private void WriteToCache(LogFile logFile, IEnumerable<LogEventInfo> logEvents)
        {
            if (this.Header != null && logFile.RequiresHeader())
            {
                var bytes = this.RenderBytes(this.Header, LogEventInfo.CreateNullEvent());
                logFile.WriteBytesToCache(bytes, 0, bytes.Length);
            }

            foreach (var logEvent in logEvents)
            {
                var bytes = this.RenderBytes(this.Layout, logEvent);
                logFile.WriteBytesToCache(bytes, 0, bytes.Length);
                logFile.LastWriteTimeStamp = logEvent.TimeStamp;
            }
        }

        private byte[] RenderBytes(Layout layout, LogEventInfo logEvent)
        {
            return this.Encoding.GetBytes(layout.Render(logEvent) + this.newLineChars);
        }

        private LogFile GetLogFile(LogEventInfo logEvent)
        {
            var fileName = this.FileName.Render(logEvent);
            LogFile logFile;
            if (this.fullPaths.TryGetValue(fileName, out logFile))
            {
                return logFile;
            }

            if (this.fullPaths.Count > MaxPathCache)
            {
                foreach (var file in this.fullPaths.Values)
                {
                    file.FlushCache();
                    file.Dispose();
                }

                this.fullPaths.Clear();
            }

            var fullPath = this.CreateFullPath(fileName);
            logFile = new LogFile(fullPath, this.CacheSizeKb * 1024, this.maximumCacheTime);

            this.fullPaths[fileName] = logFile;
            return logFile;
        }

        private string CreateFullPath(string fileName)
        {
            var file = fileName.Substring(fileName.LastIndexOfAny(new[] { '\\', '/' }) + 1);
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                file = file.Replace(c, '-');
            }

            var parent = Path.GetDirectoryName(fileName);
            var fullPath = parent == null ? file : Path.Combine(parent, file);

            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.Combine(this.basePath, fullPath);
            }

            fullPath = Path.GetFullPath(fullPath);
            if (parent != null)
            {
                Directory.CreateDirectory(parent);
            }

            return fullPath;
        }

        private void DoAutoArchive(LogFile logFile, LogEventInfo logEvent)
        {
            var archiveFile = this.GetArchiveFileName(logFile, logEvent);
            archiveFile = this.CreateFullPath(archiveFile);
            if (File.Exists(archiveFile))
            {
                var baseName = Path.ChangeExtension(archiveFile, string.Empty);
                var extension = Path.GetExtension(archiveFile);
                var index = 1;
                do
                {
                    archiveFile = baseName + index + extension;
                    index++;
                }
                while (File.Exists(archiveFile));
            }

            if (this.Footer != null)
            {
                if (this.CacheSizeKb > 0)
                {
                    var footerBytes = this.RenderBytes(this.Footer, LogEventInfo.CreateNullEvent());
                    logFile.WriteBytesToCache(footerBytes, 0, footerBytes.Length);
                }
                else
                {
                    using (var writer = logFile.AppendText(this.Encoding))
                    {
                        writer.Write(this.Footer.Render(LogEventInfo.CreateNullEvent()) + this.newLineChars);
                    }
                }
            }

            logFile.Archive(archiveFile, this.ArchiveCompression);
        }

        private string GetArchiveFileName(LogFile logFile, LogEventInfo logEvent)
        {
            var archiveDateTime = this.GetArchiveDateTime(logEvent.TimeStamp);
            var lastWriteTime = logFile.LastWriteTimeStamp;
            if (lastWriteTime < archiveDateTime)
            {
                archiveDateTime = lastWriteTime;
            }

            if (this.ArchiveFileName != null)
            {
                var archiveEvent = LogEventInfo.CreateNullEvent();
                archiveEvent.TimeStamp = archiveDateTime;
                return this.ArchiveFileName.Render(archiveEvent);
            }

            var fileName = logFile.FullName;
            var date = archiveDateTime.ToString(this.GetDefaultDatePattern());
            return Path.ChangeExtension(fileName, "." + date + Path.GetExtension(fileName));
        }

        private string GetDefaultDatePattern()
        {
            switch (this.ArchiveEvery)
            {
                case FileArchivePeriod.Year:
                    return "yyyy";
                case FileArchivePeriod.Month:
                    return "yyyy-MM";
                case FileArchivePeriod.Hour:
                    return "yyyy-MM-dd-HH";
                case FileArchivePeriod.Minute:
                    return "yyyy-MM-dd-HH-mm";
                default:
                    return "yyyy-MM-dd";
            }
        }

        private DateTime GetArchiveDateTime(DateTime timeStamp)
        {
            switch (this.ArchiveEvery)
            {
                case FileArchivePeriod.Year:
                    return timeStamp.AddYears(-1);
                case FileArchivePeriod.Month:
                    return timeStamp.AddMonths(-1);
                case FileArchivePeriod.Hour:
                    return timeStamp.AddHours(-1);
                case FileArchivePeriod.Minute:
                    return timeStamp.AddMinutes(-1);
                default:
                    return timeStamp.AddDays(-1);
            }
        }

        private bool ShouldAutoArchive(LogFile logFile, LogEventInfo logEvent)
        {
            if (this.ArchiveEvery == FileArchivePeriod.None)
            {
                return false;
            }

            if (logFile.LastWriteTimeStamp == LogFile.InvalidTimeStamp)
            {
                return false;
            }

            var lastWriteTime = logFile.LastWriteTimeStamp;
            var timeStamp = logEvent.TimeStamp;

            switch (this.ArchiveEvery)
            {
                case FileArchivePeriod.Year:
                    return lastWriteTime.Year != timeStamp.Year;
                case FileArchivePeriod.Month:
                    return lastWriteTime.Year != timeStamp.Year
                        || lastWriteTime.Month != timeStamp.Month;
                case FileArchivePeriod.Hour:
                    return lastWriteTime.Year != timeStamp.Year
                        || lastWriteTime.Month != timeStamp.Month
                        || lastWriteTime.Day != timeStamp.Day
                        || lastWriteTime.Hour != timeStamp.Hour;
                case FileArchivePeriod.Minute:
                    return lastWriteTime.Year != timeStamp.Year
                        || lastWriteTime.Month != timeStamp.Month
                        || lastWriteTime.Day != timeStamp.Day
                        || lastWriteTime.Hour != timeStamp.Hour
                        || lastWriteTime.Minute != timeStamp.Minute;
                default:
                    return lastWriteTime.Year != timeStamp.Year
                        || lastWriteTime.Month != timeStamp.Month
                        || lastWriteTime.Day != timeStamp.Day;
            }
        }
    }
}