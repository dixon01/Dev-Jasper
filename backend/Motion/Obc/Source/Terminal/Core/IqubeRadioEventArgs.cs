// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeRadioEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IqubeRadioEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The iqube radio event arguments.
    /// </summary>
    public class IqubeRadioEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IqubeRadioEventArgs"/> class.
        /// </summary>
        /// <param name="receivers">
        /// The receivers.
        /// </param>
        public IqubeRadioEventArgs(params string[] receivers)
        {
            this.Receivers = receivers;
        }

        /// <summary>
        /// Gets the receivers.
        /// </summary>
        public string[] Receivers { get; private set; }
    }
}