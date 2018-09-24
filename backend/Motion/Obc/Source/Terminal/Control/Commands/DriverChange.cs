// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriverChange.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Just changes the driver number
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Just changes the driver number
    /// </summary>
    internal class DriverChange : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverChange"/> class.
        /// </summary>
        public DriverChange()
            : base(MainFieldKey.DriverLogin)
        {
        }
    }
}