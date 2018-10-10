// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogEntryViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Log
{
    using System;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.ViewModels.App;

    /// <summary>
    /// The log entry view model.
    /// This class is not implementing <see cref="ViewModelBase"/> because
    /// it would be too heavy for an object that is read-only.
    /// </summary>
    public class LogEntryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryViewModel"/> class.
        /// </summary>
        /// <param name="application">
        /// The remote application from which this entry comes.
        /// </param>
        /// <param name="timestampUtc">
        /// The timestamp in UTC.
        /// </param>
        /// <param name="level">
        /// The log level.
        /// </param>
        /// <param name="loggerName">
        /// The logger name.
        /// </param>
        /// <param name="message">
        /// The complete formatted log message.
        /// </param>
        /// <param name="exceptionInfo">
        /// The exception information, this can be null.
        /// </param>
        public LogEntryViewModel(
            RemoteAppViewModel application,
            DateTime timestampUtc,
            LogLevel level,
            string loggerName,
            string message,
            string exceptionInfo)
        {
            this.Application = application;
            this.ExceptionInfo = exceptionInfo;
            this.Message = message;
            this.LoggerName = loggerName;
            this.Level = level;
            this.TimestampUtc = timestampUtc;
            this.TimestampLocal = timestampUtc.ToLocalTime();

            var index = loggerName.LastIndexOf('.');
            this.LoggerShortName = index >= 0 ? loggerName.Substring(index + 1) : loggerName;
        }

        /// <summary>
        /// Gets the remote application from which this entry comes.
        /// </summary>
        public RemoteAppViewModel Application { get; private set; }

        /// <summary>
        /// Gets the timestamp in UTC.
        /// </summary>
        public DateTime TimestampUtc { get; private set; }

        /// <summary>
        /// Gets the timestamp in local time.
        /// </summary>
        public DateTime TimestampLocal { get; private set; }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// Gets the logger name.
        /// </summary>
        public string LoggerName { get; private set; }

        /// <summary>
        /// Gets the short logger name.
        /// </summary>
        public string LoggerShortName { get; private set; }

        /// <summary>
        /// Gets the complete formatted log message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception information, this can be null.
        /// </summary>
        public string ExceptionInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this view model can be expanded (i.e. has exception information).
        /// </summary>
        public bool IsExpandable
        {
            get
            {
                return !string.IsNullOrEmpty(this.ExceptionInfo);
            }
        }
    }
}
