//--------------------------------------------------------------------------
// <copyright file="EnableButtonsEventArgs.cs" company="Jeff Winn">
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
    using System.Windows.Forms;

    /// <summary>
    /// Provides data for the <see cref="ExclusionManagerControl.SelectedIndexChanged"/> event.
    /// </summary>
    internal class EnableButtonsEventArgs : ItemEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableButtonsEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item that caused the event.</param>       
        public EnableButtonsEventArgs(ListViewItem item)
            : base(item)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the add button will be enabled.
        /// </summary>
        public bool AddButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the edit button will be enabled.
        /// </summary>
        public bool EditButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the remove button will be enabled.
        /// </summary>
        public bool RemoveButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the enable button will be enabled.
        /// </summary>
        public bool EnableButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the disable button will be enabled.
        /// </summary>
        public bool DisableButton
        {
            get;
            set;
        }

        #endregion
    }
}