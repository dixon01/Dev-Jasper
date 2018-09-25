// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Net;

    using NLog;

    /// <summary>
    /// Extension methods useful to the communication library.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void TraceHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, params object[] parameters)
        {
            Log(logger, LogLevel.Trace, httpStatusCode, httpStatusMessage, message, null, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void DebugHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, params object[] parameters)
        {
            Log(logger, LogLevel.Debug, httpStatusCode, httpStatusMessage, message, null, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void InfoHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, params object[] parameters)
        {
            Log(logger, LogLevel.Info, httpStatusCode, httpStatusMessage, message, null, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Warn"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void WarnHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, params object[] parameters)
        {
            Log(logger, LogLevel.Warn, httpStatusCode, httpStatusMessage, message, null, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Error"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void ErrorHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, params object[] parameters)
        {
            Log(logger, LogLevel.Error, httpStatusCode, httpStatusMessage, message, null, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Fatal"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void FatalHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, params object[] parameters)
        {
            Log(logger, LogLevel.Fatal, httpStatusCode, httpStatusMessage, message, null, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Warn"/> level with an <paramref name="exception"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public static void WarnExceptionHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, Exception exception, params object[] parameters)
        {
            Log(logger, LogLevel.Warn, httpStatusCode, httpStatusMessage, message, exception, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Error"/> level with an <paramref name="exception"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public static void ErrorExceptionHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, Exception exception, params object[] parameters)
        {
            Log(logger, LogLevel.Error, httpStatusCode, httpStatusMessage, message, exception, parameters);
        }

        /// <summary>
        /// Logs HTTP status information on the <see cref="LogLevel.Fatal"/> level with an <paramref name="exception"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusMessage">The HTTP status message.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public static void FatalExceptionHttpStatus(
            this Logger logger, HttpStatusCode httpStatusCode, string httpStatusMessage, string message, Exception exception, params object[] parameters)
        {
            Log(logger, LogLevel.Fatal, httpStatusCode, httpStatusMessage, message, exception, parameters);
        }

        /// <summary>
        /// Logs the HTTP channel response.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="response">The response.</param>
        public static void LogHttpChannelResponse(this Logger logger, HttpChannelResponse response)
        {
            if (response.IsFaulted)
            {
                if (!logger.IsEnabled(LogLevel.Error))
                {
                    return;
                }

                logger.Error("Invalid response received by the HttpChannel");
                return;
            }

            Contract.Assert(response.StatusCode.HasValue, "If the response is not faulted, it must contain a valid status code");
            if ((int)response.StatusCode.Value < 400)
            {
                logger.DebugHttpStatus(response.StatusCode.Value, response.StatusDescription, "Received a valid HTTP response");
                return;
            }

            logger.ErrorHttpStatus(
                response.StatusCode.Value,
                response.StatusDescription,
                "Received a valid HTTP response containing an error");
        }

        /// <summary>
        /// Sets the status for the <paramref name="httpChannelResponse"/> from the given <paramref name="httpWebResponse"/>.
        /// </summary>
        /// <param name="httpChannelResponse">The HTTP channel response.</param>
        /// <param name="httpWebResponse">The HTTP web response.</param>
        public static void SetStatus(this HttpChannelResponse httpChannelResponse, HttpWebResponse httpWebResponse)
        {
            httpChannelResponse.SetStatus(httpWebResponse.StatusCode, httpWebResponse.StatusDescription);
        }

        private static void Log(
            Logger logger,
            LogLevel level,
            HttpStatusCode httpStatusCode,
            string httpStatusMessage,
            string message,
            Exception exception,
            params object[] parameters)
        {
            if (!logger.IsEnabled(level))
            {
                return;
            }

            var logEventInfo = new LogEventInfo(level, logger.Name, CultureInfo.InvariantCulture, message, parameters)
                {
                    Exception = exception
                };
            logEventInfo.Properties.Add("HttpStatusCode", httpStatusCode);
            if (!string.IsNullOrWhiteSpace(httpStatusMessage))
            {
                logEventInfo.Properties.Add("HttpStatusMessage", httpStatusMessage);
            }

            logger.Log(logEventInfo);
        }
    }
}