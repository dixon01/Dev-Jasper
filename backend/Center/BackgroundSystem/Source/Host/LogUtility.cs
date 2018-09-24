// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Host
{
    using System;
    using System.Diagnostics;

    using NLog;

    /// <summary>
    /// Log utility methods.
    /// </summary>
    public static class LogUtility
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Maps a LogLevel to the corresponding SourceLevels value.
        /// </summary>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="sourceLevel">
        /// The source level.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was recognized; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="logLevel"/> is null.</exception>
        public static bool MapNLogLevelToSourceLevel(this LogLevel logLevel, out SourceLevels sourceLevel)
        {
            if (logLevel == null)
            {
                throw new ArgumentNullException("logLevel");
            }

            return MapNLogLevelToSourceLevel(logLevel.Name, out sourceLevel);
        }

        /// <summary>
        /// Maps a LogLevel string to the corresponding SourceLevels value.
        /// </summary>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="sourceLevel">
        /// The source level.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was recognized; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The log level is null or empty.</exception>
        public static bool MapNLogLevelToSourceLevel(string logLevel, out SourceLevels sourceLevel)
        {
            if (string.IsNullOrEmpty(logLevel))
            {
                throw new ArgumentOutOfRangeException("logLevel", "Log level must be a non-empty string");
            }

            switch (logLevel)
            {
                case "Off":
                    sourceLevel = SourceLevels.Off;
                    break;

                case "Fatal":
                    sourceLevel = SourceLevels.Critical;
                    break;

                case "Error":
                    sourceLevel = SourceLevels.Error;
                    break;

                case "Warn":
                    sourceLevel = SourceLevels.Warning;
                    break;

                case "Info":
                    sourceLevel = SourceLevels.Information;
                    break;

                case "Debug":
                    sourceLevel = SourceLevels.Verbose;
                    break;

                case "Trace":
                    sourceLevel = SourceLevels.All;
                    break;

                default:
                    Logger.Info("Unknown desired log level '{0}', setting Off SourceLevels.", logLevel);
                    sourceLevel = SourceLevels.Off;
                    return false;
            }

            return true;
        }
    }
}
