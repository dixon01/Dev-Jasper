// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    using Gorba.Common.Configuration.Obc.Terminal;

    /// <summary>
    /// The command event arguments.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">
        /// The command name.
        /// </param>
        public CommandEventArgs(CommandName command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public CommandName Command { get; private set; }
    }
}