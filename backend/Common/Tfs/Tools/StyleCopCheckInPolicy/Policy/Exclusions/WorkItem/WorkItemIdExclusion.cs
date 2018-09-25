//--------------------------------------------------------------------------
// <copyright file="WorkItemIdExclusion.cs" company="Jeff Winn">
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

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions.WorkItem
{
    using System;
    using System.Globalization;

    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Represents a work item id exclusion. This class cannot be inherited.
    /// </summary>
    internal sealed class WorkItemIdExclusion : WorkItemExclusion
    {
        #region Fields

        /// <summary>
        /// Defines the name of the work item id property.
        /// </summary>
        public const string WorkItemIdProperty = "workItemId";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemIdExclusion"/> class.
        /// </summary>
        public WorkItemIdExclusion()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the work item id to exclude.
        /// </summary>
        public int WorkItemId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of the policy exclusion.
        /// </summary>
        public override string Description
        {
            get
            {
                return this.WorkItemId.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the policy exclusion.
        /// </summary>
        /// <param name="config">A collection of name/value pairs representing the exclusion specific configuration attributes.</param>
        public override void Initialize(System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            string workItemId = config[WorkItemIdProperty];

            if (!string.IsNullOrEmpty(workItemId))
            {
                this.WorkItemId = int.Parse(workItemId);
            }

            base.Initialize(config);
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> should be excluded from the policy.
        /// </summary>
        /// <param name="value">An <see cref="System.Object"/> to evaluate.</param>
        /// <returns><b>true</b> if the exclusion is successful; otherwise, <b>false</b>.</returns>
        public override bool Evaluate(object value)
        {
            bool retval = false;

            if (value != null)
            {
                WorkItemCheckinInfo item = (WorkItemCheckinInfo)value;
                retval = item.WorkItem != null && item.WorkItem.Id == this.WorkItemId;
            }

            return retval;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="exclusion">An <see cref="PolicyExclusion"/> to compare.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public override int CompareTo(PolicyExclusion exclusion)
        {
            WorkItemIdExclusion item = (WorkItemIdExclusion)exclusion;
            if (item != null)
            {
                return this.WorkItemId.CompareTo(item.WorkItemId);
            }

            return -1;
        }

        #endregion
    }
}