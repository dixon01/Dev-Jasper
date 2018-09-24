// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleCtrlEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConsoleCtrlEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers.Console
{
    using System;

    /// <summary>
    /// The console control event arguments.
    /// </summary>
    public class ConsoleCtrlEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCtrlEventArgs"/> class.
        /// </summary>
        /// <param name="ctrlType">
        /// The control type.
        /// </param>
        public ConsoleCtrlEventArgs(CtrlTypes ctrlType)
        {
            this.CtrlType = ctrlType;
        }

        /// <summary>
        /// Gets the control type.
        /// </summary>
        public CtrlTypes CtrlType { get; private set; }
    }
}