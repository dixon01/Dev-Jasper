//--------------------------------------------------------------------------
// <copyright file="PolicyExclusionFactory.cs" company="PNC Mortgage">
//     Copyright (c) PNC Mortgage. All rights reserved.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides factory methods for creating <see cref="PolicyExclusion"/> objects. This class cannot be inherited.
    /// </summary>
    internal sealed class PolicyExclusionFactory
    {
        #region Fields

        /// <summary>
        /// Contains the root synchronization object.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Contains a value indicating whether the factory has been initialized.
        /// </summary>
        private static bool initialized;

        /// <summary>
        /// Contains the exclusion type cache.
        /// </summary>
        private static Dictionary<PolicyExclusionType, Type> exclusionTypeCache;

        /// <summary>
        /// Contains the factory instance.
        /// </summary>
        private static PolicyExclusionFactory instance;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PolicyExclusionFactory"/> class from being created.
        /// </summary>
        private PolicyExclusionFactory()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        public static PolicyExclusionFactory Instance
        {
            get
            {
                lock (SyncRoot)
                {
                    if (!initialized)
                    {
                        Initialize();
                    }

                    return instance;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new policy exclusion.
        /// </summary>
        /// <param name="config">The <see cref="PolicyExclusionConfigInfo"/> containing exclusion configuration information.</param>
        /// <returns>A new <see cref="PolicyExclusion"/> object if the exclusion type was available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public PolicyExclusion Create(PolicyExclusionConfigInfo config)
        {
            PolicyExclusion exclusion = null;

            Type exclusionType = exclusionTypeCache[config.ExclusionType];
            if (exclusionType != null)
            {
                exclusion = (PolicyExclusion)Activator.CreateInstance(exclusionType);
                if (exclusion != null)
                {
                    exclusion.Initialize(config.Configuration);
                }
            }

            return exclusion;
        }

        /// <summary>
        /// Initializes the factory.
        /// </summary>
        private static void Initialize()
        {
            instance = new PolicyExclusionFactory();
            exclusionTypeCache = new Dictionary<PolicyExclusionType, Type>();

            foreach (FieldInfo field in typeof(PolicyExclusionType).GetFields())
            {
                ExclusionAttribute attribute = Utilities.GetExclusionAttributeByField(field);
                if (attribute != null)
                {
                    exclusionTypeCache.Add((PolicyExclusionType)Enum.Parse(typeof(PolicyExclusionType), field.Name), attribute.ExclusionType);
                }
            }

            initialized = true;
        }

        #endregion
    }
}