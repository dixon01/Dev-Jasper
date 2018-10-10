//--------------------------------------------------------------------------
// <copyright file="BaseUserControl.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls.Design
{
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Provides a base user control.
    /// </summary>
    [ToolboxItem(false)]
    internal partial class BaseUserControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUserControl"/> class.
        /// </summary>
        public BaseUserControl()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}