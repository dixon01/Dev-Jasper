// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Placement.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Placement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Core
{
    /// <summary>
    /// Placement to place a new layer at the right place into the layers stack.
    /// </summary>
    public enum Placement
    {
        /// <summary>
        /// To place a layer on top of the layers stack.
        /// </summary>
        Top,

        /// <summary>
        /// To place a layer above another one.
        /// </summary>
        Above,

        /// <summary>
        /// To place a layer below another one.
        /// </summary>
        Below
    }
}
