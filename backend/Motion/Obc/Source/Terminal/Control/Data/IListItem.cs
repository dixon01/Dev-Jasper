// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IListItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IListItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Data
{
    using System.Collections.Generic;

    /// <summary>
    ///   Object for Lists(tree) which can be used in the GUI
    /// </summary>
    public interface IListItem
    {
        /// <summary>
        ///   Gets or sets the parent element
        /// </summary>
        IListItem Parent { get; set; }

        /// <summary>
        ///   Gets or sets the order number. Used for sorting the list
        /// </summary>
        int OrderNumber { get; set; }

        /// <summary>
        ///   Gets the displayable label of this item.
        /// </summary>
        string Label { get; }

        /// <summary>
        ///   Gets or sets the last selected entry
        /// </summary>
        int LastSelection { get; set; }

        /// <summary>
        /// Gets the number of levels of parents.
        /// </summary>
        int ParentCount { get; }

        /// <summary>
        /// Gets a value indicating whether this element is a leaf. -> no children
        /// </summary>
        bool IsLeaf { get; }

        /// <summary>
        /// Gets a value indicating whether  this element is the top element
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        ///   Gets the caption for this entry. It's normally the name from the parent
        /// </summary>
        string Caption { get; }

        /// <summary>
        ///   Gets the tag value which can be used for storing list specific data.
        /// </summary>
        object Tag { get; }

        /// <summary>
        /// Gets all child elements
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/> of child <see cref="IListItem"/>s.
        /// </returns>
        List<IListItem> GetChildren();

        /// <summary>
        ///   Gets the specific child from an index. The index is normally the "selectedIndex" from the received event.
        /// </summary>
        /// <param name = "childIndex">index start at 0</param>
        /// <returns>The child.</returns>
        IListItem GetChild(int childIndex);
    }
}