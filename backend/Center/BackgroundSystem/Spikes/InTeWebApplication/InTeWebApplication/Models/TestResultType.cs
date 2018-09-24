// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestResultType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestResultType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Models
{
    /// <summary>
    /// The test result type.
    /// </summary>
    public enum TestResultType
    {
        /// <summary>
        /// The inconclusive.
        /// </summary>
        Inconclusive = 0,

        /// <summary>
        /// The warning.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// The success.
        /// </summary>
        Success = 2,

        /// <summary>
        /// The error.
        /// </summary>
        Error = 3
    }
}