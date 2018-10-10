// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StyleCopCheckInPolicy.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms;

    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Provides a check-in policy for Microsoft Team Foundation Server to check for source analysis violations
    /// with Microsoft StyleCop. This class cannot be inherited.
    /// This policy is based on Jeff Winn's Source Analysis Policy (http://sourceanalysispolicy.codeplex.com/).
    /// It has been adapted:
    ///  - to work with Visual Studio 2010, 2012 and 2013.
    ///  - to not add errors to the Visual Studio error list anymore.
    /// </summary>
    [Serializable]
    public sealed class StyleCopCheckInPolicy : PolicyBase
    {
        #region Fields

        /// <summary>
        /// Contains the settings for the policy.
        /// </summary>
        private PolicySettings settings;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the policy can be edited.
        /// </summary>
        public override bool CanEdit
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the description of the policy.
        /// </summary>
        public override string Description
        {
            get { return Resources.Message_PolicyDescription; }
        }

        /// <summary>
        /// Gets the type of policy.
        /// </summary>
        public override string Type
        {
            get { return Resources.Message_PolicyType; }
        }

        /// <summary>
        /// Gets the policy type description.
        /// </summary>
        public override string TypeDescription
        {
            get { return Resources.Message_PolicyTypeDescription; }
        }

        /// <summary>
        /// Gets or sets the policy settings.
        /// </summary>
        private PolicySettings Settings
        {
            get
            {
                if (this.settings == null)
                {
                    this.settings = PolicySettings.Create(true);
                }

                return this.settings;
            }

            set
            {
                this.settings = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Activates a policy failure.
        /// </summary>
        /// <param name="failure">The failure to activate.</param>
        public override void Activate(PolicyFailure failure)
        {
            var policyFailure = failure as SourceAnalysisPolicyFailure;
            if (policyFailure != null)
            {
                using (var dialog = new DisplayViolationsDialog())
                {
                    dialog.Violations = policyFailure.Violations;

                    dialog.ShowDialog();
                }
            }

            base.Activate(failure);
        }

        /// <summary>
        /// Edits the policy.
        /// </summary>
        /// <param name="policyEditArgs">An <see cref="IPolicyEditArgs"/> containing policy edit arguments.</param>
        /// <returns><b>true</b> if the policy has been edited successfully, otherwise <b>false</b>.</returns>
        public override bool Edit(IPolicyEditArgs policyEditArgs)
        {
            if (policyEditArgs == null)
            {
                throw new ArgumentNullException("policyEditArgs");
            }

            bool retval = false;

            using (EditPolicyDialog dialog = new EditPolicyDialog())
            {
                // Clone the settings to prevent modifying the settings currently in use.
                dialog.Settings = (PolicySettings)this.Settings.Clone();

                if (dialog.ShowDialog(policyEditArgs.Parent) == DialogResult.OK)
                {
                    this.Settings = dialog.Settings;
                    retval = true;
                }
            }

            return retval;
        }

        /// <summary>
        /// Evaluates the policy.
        /// </summary>
        /// <returns>The policy failures, if any, that occurred.</returns>
        public override PolicyFailure[] Evaluate()
        {
            PolicyFailure[] failures = null;

            using (EvaluationProcess process = new EvaluationProcess())
            {
                process.Initialize(new EvaluationContext(this, this.Settings, this.PendingCheckin));
                failures = process.Analyze();
            }

            return failures;
        }

        /// <summary>
        /// Disposes of the policy.
        /// </summary>
        public sealed override void Dispose()
        {
            this.settings = null;

            base.Dispose();
        }

        #endregion
    }
}
