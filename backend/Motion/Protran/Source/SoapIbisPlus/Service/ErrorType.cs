// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    /// <summary>
    /// The error type.
    /// Maximum value is 15 (4 bits).
    /// </summary>
    internal enum ErrorType
    {
        /// <summary>
        /// No error.
        /// </summary>
        None = 0,

        /// <summary>
        /// Error with the entire trip data.
        /// </summary>
        Trip = 1,

        /// <summary>
        /// Error with the current stop data.
        /// </summary>
        Stop = 2,
    }
}