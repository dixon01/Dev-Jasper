//--------------------------------------------------------------------------
// <copyright file="PathPolicyExclusion.cs" company="Jeff Winn">
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
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides the base implementation for path based exclusions. This class must be inherited.
    /// </summary>
    internal abstract class PathPolicyExclusion : PolicyExclusion
    {
        #region Fields

        /// <summary>
        /// Defines the name of the path property.
        /// </summary>
        public const string PathProperty = "path";

        /// <summary>
        /// Defines the name of the exclusion type property.
        /// </summary>
        public const string ExclusionTypeProperty = "exclusionType";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathPolicyExclusion"/> class.
        /// </summary>
        protected PathPolicyExclusion()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path to exclude.
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exclusion type.
        /// </summary>
        public PathExclusionType ExclusionType
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
                return this.Path;
            }
        }

        /// <summary>
        /// Gets an expression to be used during processing for regex exclusion types.
        /// </summary>
        protected Regex Expression
        {
            get;
            private set;
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

            this.Path = config[PathProperty];
            this.ExclusionType = (PathExclusionType)Enum.Parse(typeof(PathExclusionType), config[ExclusionTypeProperty]);

            if (this.ExclusionType == PathExclusionType.Regex)
            {
                this.Expression = new Regex(this.Path, RegexOptions.None);
            }

            base.Initialize(config);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="exclusion">An <see cref="PolicyExclusion"/> to compare.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public override int CompareTo(PolicyExclusion exclusion)
        {
            PathPolicyExclusion item = (PathPolicyExclusion)exclusion;
            if (item != null)
            {
                return string.Compare(this.Path, item.Path, true, CultureInfo.CurrentCulture);
            }

            return -1;
        }

        #endregion
    }
}