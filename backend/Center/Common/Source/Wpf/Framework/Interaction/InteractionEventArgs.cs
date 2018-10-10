// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;

    /// <summary>
    /// Defines the event args for interaction requests.
    /// </summary>
    public class InteractionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionEventArgs"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal InteractionEventArgs(InteractionType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        internal InteractionType Type { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Type.ToString();
        }
    }
}