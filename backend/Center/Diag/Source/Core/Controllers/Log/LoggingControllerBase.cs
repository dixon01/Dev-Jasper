// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoggingControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Log
{
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Center.Diag.Core.ViewModels.Log;
    using Gorba.Common.Medi.Core.Logging;

    using LogLevel = NLog.LogLevel;

    /// <summary>
    /// Base class for all logging controllers.
    /// </summary>
    public abstract class LoggingControllerBase : SynchronizableControllerBase
    {
        private readonly LoggingViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingControllerBase"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The logging view model to which this controller will connect.
        /// </param>
        protected LoggingControllerBase(LoggingViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += this.ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Subclasses must start the listening to log events in this method.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Subclasses must stop listening to log events in this method.
        /// </summary>
        protected abstract void Stop();

        /// <summary>
        /// Subclasses must change the minimum level they are listening to in this method.
        /// </summary>
        /// <param name="newLevel">
        /// The new level.
        /// </param>
        protected abstract void UpdateMinimumLevel(LogLevel newLevel);

        /// <summary>
        /// Gets the currently set minimum NLog log level.
        /// </summary>
        /// <returns>
        /// The NLog log level.
        /// </returns>
        protected LogLevel GetMinimumLevel()
        {
            switch (this.viewModel.MinimumLevel)
            {
                case ViewModels.Log.LogLevel.Trace:
                    return LogLevel.Trace;
                case ViewModels.Log.LogLevel.Debug:
                    return LogLevel.Debug;
                case ViewModels.Log.LogLevel.Info:
                    return LogLevel.Info;
                case ViewModels.Log.LogLevel.Warn:
                    return LogLevel.Warn;
                case ViewModels.Log.LogLevel.Error:
                    return LogLevel.Error;
                case ViewModels.Log.LogLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Info;
            }
        }

        /// <summary>
        /// Adds a log message to the underlying view model.
        /// </summary>
        /// <param name="logEvent">
        /// The log event.
        /// </param>
        /// <param name="application">
        /// The application from which this event was received.
        /// </param>
        protected void AddLogMessage(LogEventArgs logEvent, RemoteAppViewModel application)
        {
            ViewModels.Log.LogLevel logLevel;
            if (logEvent.Level == LogLevel.Trace)
            {
                logLevel = ViewModels.Log.LogLevel.Trace;
            }
            else if (logEvent.Level == LogLevel.Debug)
            {
                logLevel = ViewModels.Log.LogLevel.Debug;
            }
            else if (logEvent.Level == LogLevel.Info)
            {
                logLevel = ViewModels.Log.LogLevel.Info;
            }
            else if (logEvent.Level == LogLevel.Warn)
            {
                logLevel = ViewModels.Log.LogLevel.Warn;
            }
            else if (logEvent.Level == LogLevel.Error)
            {
                logLevel = ViewModels.Log.LogLevel.Error;
            }
            else if (logEvent.Level == LogLevel.Fatal)
            {
                logLevel = ViewModels.Log.LogLevel.Fatal;
            }
            else
            {
                // unknown level, let's assume "info"
                logLevel = ViewModels.Log.LogLevel.Info;
            }

            var logEntry = new LogEntryViewModel(
                application,
                logEvent.Timestamp,
                logLevel,
                logEvent.LoggerName,
                logEvent.Message,
                logEvent.Exception);

            this.StartNew(() => this.viewModel.Messages.Add(logEntry));
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnabled":
                    if (this.viewModel.IsEnabled)
                    {
                        this.Start();
                    }
                    else
                    {
                        this.Stop();
                    }

                    break;

                case "MinimumLevel":

                    this.UpdateMinimumLevel(this.GetMinimumLevel());
                    break;
            }
        }
    }
}
