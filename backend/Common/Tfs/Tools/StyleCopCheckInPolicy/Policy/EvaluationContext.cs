//--------------------------------------------------------------------------
// <copyright file="EvaluationContext.cs" company="Jeff Winn">
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
    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Provides contextual information for a source analysis evaluation process. This class cannot be inherited.
    /// </summary>
    internal sealed class EvaluationContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationContext"/> class.
        /// </summary>
        /// <param name="policy">The policy instance.</param>
        /// <param name="settings">The policy settings.</param>
        /// <param name="pendingCheckin">The pending check-in whose files to analyze.</param>
        public EvaluationContext(IPolicyEvaluation policy, PolicySettings settings, IPendingCheckin pendingCheckin)
        {
            this.Policy = policy;
            this.Settings = settings;
            this.PendingCheckin = pendingCheckin;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the policy instance.
        /// </summary>
        public IPolicyEvaluation Policy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the policy settings.
        /// </summary>
        public PolicySettings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pending check-in whose files to analyze.
        /// </summary>
        public IPendingCheckin PendingCheckin
        {
            get;
            private set;
        }

        #endregion
    }
}