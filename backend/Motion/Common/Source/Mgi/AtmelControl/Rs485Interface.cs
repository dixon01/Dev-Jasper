// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rs485Interface.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Rs485Interface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// The RS485 interface state.
    /// </summary>
    public enum Rs485Interface
    {
        /// <summary>
        /// Interface connected to CPU.
        /// </summary>
        Cpu = 0,

        /// <summary>
        /// Interface connected to at91 (default)
        /// </summary>
        At91 = 1
    }
}