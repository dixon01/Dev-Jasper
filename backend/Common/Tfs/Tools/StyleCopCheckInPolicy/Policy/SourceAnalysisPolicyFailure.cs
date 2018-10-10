//--------------------------------------------------------------------------
// <copyright file="SourceAnalysisPolicyFailure.cs" company="Jeff Winn">
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
    using System.Collections.ObjectModel;

    using Microsoft.TeamFoundation.VersionControl.Client;

    using StyleCop;

    /// <summary>
    /// Represents a failure for the source analysis policy. This class cannot be inherited.
    /// </summary>
    internal sealed class SourceAnalysisPolicyFailure : PolicyFailure
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceAnalysisPolicyFailure"/> class.
        /// </summary>
        /// <param name="message">The message of the policy failure.</param>
        /// <param name="policy">The <see cref="IPolicyEvaluation"/> where the failure occurred.</param>
        /// <param name="violations">An array of <see cref="Microsoft.StyleCop.Violation"/> instances that occurred while analyzing the source file.</param>
        public SourceAnalysisPolicyFailure(string message, IPolicyEvaluation policy, Collection<Violation> violations)
            : base(message, policy)
        {
            this.Violations = violations;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the violations associated with the policy failure.
        /// </summary>
        public Collection<Violation> Violations
        {
            get;
            private set;
        }

        #endregion
    }
}