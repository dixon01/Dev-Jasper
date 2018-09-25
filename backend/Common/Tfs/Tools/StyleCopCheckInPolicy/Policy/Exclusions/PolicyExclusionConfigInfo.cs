//--------------------------------------------------------------------------
// <copyright file="PolicyExclusionConfigInfo.cs" company="PNC Mortgage">
//     Copyright (c) PNC Mortgage. All rights reserved.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Represents configuration information for a policy exclusion.
    /// </summary>
    [Serializable]
    internal class PolicyExclusionConfigInfo : ICloneable
    {
        #region Fields

        /// <summary>
        /// Contains the exclusion configuration.
        /// </summary>
        private NameValueCollection configuration;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyExclusionConfigInfo"/> class.
        /// </summary>
        public PolicyExclusionConfigInfo()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the exclusion type.
        /// </summary>
        public PolicyExclusionType ExclusionType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public NameValueCollection Configuration
        {
            get
            {
                if (this.configuration == null)
                {
                    this.configuration = new NameValueCollection();
                }

                return this.configuration;
            }

            set
            {
                this.configuration = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>The <see cref="NameValueCollection"/> cloned instance.</returns>
        public object Clone()
        {
            return this.Clone(false);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="deep"><b>true</b> if a deep copy should be performed, otherwise <b>false</b> to perform a shallow copy.</param>
        /// <returns>The <see cref="NameValueCollection"/> cloned instance.</returns>
        public object Clone(bool deep)
        {
            PolicyExclusionConfigInfo clone = new PolicyExclusionConfigInfo();

            clone.ExclusionType = this.ExclusionType;
            clone.Configuration = this.Configuration.Clone(deep);

            return clone;
        }

        #endregion
    }
}