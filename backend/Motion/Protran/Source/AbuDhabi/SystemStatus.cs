// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemStatus.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi
{
    /// <summary>
    /// Enumeration class with the possible system status integer values available in
    /// the Abu Dhabi system.
    /// </summary>
    internal static class SystemStatus
    {
        /// <summary>
        /// No data source is available
        /// </summary>
        public static readonly int None = 0;

        /// <summary>
        /// Data is taken from INIT OBU using ISI protocol
        /// </summary>
        public static readonly int Isi = 1;

        /// <summary>
        /// Data is taken from CU5 using IBIS protocol
        /// </summary>
        public static readonly int Ibis = 2;
    }
}
