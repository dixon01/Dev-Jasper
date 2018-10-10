// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputAlphabet.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Input alphabet for the state machine (DFA)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    /// <summary>
    ///   Input alphabet for the state machine (DFA)
    /// </summary>
    internal enum InputAlphabet
    {
        /// <summary>
        /// A special destination is set.
        /// </summary>
        SpecialDestSet = 2,

        /// <summary>
        /// A regular block is set.
        /// </summary>
        BlockSet = 3,

        /// <summary>
        /// The driver is completely logged off.
        /// </summary>
        LogOffAll = 4,

        /// <summary>
        /// The driver is logged of from the current drive (special destination or block)
        /// </summary>
        LogOffDrive = 5,
    }
}