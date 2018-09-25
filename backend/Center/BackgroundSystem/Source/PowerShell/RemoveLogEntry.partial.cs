// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveLogEntry.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoveLogEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Log;

    using Nito.AsyncEx.Synchronous;

    /// <summary>
    /// Removes a single log entry or a range of log entries.
    /// </summary>
    public partial class RemoveLogEntry
    {
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Filter", Position = 3)]
        public LogEntryFilter Filter { get; set; }

        partial void OnRemoving(ref bool shouldStop)
        {
            if (this.ParameterSetName == "Filter")
            {
                shouldStop = true;
                using (var scope = this.CreateDataChannelScope<ILogEntryDataService>())
                {
                    var result = scope.Channel.DeleteAsync(this.Filter).WaitAndUnwrapException();
                    this.WriteObject(result);
                }
            }
        }
    }
}