// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleCtrlHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConsoleCtrlHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers.Console
{
    using System;

    /// <summary>
    /// Handler for console control events.
    /// </summary>
    public sealed partial class ConsoleCtrlHandler : IDisposable
    {
        /// <summary>
        /// Event that is risen when the console is closed.
        /// </summary>
        public event EventHandler<ConsoleCtrlEventArgs> ConsoleClosed;

        private int ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            var handler = this.ConsoleClosed;
            if (handler != null)
            {
                handler(this, new ConsoleCtrlEventArgs(ctrlType));
            }

            return 0;
        }
    }
}