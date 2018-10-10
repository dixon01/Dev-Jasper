// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorNumber.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorNumber type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    /// <summary>
    /// The error number.
    /// Maximum value is 15 (4 bits).
    /// </summary>
    internal enum ErrorNumber
    {
        /// <summary>
        /// Everything is OK.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Something was not found.
        /// </summary>
        NotFound = 1,

        /// <summary>
        /// We received wrong/bad data.
        /// </summary>
        BadData = 2,
    }
}