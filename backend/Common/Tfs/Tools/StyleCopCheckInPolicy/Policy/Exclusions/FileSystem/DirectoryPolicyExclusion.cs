//--------------------------------------------------------------------------
// <copyright file="DirectoryPolicyExclusion.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.FileSystem
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a directory policy exclusion. This class cannot be inherited.
    /// </summary>
    internal sealed class DirectoryPolicyExclusion : PathPolicyExclusion
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryPolicyExclusion"/> class.
        /// </summary>
        public DirectoryPolicyExclusion()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the <paramref name="value"/> should be excluded from the policy.
        /// </summary>
        /// <param name="value">An <see cref="System.Object"/> to evaluate.</param>
        /// <returns><b>true</b> if the exclusion is successful; otherwise, <b>false</b>.</returns>
        public override bool Evaluate(object value)
        {
            bool excluded = false;

            if (value != null)
            {
                DirectoryInfo item = (DirectoryInfo)value;

                if (this.ExclusionType == PathExclusionType.Literal)
                {
                    bool found = false;

                    if (item.FullName.EndsWith(this.Path, StringComparison.CurrentCulture))
                    {
                        found = true;
                    }

                    if (found)
                    {
                        excluded = true;
                    }
                    else if (!found && item.Parent != null)
                    {
                        // Recursively check the remainder of the folder path to ensure those aren't in the exclusions.
                        excluded = this.Evaluate(item.Parent);
                    }
                }
                else if (this.ExclusionType == PathExclusionType.Regex && this.Expression != null)
                {
                    excluded = this.Expression.IsMatch(item.FullName);
                }
            }

            return excluded;
        }

        #endregion
    }
}