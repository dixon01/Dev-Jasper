// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogPerformanceLogInfoWriter.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NLogInfoPerformanceLogInfoWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Performance
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IPerformanceLogInfoWriter"/> using NLog.
    /// </summary>
    internal partial class NLogPerformanceLogInfoWriter : IPerformanceLogInfoWriter
    {
        private static readonly Logger Logger = LogHelper.GetLogger<NLogPerformanceLogInfoWriter>();

        /// <summary>
        /// Adds a marker for the specified parameters.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="id">The id.</param>
        /// <param name="properties">The properties.</param>
        public void Mark(string category, string tag, int id, params KeyValuePair<string, string>[] properties)
        {
            if (!Logger.IsDebugEnabled)
            {
                return;
            }

            var parameters = new object[] { id, tag };
            var logEventInfo = new LogEventInfo(
                LogLevel.Debug, Logger.Name, CultureInfo.InvariantCulture, "{0} - {1}", parameters);
            logEventInfo.Properties.Add("Category", category);
            if (!string.IsNullOrEmpty(tag))
            {
                logEventInfo.Properties.Add("Tag", tag);
            }

            logEventInfo.Properties.Add("MarkerId", id);
            logEventInfo.Properties.Add("TickCount", Stopwatch.GetTimestamp());

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    logEventInfo.Properties.Add(property.Key, property.Value);
                }
            }

            Logger.Log(logEventInfo);
        }
    }
}