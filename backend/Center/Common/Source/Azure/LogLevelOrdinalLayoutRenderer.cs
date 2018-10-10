// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevelOrdinalLayoutRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogLevelOrdinalLayoutRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Azure
{
    using System.Text;

    using NLog;
    using NLog.LayoutRenderers;

    /// <summary>
    /// Layout renderer that outputs the ordinal value of the logger.
    /// </summary>
    [LayoutRenderer("levelOrdinal")]
    public class LogLevelOrdinalLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Gets the ordinal value for the given log level name.
        /// </summary>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="ordinal">
        /// The ordinal.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the given string was a valid level or not.
        /// </returns>
        public static bool TryGetOrdinalValue(string logLevel, out int ordinal)
        {
            if (string.IsNullOrEmpty(logLevel))
            {
                ordinal = -1;
                return false;
            }

            switch (logLevel.ToLower())
            {
                case "trace":
                    ordinal = 0;
                    break;
                case "debug":
                    ordinal = 1;
                    break;
                case "info":
                    ordinal = 2;
                    break;
                case "warn":
                    ordinal = 3;
                    break;
                case "error":
                    ordinal = 4;
                    break;
                case "fatal":
                    ordinal = 5;
                    break;
                default:
                    ordinal = -1;
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            int ordinal;
            if (!TryGetOrdinalValue(logEvent.Level.Name, out ordinal))
            {
                return;
            }

            builder.Append(ordinal);
        }
    }
}