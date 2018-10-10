// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecutionCondition.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExecutionConditionEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumerates the available values for the condition event of the execution of a task
    /// </summary>
    public enum ExecutionCondition : sbyte
    {
        /// <summary>
        /// The condition is not set
        /// </summary>
        None = -1,

        // start conditions:

        /// <summary>
        /// The task is executed immeditely 
        /// </summary>
        Immediately,

        // start & stop conditions:

        /// <summary>
        ///  The task is execute at the given date/time
        /// </summary>
        DateTime,

        /// <summary>
        /// The task is executed every day
        /// </summary>
        Daily,

        // stop conditions:

        /// <summary>
        /// The task is executed until it will be aborted ( = revoked ? )
        /// </summary>
        UntilAbort,

        /// <summary>
        /// The task is executed only one time
        /// </summary>
        Once,

        /// <summary>
        /// The task is executed n times. [N_TIMES]
        /// </summary>
        SeveralTimes,

        /// <summary>
        /// The task is executed during a defined period
        /// </summary>
        Duration,
    }
}
