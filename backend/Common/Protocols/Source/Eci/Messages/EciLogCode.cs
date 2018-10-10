// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciLogCode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The eci log code.
    /// </summary>
    public enum EciLogCode
    {
        /// <summary>
        /// The debug.
        /// </summary>
        Debug = 0,

        /// <summary>
        /// The info.
        /// </summary>
        Info = 1,

        /// <summary>
        /// The warning.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The error.
        /// </summary>
        Error = 3,

        /// <summary>
        /// The fatal.
        /// </summary>
        Fatal = 4
    }
}
