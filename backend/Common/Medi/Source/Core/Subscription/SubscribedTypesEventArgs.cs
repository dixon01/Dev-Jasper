// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscribedTypesEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SubscribedTypesEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;

    /// <summary>
    /// Event arguments for type subscription add and remove events.
    /// </summary>
    internal class SubscribedTypesEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribedTypesEventArgs"/> class.
        /// </summary>
        /// <param name="added">
        /// A value indicating whether the provided <see cref="Types"/> are being added.
        /// </param>
        /// <param name="types">
        /// The types.
        /// </param>
        public SubscribedTypesEventArgs(bool added, params TypeName[] types)
        {
            this.Added = added;
            this.Types = types;
        }

        /// <summary>
        /// Gets a value indicating whether the provided <see cref="Types"/> are being added.
        /// </summary>
        public bool Added { get; private set; }

        /// <summary>
        /// Gets the types that are added/removed.
        /// </summary>
        public TypeName[] Types { get; private set; }
    }
}
