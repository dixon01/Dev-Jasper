// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Shows a number input screen to enter system codes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Shows a number input screen to enter system codes
    /// </summary>
    internal class SystemCode : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCode"/> class.
        /// </summary>
        public SystemCode()
            : base(MainFieldKey.SystemCode)
        {
        }
    }
}
