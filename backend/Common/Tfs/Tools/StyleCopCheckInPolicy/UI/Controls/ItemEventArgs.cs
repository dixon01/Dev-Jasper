//--------------------------------------------------------------------------
// <copyright file="ItemEventArgs.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls
{
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Provides data for events that add a list view item.
    /// </summary>
    internal class ItemEventArgs : CancelEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item that caused the event.</param>       
        public ItemEventArgs(ListViewItem item)
            : this(item, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item being added.</param>
        /// <param name="cancel"><b>true</b> to cancel the event; otherwise, <b>false</b>.</param>
        public ItemEventArgs(ListViewItem item, bool cancel)
            : base(cancel)
        {
            this.Item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item being added.
        /// </summary>
        public ListViewItem Item
        {
            get;
            private set;
        }

        #endregion
    }
}