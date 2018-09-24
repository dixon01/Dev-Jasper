// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IODirection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IODirection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32.IO
{
    /// <summary>
    /// The direction of an I/O.
    /// </summary>
    public enum IODirection
    {
        /// <summary>
        /// The I/O is an output which can be read and written.
        /// </summary>
        Output = 0,

        /// <summary>
        /// The I/O is an input which can only be read.
        /// </summary>
        Input = 1
    }
}