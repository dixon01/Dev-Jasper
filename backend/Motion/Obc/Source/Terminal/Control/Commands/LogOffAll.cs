// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogOffAll.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Log off all. Driver and current drive
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;

    /// <summary>
    ///   Logs off all; the driver and current drive
    /// </summary>
    internal class LogOffAll : ProcessCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogOffAll"/> class.
        /// </summary>
        public LogOffAll()
            : base(InputAlphabet.LogOffAll)
        {
        }
    }
}
