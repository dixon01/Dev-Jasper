// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILog.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    /// <summary>
    /// Logging interface that can be used by components to log to the build server.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs a message of low priority to the build server
        /// </summary>
        /// <param name="msg">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        void Message(string msg, params object[] args);

        /// <summary>
        /// Logs a warning message of medium priority to the build server
        /// </summary>
        /// <param name="msg">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        void Warning(string msg, params object[] args);

        /// <summary>
        /// Logs an error message of high priority to the build server
        /// </summary>
        /// <param name="msg">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        void Error(string msg, params object[] args);
    }
}
