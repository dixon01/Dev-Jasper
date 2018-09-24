//--------------------------------------------------------------------------
// <copyright file="EvaluateOnType.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy
{
    using System;

    /// <summary>
    /// Defines when the evaluation should occur.
    /// </summary>
    [Flags]
    internal enum EvaluateOnType
    {
        /// <summary>
        /// The evaluation type(s) have not been set.
        /// </summary>
        NotSet = 0x00,

        /// <summary>
        /// No evaluations will be made.
        /// </summary>
        None = 0x01,

        /// <summary>
        /// Evaluate on add.
        /// </summary>
        Add = 0x02,

        /// <summary>
        /// Evaluate on edit.
        /// </summary>
        Edit = 0x04,

        /// <summary>
        /// Evaluate on rename.
        /// </summary>
        Rename = 0x08,

        /// <summary>
        /// Evaluate on branch.
        /// </summary>
        Branch = 0x10,

        /// <summary>
        /// Evaluate on merge.
        /// </summary>
        Merge = 0x20
    }
}