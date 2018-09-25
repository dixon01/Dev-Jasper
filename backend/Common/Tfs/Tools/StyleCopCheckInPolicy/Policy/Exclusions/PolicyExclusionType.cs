//--------------------------------------------------------------------------
// <copyright file="PolicyExclusionType.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions
{
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.FileSystem;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.WorkItem;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors;

    /// <summary>
    /// Defines the policy exclusions available.
    /// </summary>
    internal enum PolicyExclusionType
    {
        /// <summary>
        /// No policy exclusion.
        /// </summary>
        None,

        /// <summary>
        /// The check-in is excluded because of the directory.
        /// </summary>
        [Exclusion(typeof(DirectoryPolicyExclusion),
            "DirectoryPolicyExclusionName",
            "DirectoryPolicyExclusionDesc",
            typeof(DirectoryEditorDialog))]
        Directory,

        /// <summary>
        /// The check-in is excluded because of the name of the file.
        /// </summary>
        [Exclusion(typeof(FilePolicyExclusion),
            "FilePolicyExclusionName",
            "FilePolicyExclusionDesc",
            typeof(FileEditorDialog))]
        FileName,

        /// <summary>
        /// The check-in is excluded because of an associated work item with a matching work item id.
        /// </summary>
        [Exclusion(typeof(WorkItemIdExclusion), 
            "WorkItemIdExclusionName", 
            "WorkItemIdExclusionDesc", 
            typeof(WorkItemIdEditorDialog))]
        WorkItemId,

        /// <summary>
        /// The check-in is excluded because of an associated work item whose field has been set.
        /// </summary>
        [Exclusion(typeof(WorkItemFieldExclusion), 
            "WorkItemFieldExclusionName", 
            "WorkItemFieldExclusionDesc", 
            typeof(WorkItemFieldEditorDialog))]
        WorkItemField
    }
}