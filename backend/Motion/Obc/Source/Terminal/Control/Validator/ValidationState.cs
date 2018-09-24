// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValidationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Validator
{
    /// <summary>
    /// The validation state.
    /// </summary>
    public enum ValidationState
    {
        /// <summary>
        /// Validation is currently running.
        /// </summary>
        Running = 1,

        /// <summary>
        /// The validation was successful.
        /// </summary>
        Success = 2,

        /// <summary>
        /// The validation failed.
        /// </summary>
        Failed = 3,
    }
}