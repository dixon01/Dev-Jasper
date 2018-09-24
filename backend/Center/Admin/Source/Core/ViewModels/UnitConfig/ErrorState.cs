// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    /// <summary>
    /// The possible error states of a node in the unit configurator navigation tree.
    /// </summary>
    public enum ErrorState
    {
        /// <summary>
        /// The node (and all of its children) is ok.
        /// </summary>
        Ok,

        /// <summary>
        /// The node has one or more warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// The node is missing some information.
        /// </summary>
        Missing,

        /// <summary>
        /// The node has one or more errors.
        /// </summary>
        Error
    }
}