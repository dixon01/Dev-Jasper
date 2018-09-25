// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandsEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandsEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Event arguments with an array of commands.
    /// </summary>
    public class UpdateCommandsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandsEventArgs"/> class.
        /// </summary>
        /// <param name="commands">
        /// The commands.
        /// </param>
        public UpdateCommandsEventArgs(params UpdateCommand[] commands)
        {
            this.Commands = commands;
        }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        public UpdateCommand[] Commands { get; private set; }
    }
}