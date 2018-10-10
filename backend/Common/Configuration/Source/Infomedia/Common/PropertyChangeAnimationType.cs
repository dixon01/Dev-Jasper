// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangeAnimationType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyChangeAnimationType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Common
{
    /// <summary>
    /// Type of a <see cref="PropertyChangeAnimation"/>.
    /// </summary>
    public enum PropertyChangeAnimationType
    {
        /// <summary>
        /// The property is not animated when it changes.
        /// This is the default.
        /// </summary>
        None,

        /// <summary>
        /// Changes the property from the old to the new value
        /// in a linear way.
        /// </summary>
        Linear,

        /// <summary>
        /// Fades the old property out first and then fades in the
        /// new property.
        /// </summary>
        FadeThroughNothing
    }
}