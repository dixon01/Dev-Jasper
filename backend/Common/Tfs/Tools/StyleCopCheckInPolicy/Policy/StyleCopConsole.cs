//--------------------------------------------------------------------------
// <copyright file="StyleCopConsole.cs" company="Jeff Winn">
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
    using System.Collections.Generic;

    using StyleCop;

    /// <summary>
    /// Provides a wrapper for hosting StyleCop within the policy. This class cannot be inherited.
    /// </summary>
    internal sealed class StyleCopConsole : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopConsole"/> class.
        /// </summary>
        /// <param name="policySettingsPath">The path to the policy settings.</param>
        /// <param name="settings">The policy settings.</param>
        public StyleCopConsole(string policySettingsPath, PolicySettings settings)
        {
            this.PolicySettingsPath = policySettingsPath;
            this.Settings = settings;

            StyleCopCore core = new StyleCopCore(new TfsCheckInPolicyEnvironment(), null);
            core.ViolationEncountered += new EventHandler<ViolationEventArgs>(this.VoilationEncounteredCallback);
            core.Initialize(null, true);
            core.WriteResultsCache = false;
            core.DisplayUI = false;

            this.Core = core;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StyleCopConsole"/> class.
        /// </summary>
        ~StyleCopConsole()
        {
            this.Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a violation is encountered.
        /// </summary>
        public event EventHandler<ViolationEventArgs> ViolationEncountered;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the core instance.
        /// </summary>
        public StyleCopCore Core
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the path to the file containing the policy StyleCop settings.
        /// </summary>
        private string PolicySettingsPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy settings.
        /// </summary>
        private PolicySettings Settings
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Analyzes the source code documents contained in the <paramref name="projects"/> specified.
        /// </summary>
        /// <param name="projects">The projects to analyze.</param>
        public void Analyze(IList<CodeProject> projects)
        {
            if (projects == null)
            {
                throw new ArgumentNullException("projects");
            }

            this.LoadProjectSettings(projects);

            this.Core.FullAnalyze(projects);
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"><b>true</b> to release managed resources along with unmanaged resources; otherwise, <b>false</b>.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Core.ViolationEncountered -= this.VoilationEncounteredCallback;
            }
        }

        /// <summary>
        /// Loads the settings for the <paramref name="projects"/> specified.
        /// </summary>
        /// <param name="projects">The projects whose settings to load.</param>
        private void LoadProjectSettings(IList<CodeProject> projects)
        {
            Settings policySettings = null;
            if (!string.IsNullOrEmpty(this.PolicySettingsPath))
            {
                Settings localSettings = this.Core.Environment.GetSettings(this.PolicySettingsPath, false);
                if (localSettings != null)
                {
                    SettingsMerger merger = new SettingsMerger(localSettings, this.Core.Environment);
                    policySettings = merger.MergedSettings;
                }
            }

            foreach (CodeProject project in projects)
            {
                Settings projectSettings = policySettings;
                if (this.Settings.AllowProjectToOverridePolicy)
                {
                    projectSettings = this.Core.Environment.GetProjectSettings(project, true);
                }

                if (projectSettings != null)
                {
                    project.Settings = projectSettings;
                    project.SettingsLoaded = true;
                }
            }
        }

        /// <summary>
        /// Callback when the <see cref="StyleCopCore"/> encounters a violation.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ViolationEventArgs"/> containing event data.</param>
        private void VoilationEncounteredCallback(object sender, ViolationEventArgs e)
        {
            this.OnViolationEncountered(e);
        }

        /// <summary>
        /// Raises the <see cref="ViolationEncountered"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ViolationEventArgs"/> containing event data.</param>
        private void OnViolationEncountered(ViolationEventArgs e)
        {
            if (this.ViolationEncountered != null)
            {
                this.ViolationEncountered(this, e);
            }
        }

        #endregion
    }
}