// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessPriorityClass.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessPriorityClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace System.Diagnostics
{
    /// <summary>
    /// The process priority class found in System.Diagnostics in the full framework.
    /// </summary>
    public enum ProcessPriorityClass
    {
        /// <summary>
        /// Specifies that the process has no special scheduling needs.
        /// </summary>
        Normal = 32,

        /// <summary>
        /// Specifies that the threads of this process run only when the system is idle,
        /// such as a screen saver. The threads of the process are preempted by the threads
        /// of any process running in a higher priority class.
        /// </summary>
        Idle = 64,

        /// <summary>
        /// Specifies that the process performs time-critical tasks that must be executed immediately,
        /// such as the Task List dialog, which must respond quickly when called by the user,
        /// regardless of the load on the operating system.
        /// The threads of the process preempt the threads of normal or idle priority class processes.
        /// </summary>
        High = 128,

        /// <summary>
        /// Specifies that the process has the highest possible priority.
        /// </summary>
        RealTime = 256,

        /// <summary>
        /// Specifies that the process has priority above <see cref="ProcessPriorityClass.Idle" />
        /// but below <see cref="ProcessPriorityClass.Normal" />.
        /// </summary>
        BelowNormal = 16384,

        /// <summary>
        /// Specifies that the process has priority above <see cref="ProcessPriorityClass.Normal" />
        /// but below <see cref="ProcessPriorityClass.High" />.
        /// </summary>
        AboveNormal = 32768
    }
}

// ReSharper restore CheckNamespace