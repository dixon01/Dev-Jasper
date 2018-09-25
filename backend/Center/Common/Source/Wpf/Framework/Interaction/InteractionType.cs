// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    /// <summary>
    /// Defines the possible interaction types.
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// A positive answer from the user.
        /// </summary>
        Yes,

        /// <summary>
        /// A negative answer from the user.
        /// </summary>
        No,

        /// <summary>
        /// The user accepted the content of the dialog.
        /// </summary>
        Ok,

        /// <summary>
        /// The user canceled the dialog.
        /// </summary>
        Cancel
    }
}