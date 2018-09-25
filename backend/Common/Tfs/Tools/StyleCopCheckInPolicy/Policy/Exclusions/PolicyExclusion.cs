//--------------------------------------------------------------------------
// <copyright file="PolicyExclusion.cs" company="Jeff Winn">
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
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Provides the base implementation for policy exclusions. This class must be inherited.
    /// </summary>
    internal abstract class PolicyExclusion
    {
        #region Fields

        /// <summary>
        /// Defines the name of the enabled property.
        /// </summary>
        public const string EnabledProperty = "enabled";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyExclusion"/> class.
        /// </summary>
        protected PolicyExclusion()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the policy exclusion is active.
        /// </summary>
        public bool Enabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of the policy exclusion.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the policy exclusion.
        /// </summary>
        /// <param name="config">A collection of name/value pairs representing the exclusion specific configuration attributes.</param>
        public virtual void Initialize(NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            string enabled = config[EnabledProperty];

            if (!string.IsNullOrEmpty(enabled))
            {
                this.Enabled = bool.Parse(enabled);
            }
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> should be excluded from the policy.
        /// </summary>
        /// <param name="value">An <see cref="System.Object"/> to evaluate.</param>
        /// <returns><b>true</b> if the exclusion is successful; otherwise, <b>false</b>.</returns>
        public abstract bool Evaluate(object value);

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="exclusion">An <see cref="PolicyExclusion"/> to compare.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public abstract int CompareTo(PolicyExclusion exclusion);

        #endregion
    }
}