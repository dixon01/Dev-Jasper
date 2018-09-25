// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevels.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogLevels type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IntegratedLoggingTest.Config
{
    /// <summary>
    /// The log levels cases.
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        /// The low level = 1 sec.
        /// </summary>
        Low,

        /// <summary>
        /// The med level = 1/10 sec.
        /// </summary>
        Mid,

        /// <summary>
        /// The high level = 1/100 sec.
        /// </summary>
        High
    }
}
