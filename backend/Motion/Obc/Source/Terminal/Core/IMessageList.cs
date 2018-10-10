// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessageList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The message list main field interface.
    /// </summary>
    public interface IMessageList : IMainField
    {
        /// <summary>
        ///   Enter pressed
        /// </summary>
        event EventHandler<IndexEventArgs> SelectedIndexChanged;

        /// <summary>
        ///   List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        void Init(string caption, List<MessageListEntry> items);

        /// <summary>
        ///   List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        /// <param name="focusIndex">Index of the focused element in the items</param>
        void Init(string caption, List<MessageListEntry> items, int focusIndex);
    }
}