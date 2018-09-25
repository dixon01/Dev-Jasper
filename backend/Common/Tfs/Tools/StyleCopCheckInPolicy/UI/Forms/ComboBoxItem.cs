//--------------------------------------------------------------------------
// <copyright file="ComboBoxItem.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms
{
    /// <summary>
    /// Represents an item within a <see cref="System.Windows.Forms.ComboBox"/> control. This class cannot be inherited.
    /// </summary>
    internal sealed class ComboBoxItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxItem"/> class.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="tag">The tag of the item.</param>
        public ComboBoxItem(string text, object tag)
        {
            this.Text = text;
            this.Tag = tag;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString()
        {
            return this.Text;
        }

        #endregion
    }
}