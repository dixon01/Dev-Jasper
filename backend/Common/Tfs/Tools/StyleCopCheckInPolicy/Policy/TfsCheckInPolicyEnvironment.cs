//--------------------------------------------------------------------------
// <copyright file="TfsCheckInPolicyEnvironment.cs" company="Jeff Winn">
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
    using System.IO;
    using System.Xml;

    using StyleCop;

    /// <summary>
    /// Provides the StyleCop environment to interact with source documents and settings. This class cannot be inherited.
    /// </summary>
    internal sealed class TfsCheckInPolicyEnvironment : FileBasedEnvironment
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsCheckInPolicyEnvironment"/> class.
        /// </summary>
        public TfsCheckInPolicyEnvironment()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the environment supports reading and writing violation results caches.
        /// </summary>
        public override bool SupportsResultsCache
        {
            get { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the settings for the given project.
        /// </summary>
        /// <param name="project">The project containing the settings.</param>
        /// <param name="merge"><c>true</c> whether to merge the settings with the parent settings before returning; otherwise, <c>false</c>.</param>
        /// <param name="exception">Upon return, contains an exception, if one occurred.</param>
        /// <returns>The resulting settings.</returns>
        public override Settings GetProjectSettings(CodeProject project, bool merge, out Exception exception)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            Settings retval = null;
            exception = null;

            if (!string.IsNullOrEmpty(project.Location))
            {
                // Attempt to find the default file in the current folder.
                string path = Path.Combine(project.Location, Settings.DefaultFileName);
                if (!File.Exists(path))
                {
                    // Attempt to find the alternate file in the current folder.
                    path = Path.Combine(project.Location, Settings.AlternateFileName);

                    if (!File.Exists(path))
                    {
                        // Neither files existed in the current folder, begin searching the parent folders.
                        path = this.GetParentSettingsPath(project.Location);
                    }
                }

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    // The file has been found, attempt to load the settings contained in the file.
                    retval = this.GetSettings(path, merge, out exception);
                }
            }

            return retval;
        }

        /// <summary>
        /// Saves the analysis results at the given location.
        /// </summary>
        /// <param name="location">The path at which to save the results.</param>
        /// <param name="analysisResults">The results to save.</param>
        /// <param name="exception">Upon return, contains an exception if an exception occurs while saving the results.</param>
        /// <returns><c>true</c> if the results were saved successfully; otherwise, <c>false</c>.</returns>
        public override bool SaveAnalysisResults(string location, XmlDocument analysisResults, out Exception exception)
        {
            exception = null;
            return true;
        }

        /// <summary>
        /// Saves the settings document at the path specified within the document.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        /// <param name="exception">Upon return, contains an exception if an exception occurs while saving the settings.</param>
        /// <returns><c>true</c> if the settings were saved successfully; otherwise, <c>false</c>.</returns>
        public override bool SaveSettings(WritableSettings settings, out Exception exception)
        {
            exception = null;
            return true;
        }

        #endregion
    }
}