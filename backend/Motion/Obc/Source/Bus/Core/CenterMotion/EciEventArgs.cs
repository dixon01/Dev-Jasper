// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.CenterMotion
{
    using System;

    using Gorba.Common.Protocols.Eci.Messages;

    /// <summary>
    /// The eci even args.
    /// </summary>
    public class EciEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EciEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public EciEventArgs(EciMessageBase message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public EciMessageBase Message { get; private set; }
    }
}
