// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncCmdlet.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncCmdlet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Nito.AsyncEx;

    using NLog;

    /// <summary>
    /// Defines a cmdlet with async execution implementation.
    /// </summary>
    public abstract class AsyncCmdlet : PSCmdlet
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected abstract Task ProcessRecordAsync();

        /// <inheritdoc />
        protected sealed override void ProcessRecord()
        {
            Logger.Trace("Executing AsyncCmdlet");
            AsyncContext.Run(async () => await this.ProcessRecordAsync());
            Logger.Trace("AsyncCmdlet executed");
        }
    }
}