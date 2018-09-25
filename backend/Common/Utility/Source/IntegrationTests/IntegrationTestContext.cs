// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTestContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegrationTestContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.IntegrationTests
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Defines a context object to be used within integration tests.
    /// </summary>
    public class IntegrationTestContext
    {
        private readonly IntegrationTestResult result;

        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestContext"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="minLogLevel">The min log level. If null, the log level will be set by default to Info.</param>
        private IntegrationTestContext(string name, string logFilePath, LogLevel minLogLevel)
        {
            this.result = new IntegrationTestResult(name) { Start = TimeProvider.Current.UtcNow };
            if (!string.IsNullOrWhiteSpace(logFilePath))
            {
                this.ConfigureForLogging(logFilePath);
                this.logger = LogManager.GetLogger(name);
            }

            this.MinLogLevel = minLogLevel;
        }

        /// <summary>
        /// Gets the minimum log level value.
        /// </summary>
        public LogLevel MinLogLevel { get; private set; }

        /// <summary>
        /// Gets the name of this test context.
        /// </summary>
        public string Name
        {
            get
            {
                return this.result.Name;
            }
        }

        /// <summary>
        /// Starts the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="minLogLevel">The minimum log level value.</param>
        /// <returns>
        /// A new context for an integration test. If the <paramref name="logFilePath"/> is not null, the system
        /// will be configured to use it as target location for logs.
        /// </returns>
        public static IntegrationTestContext Start(string name, string logFilePath = null, LogLevel minLogLevel = null)
        {
            return new IntegrationTestContext(name, logFilePath, minLogLevel);
        }

        /// <summary>
        /// Adds a "failure" to this test context.
        /// </summary>
        /// <param name="reason">The reason of the failure.</param>
        public void Fail(string reason)
        {
            var failure = new Failure { Reason = reason, Time = TimeProvider.Current.UtcNow };
            if (this.logger != null)
            {
                this.logger.Error(
                    "Added failure at '{0}'. Reason: '{1}'", failure.Time.ToLocalTime().ToShortTimeString(), reason);
            }

            this.result.Failed = true;
            this.result.Failures.Add(failure);
        }

        /// <summary>
        /// Adds a "failure" to this test context corresponding to the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Fail(Exception exception)
        {
            var failure = new Failure { Reason = exception.Message, Time = TimeProvider.Current.UtcNow };
            if (this.logger != null)
            {
                this.logger.Error(exception, string.Format("Added failure at '{0}'.", failure.Time.ToLocalTime().ToShortTimeString()));
            }

            this.result.Failed = true;
            this.result.Failures.Add(failure);
        }

        /// <summary>
        /// Logs the specified message to the Trace log.
        /// </summary>
        /// <param name="message">The format.</param>
        /// <param name="args">The args.</param>
        public void Trace(string message, params object[] args)
        {
            if (this.logger != null)
            {
                this.logger.Trace(message, args);
            }
        }

        /// <summary>
        /// Logs the specified message to the Debug log.
        /// </summary>
        /// <param name="message">The format.</param>
        /// <param name="args">The args.</param>
        public void Debug(string message, params object[] args)
        {
            if (this.logger != null)
            {
                this.logger.Debug(message, args);
            }
        }

        /// <summary>
        /// Logs the specified message to the Info log.
        /// </summary>
        /// <param name="message">The format.</param>
        /// <param name="args">The args.</param>
        public void Info(string message, params object[] args)
        {
            if (this.logger != null)
            {
                this.logger.Info(message, args);
            }
        }

        /// <summary>
        /// Logs the specified message to the Warn log.
        /// </summary>
        /// <param name="message">The format.</param>
        /// <param name="args">The args.</param>
        public void Warn(string message, params object[] args)
        {
            if (this.logger != null)
            {
                this.logger.Warn(message, args);
            }
        }

        /// <summary>
        /// Logs the specified message to the Error log.
        /// </summary>
        /// <param name="message">The format.</param>
        /// <param name="args">The args.</param>
        public void Error(string message, params object[] args)
        {
            if (this.logger != null)
            {
                this.logger.Error(message, args);
            }
        }

        /// <summary>
        /// Logs the specified message to the Fatal log.
        /// </summary>
        /// <param name="message">The format.</param>
        /// <param name="args">The args.</param>
        public void Fatal(string message, params object[] args)
        {
            if (this.logger != null)
            {
                this.logger.Fatal(message, args);
            }
        }

        /// <summary>
        /// Completes this test context and returns the result.
        /// </summary>
        /// <returns>The result of the test.</returns>
        public IntegrationTestResult Complete()
        {
            LogManager.Flush();
            this.result.End = TimeProvider.Current.UtcNow;
            return this.result;
        }

        /// <summary>
        /// Configures for logging.
        /// </summary>
        /// <param name="logFilePath">The full path to the log file.</param>
        private void ConfigureForLogging(string logFilePath)
        {
            var config = new NLog.Config.LoggingConfiguration();
            const string Layout = "${time} ${uppercase:${level}} ${logger} ${message}${onexception: ${newline}"
                                  + "${exception:format=tostring}}";
            var fileTarget = new NLog.Targets.FileTarget
                {
                    AutoFlush = true,
                    CreateDirs = true,
                    ConcurrentWrites = false,
                    FileName = logFilePath,
                    KeepFileOpen = false,
                    Name = "file",
                    Layout = Layout
                };
            config.AddTarget(fileTarget.Name, fileTarget);
            var consoleTarget = new NLog.Targets.ConsoleTarget { Error = true, Name = "console", Layout = Layout };
            config.AddTarget(consoleTarget.Name, consoleTarget);
            var rule = new NLog.Config.LoggingRule("*", this.MinLogLevel ?? LogLevel.Info, fileTarget);
            config.LoggingRules.Add(rule);
            rule = new NLog.Config.LoggingRule("*", this.MinLogLevel ?? LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }
    }
}
