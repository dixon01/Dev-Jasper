// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.PerformanceLogger.Component
{
    /// <summary>
    /// The extension methods for the logger.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Logs an incoming time table entry.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public static void LogBegin(int id)
        {
            Gorba.Common.Utility.Core.Performance.PerformanceLogger.Mark("Test", "Begin", id);
        }

        /// <summary>
        /// Logs an outgoing time table message.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public static void LogEnd(int id)
        {
            Gorba.Common.Utility.Core.Performance.PerformanceLogger.Mark("Test", "End", id);
        }
    }
}
