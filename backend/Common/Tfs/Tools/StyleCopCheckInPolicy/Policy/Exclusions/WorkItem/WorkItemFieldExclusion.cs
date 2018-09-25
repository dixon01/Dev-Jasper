//--------------------------------------------------------------------------
// <copyright file="WorkItemFieldExclusion.cs" company="Jeff Winn">
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

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;

    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Represents a work item field exclusion. This class cannot be inherited.
    /// </summary>
    internal sealed class WorkItemFieldExclusion : WorkItemExclusion
    {
        #region Fields

        /// <summary>
        /// Defines the name of the field name property.
        /// </summary>
        public const string FieldNameProperty = "fieldName";

        /// <summary>
        /// Defines the name of the field value property.
        /// </summary>
        public const string FieldValueProperty = "fieldValue";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemFieldExclusion"/> class.
        /// </summary>
        public WorkItemFieldExclusion()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the description of the policy exclusion.
        /// </summary>
        public override string Description
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, Resources.WorkItemFieldExclusionValueDesc, this.FieldName, this.FieldValue);
            }
        }

        /// <summary>
        /// Gets the field name to locate.
        /// </summary>
        public string FieldName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the field value to check.
        /// </summary>
        public string FieldValue
        {
            get;
            private set;
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
            bool retval = false;

            if (value != null && !string.IsNullOrEmpty(this.FieldName))
            {
                WorkItemCheckinInfo item = (WorkItemCheckinInfo)value;
                if (item != null && item.WorkItem.Fields.Contains(this.FieldName))
                {
                    Field field = item.WorkItem.Fields[this.FieldName];
                    if (field != null && field.IsValid)
                    {
                        retval = this.FieldValue == field.Value.ToString();
                    }
                }
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
            int retval = 0;

            WorkItemFieldExclusion item = (WorkItemFieldExclusion)exclusion;
            if (item != null && item.FieldName != null)
            {
                retval = string.Compare(this.FieldName, item.FieldName, StringComparison.CurrentCulture);
                if (retval == 0)
                {
                    retval = string.Compare(this.FieldValue, item.FieldValue, StringComparison.CurrentCulture);
                }
            }

            return retval;
        }

        #endregion
    }
}