// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Log type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    using System.Activities;

    using Microsoft.TeamFoundation.Build.Workflow.Activities;

    /// <summary>
    /// Defines a wrapper around the <see cref="CodeActivityContext"/> to allow logging.
    /// </summary>
    internal class Log : ILog
    {
        private readonly CodeActivityContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Log(CodeActivityContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Logs a message of low priority to the build server
        /// </summary>
        /// <param name="msg">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Message(string msg, params object[] args)
        {
            this.context.TrackBuildMessage(string.Format(msg, args));
        }

        /// <summary>
        /// Logs a warning message of medium priority to the build server
        /// </summary>
        /// <param name="msg">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Warning(string msg, params object[] args)
        {
            this.context.TrackBuildWarning(string.Format(msg, args));
        }

        /// <summary>
        /// Logs an error message of high priority to the build server
        /// </summary>
        /// <param name="msg">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Error(string msg, params object[] args)
        {
            this.context.TrackBuildError(string.Format(msg, args));
        }
    }
}