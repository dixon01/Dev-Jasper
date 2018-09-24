// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    /// <summary>
    /// The type of action to be performed for a file or folder in the file system
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Action indication that a file or folder must be created
        /// </summary>
        Create,

        /// <summary>
        /// Action indication a file or folder must be deleted
        /// </summary>
        Delete,

        /// <summary>
        /// Action indicating a file or folder must be updated
        /// </summary>
        Update,
    }
}
