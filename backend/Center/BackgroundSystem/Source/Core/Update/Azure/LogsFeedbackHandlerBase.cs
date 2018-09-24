// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsFeedbackHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The base class for the handling of feedback log files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update.Azure
{
    using System.Threading.Tasks;

    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// The base class for the handling of feedback log files.
    /// </summary>
    public abstract class LogsFeedbackHandlerBase
    {
        /// <summary>
        /// Configures the handler.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public virtual void Configure(IUpdateContext context)
        {
        }

        /// <summary>
        /// Gets all the log files from the directory defined in <see cref="Configure"/>.
        /// </summary>
        /// <returns>
        /// The list of log files related to the unit.
        /// </returns>
        public virtual Task<IReceivedLogFile[]> FindLogFilesAsync()
        {
            return Task.FromResult(new IReceivedLogFile[0]);
        }

        /// <summary>
        /// Deletes the given log file from the storage.
        /// </summary>
        /// <param name="logFile">
        /// The log file, must be one returned by <see cref="FindLogFilesAsync"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await.
        /// </returns>
        public virtual Task DeleteLogFileAsync(IReceivedLogFile logFile)
        {
            return Task.FromResult(0);
        }
    }
}
