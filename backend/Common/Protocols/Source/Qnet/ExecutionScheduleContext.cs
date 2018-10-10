// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecutionScheduleContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Define the execution schedule context for task that need these settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Define the execution schedule context for task that need these settings.
    /// </summary>
    public class ExecutionScheduleContext
    {
        /// <summary>
        /// Gets or sets the start date that represents the optional start date of the task. 
        /// If equals <b>null</b>, the task should start immediately.
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// Gets or sets the stop date that represents the optional stop date of the task. 
        /// If equals <b>null</b>, the task is valid until explicit revoke.
        /// </summary>
        public DateTime? StopDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task should be scheduled daily.
        /// </summary>
        public bool IsScheduledDaily { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of repetitions of the execution. 
        /// As soon as the number of times is reached, then the activity is deleted automatically.
        /// </summary>
        public ushort NumberOfTimes { get; set; }
    }
}
