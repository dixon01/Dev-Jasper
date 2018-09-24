// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A general list main field.
    /// </summary>
    public interface IList : IMainField
    {
        /// <summary>
        /// The selected index changed event.
        /// </summary>
        event EventHandler<IndexEventArgs> SelectedIndexChanged;

        /// <summary>
        /// List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        /// <param name = "allowEscape">
        /// Allows to use go back from the list without choose an item.
        /// Set this to false if you want that a user has join an item
        /// </param>
        void Init(string caption, List<string> items, bool allowEscape);

        /// <summary>
        /// List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        /// <param name = "allowEscape">
        /// Allows to use go back from the list without choose an item.
        /// Set this to false if you want that a user has join an item
        /// </param>
        /// <param name="focusIndex">
        /// Index of the focused item in the list.
        /// </param>
        void Init(string caption, List<string> items, bool allowEscape, int focusIndex);
    }
}