// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The log level.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels
{
    /// <summary>
    /// The log level.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The trace level (most verbose).
        /// </summary>
        Trace = 0,

        /// <summary>
        /// The debug level.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// The info level (default level).
        /// </summary>
        Info = 2,

        /// <summary>
        /// The warning level.
        /// </summary>
        Warn = 3,

        /// <summary>
        /// The error level.
        /// </summary>
        Error = 4,

        /// <summary>
        /// The fatal level (most severe).
        /// </summary>
        Fatal = 5,
    }
}