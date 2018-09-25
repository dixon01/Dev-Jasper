//--------------------------------------------------------------------------
// <copyright file="EditorMode.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors
{
    /// <summary>
    /// Defines the editor modes available.
    /// </summary>
    internal enum EditorMode
    {
        /// <summary>
        /// The editor mode has not been set.
        /// </summary>
        Unknown,

        /// <summary>
        /// A new entry is being added.
        /// </summary>
        Add,

        /// <summary>
        /// An existing entry is being edited.
        /// </summary>
        Edit
    }
}