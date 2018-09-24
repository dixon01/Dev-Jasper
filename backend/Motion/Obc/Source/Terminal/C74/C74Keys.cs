// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C74Keys.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the C74Keys type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74
{
    /// <summary>
    /// Keys available on the Thoreb C74.
    /// </summary>
    public enum C74Keys
    {
        /// <summary>
        /// No key is pressed.
        /// </summary>
        None,

        /// <summary>
        /// The left-most "back" key is pressed.
        /// </summary>
        Back,

        /// <summary>
        /// The second button from the left ("up") is pressed.
        /// </summary>
        Up,

        /// <summary>
        /// The third button from the left ("down") is pressed.
        /// </summary>
        Down,

        /// <summary>
        /// The OK button is pressed.
        /// </summary>
        Ok
    }
}