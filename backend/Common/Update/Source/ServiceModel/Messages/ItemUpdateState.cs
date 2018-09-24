// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemUpdateState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemUpdateState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    /// <summary>
    /// The update state of a folder or file.
    /// </summary>
    public enum ItemUpdateState
    {
        /// <summary>
        /// The item was completely updated (or was already up-to-date).
        /// </summary>
        UpToDate,

        /// <summary>
        /// The item was not updated.
        /// </summary>
        NotUpdated,

        /// <summary>
        /// The item was not deleted during the update.
        /// </summary>
        NotDeleted,

        /// <summary>
        /// The item was not created during the update.
        /// </summary>
        NotCreated,

        /// <summary>
        /// The item was partially updated.
        /// This state is only valid for folders.
        /// </summary>
        PartiallyUpdated,
    }
}