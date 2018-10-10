// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeSpecialCharacters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates all special displayed values for led countdown display (type C)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumerates all special displayed values for led countdown display (type C)
    /// </summary>
    public enum RealtimeSpecialCharacters
    {
        /// <summary>
        /// Digit 9.
        /// </summary>
        Digit9 = 9,

        /// <summary>
        /// Digit 8.
        /// </summary>
        Digit8 = 8,

        /// <summary>
        /// Digit 7.
        /// </summary>
        Digit7 = 7,

        /// <summary>
        /// Digit 6.
        /// </summary>
        Digit6 = 6,

        /// <summary>
        /// Digit 5.
        /// </summary>
        Digit5 = 5,

        /// <summary>
        /// Digit 4.
        /// </summary>
        Digit4 = 4,

        /// <summary>
        /// Digit 3.
        /// </summary>
        Digit3 = 3,

        /// <summary>
        /// Digit 2.
        /// </summary>
        Digit2 = 2,

        /// <summary>
        /// Digit 1.
        /// </summary>
        Digit1 = 1,

        /// <summary>
        /// Digit 0.
        /// </summary>
        Digit0 = 0,

        /// <summary>
        /// Special black character [ ] (Legacy code : DISPLAY_SHOW_BLACK)
        /// </summary>
        DisplayedCharBlack = -1,

        /// <summary>
        /// Special black character [-] (Legacy code : DISPLAY_SHOW_BLACK)
        /// </summary>
        DisplayedDashes = -2,

        /// <summary>
        /// Special black character [A] (Legacy code : DISPLAY_SHOW_A)
        /// </summary>
        DisplayedCharA = -3,

        /// <summary>
        /// Special black character [C] (Legacy code : DISPLAY_SHOW_C)
        /// </summary>
        DisplayedCharC = -4, // [C]

        /// <summary>
        /// Special black character [E] (Legacy code : DISPLAY_SHOW_E)
        /// </summary>
        DisplayedCharE = -5, // [E]

        /// <summary>
        /// Special black character [F] (Legacy code : DISPLAY_SHOW_F)
        /// </summary>
        DisplayedCharF = -6, // [F]

        /// <summary>
        /// Special black character [H] (Legacy code : DISPLAY_SHOW_H)
        /// </summary>
        DisplayedCharH = -7,

        /// <summary>
        /// Special black character [L] (Legacy code : DISPLAY_SHOW_L)
        /// </summary>
        DisplayedCharL = -8,

        /// <summary>
        /// Special black character [P] (Legacy code : DISPLAY_SHOW_P)
        /// </summary>
        DisplayedCharP = -9,

        /// <summary>
        /// Special black character [U] (Legacy code : DISPLAY_SHOW_U)
        /// </summary>
        DisplayedCharU = -10
    }
}