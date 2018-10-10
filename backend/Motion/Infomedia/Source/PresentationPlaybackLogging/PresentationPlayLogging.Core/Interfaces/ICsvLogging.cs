// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvLogging.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   The CsvLogging interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Luminator.Utility.CsvFileHelper;

    /// <summary>
    /// The CsvLogging interface.
    /// </summary>
    /// <typeparam name="T">The type to log.
    /// </typeparam>
    public interface ICsvLogging<T> : IDisposable
    {
        /// <summary>The on file moved.</summary>
        event EventHandler<string> OnFileMoved;

        /// <summary>Gets a value indicating whether the logging is started.</summary>
        bool IsStarted { get; }

        /// <summary>
        /// Start Logging.
        /// </summary>
        /// <param name="logFileName">
        /// The csv log file name.
        /// </param>
        /// <param name="rolloverLogOutputFolder">
        /// The rollover Log Output Folder.
        /// </param>
        /// <param name="filenameRolloverType">
        /// The filename Rollover Type.
        /// </param>
        /// <param name="maxFileSize">
        /// The max File Size.
        /// </param>
        /// <param name="maxRecords">
        /// The max Records.
        /// </param>
        /// <exception cref="ApplicationException">Thrown exception
        /// </exception>
        void Start(string logFileName, string rolloverLogOutputFolder, FileNameRolloverType filenameRolloverType, long maxFileSize, long maxRecords);

        /// <summary>Stop Logging.</summary>
        void Stop();

        /// <summary>Write all records.</summary>
        /// <param name="records">The records.</param>
        void WriteAll(ICollection<T> records);

        /// <summary>Write an entry to the log.</summary>
        /// <param name="record">The entry.</param>
        void WriteAsync(T record);

        /// <summary>
        /// Write all entries to the log
        /// </summary>
        /// <param name="records">The records to write to the log</param>
        void WriteAllAsync(ICollection<T> records);

        /// <summary>
        /// The dispose.
        /// </summary>
        new void Dispose();
    }
}