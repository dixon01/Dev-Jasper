// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PassengerCount.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Shows a number input screen to enter the number of passengers
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Shows a number input screen to enter the number of passengers
    /// </summary>
    internal class PassengerCount : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassengerCount"/> class.
        /// </summary>
        public PassengerCount()
            : base(MainFieldKey.PassengerCount)
        {
        }
    }
}
