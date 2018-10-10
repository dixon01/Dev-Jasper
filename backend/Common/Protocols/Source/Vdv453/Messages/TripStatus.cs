// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripStatus.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TripStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;

    /// <summary>
    /// Defines the trip status type indicating whether realtime data is available or not.
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable]
    public enum TripStatus : ushort
    {
        /// <summary>
        /// No realtime data available.
        /// </summary>
        Soll,

        /// <summary>
        /// Realtime data available.
        /// </summary>
        Ist
    }
}