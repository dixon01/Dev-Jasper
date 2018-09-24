// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceLogger.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PerformanceLogger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Performance
{
    using System.Collections.Generic;

    /// <summary>
    /// Static class containing method that can be used to log information that can be used later on for statistics
    /// about application performance.
    /// </summary>
    public static partial class PerformanceLogger
    {
        private static readonly IPerformanceLogInfoWriter PerformanceLogInfoWriter =
            new NLogPerformanceLogInfoWriter();

        /// <summary>
        /// Adds a performance marker with the given id, tag and optional properties.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="id">The id.</param>
        /// <param name="properties">The properties.</param>
        public static void Mark(string category, string tag, int id, params KeyValuePair<string, string>[] properties)
        {
            PerformanceLogInfoWriter.Mark(category, tag, id, properties);
        }
    }
}