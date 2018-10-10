// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Result type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;

    /// <summary>
    /// Result type (ErgebnisType)
    /// </summary>
    [Serializable]
    public enum Result
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// indicates result is ok
        /// </summary>
        ok,

        /// <summary>
        /// indicates result is not ok
        /// </summary>
        notok

        // ReSharper restore InconsistentNaming
    }
}