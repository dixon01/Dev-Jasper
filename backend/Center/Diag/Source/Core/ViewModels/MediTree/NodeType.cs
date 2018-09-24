// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NodeType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.MediTree
{
    /// <summary>
    /// The type of Medi tree node.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// The node doesn't have properties nor a table.
        /// </summary>
        Plain,

        /// <summary>
        /// The node has properties.
        /// </summary>
        Object,

        /// <summary>
        /// The node has a table.
        /// </summary>
        Table
    }
}