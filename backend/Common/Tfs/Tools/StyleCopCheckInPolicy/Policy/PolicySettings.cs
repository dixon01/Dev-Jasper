//--------------------------------------------------------------------------
// <copyright file="PolicySettings.cs" company="Jeff Winn">
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
    using System.Collections.ObjectModel;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions;

    using StyleCop;

    /// <summary>
    /// Represents configuration settings for the <see cref="SourceAnalysisPolicy"/> policy. This class cannot be inherited.
    /// </summary>
    [Serializable]
    internal sealed class PolicySettings : ICloneable
    {
        #region Fields

        /// <summary>
        /// Contains the collection of exclusion configurations.
        /// </summary>
        private Collection<PolicyExclusionConfigInfo> exclusions;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PolicySettings"/> class from being created.
        /// </summary>
        private PolicySettings()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the StyleCop settings for the policy.
        /// </summary>
        public string StyleCopSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether project settings will override policy.
        /// </summary>
        public bool AllowProjectToOverridePolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the task category.
        /// </summary>
        public PolicyTaskCategory TaskCategory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets when the evaluation should occur.
        /// </summary>
        public EvaluateOnType EvaluateOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of exclusions and their configurations.
        /// </summary>
        public Collection<PolicyExclusionConfigInfo> Exclusions
        {
            get
            {
                if (this.exclusions == null)
                {
                    this.exclusions = new Collection<PolicyExclusionConfigInfo>();
                }

                return this.exclusions;
            }

            set
            {
                this.exclusions = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="PolicySettings"/> instance.
        /// </summary>
        /// <param name="useDefaultSettings"><c>true</c> if the default settings should be used; otherwise, <c>false</c>.</param>
        /// <returns>A new <see cref="PolicySettings"/> object.</returns>
        public static PolicySettings Create(bool useDefaultSettings)
        {
            PolicySettings settings = new PolicySettings();

            if (useDefaultSettings)
            {
                // Set the default values for the new instance.
                settings.TaskCategory = PolicyTaskCategory.Error;
                settings.EvaluateOn = EvaluateOnType.Add | EvaluateOnType.Edit | EvaluateOnType.Rename | EvaluateOnType.Merge;
                settings.StyleCopSettings = WritableSettings.NewDocument().OuterXml;
            }

            return settings;
        }

        /// <summary>
        /// Clones a copy of the object.
        /// </summary>
        /// <returns>A new <see cref="PolicySettings"/> instance.</returns>
        public object Clone()
        {
            return this.Clone(false);
        }

        /// <summary>
        /// Clones a copy of the object.
        /// </summary>
        /// <param name="deep"><b>true</b> if a deep clone should be performed, otherwise <b>false</b>.</param>
        /// <returns>A new <see cref="PolicySettings"/> instance.</returns>
        public object Clone(bool deep)
        {
            PolicySettings clone = new PolicySettings();

            clone.AllowProjectToOverridePolicy = this.AllowProjectToOverridePolicy;
            clone.EvaluateOn = this.EvaluateOn;
            clone.TaskCategory = this.TaskCategory;
            clone.StyleCopSettings = deep ? (string)this.StyleCopSettings.Clone() : this.StyleCopSettings;

            if (this.Exclusions != null)
            {
                if (!deep)
                {
                    clone.Exclusions = this.Exclusions;
                }
                else
                {
                    foreach (PolicyExclusionConfigInfo item in this.Exclusions)
                    {
                        clone.Exclusions.Add((PolicyExclusionConfigInfo)item.Clone(deep));
                    }
                }
            }

            return clone;
        }

        #endregion
    }
}