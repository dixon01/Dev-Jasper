//--------------------------------------------------------------------------
// <copyright file="EvaluationProcess.cs" company="Jeff Winn">
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
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;

    using Microsoft.TeamFoundation.VersionControl.Client;

    using StyleCop;

    /// <summary>
    /// Represents the evaluation process for analyzing source files. This class cannot be inherited.
    /// </summary>
    internal sealed class EvaluationProcess : IDisposable
    {
        #region Fields

        /// <summary>
        /// Contains the cache of violations that have occurred during analysis.
        /// </summary>
        private Dictionary<string, Collection<Violation>> cache;

        /// <summary>
        /// Contains the <see cref="StyleCopConsole"/> used to perform the analysis on the source files.
        /// </summary>
        private StyleCopConsole console;

        /// <summary>
        /// Contains the path to the temporary file storing the policy StyleCop settings.
        /// </summary>
        private string settingsFilePath;

        /// <summary>
        /// Contains the collection of exclusions.
        /// </summary>
        private Dictionary<PolicyExclusionType, Collection<PolicyExclusion>> exclusions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationProcess"/> class.
        /// </summary>
        public EvaluationProcess()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="EvaluationProcess"/> class.
        /// </summary>
        ~EvaluationProcess()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the evaluation context.
        /// </summary>
        private EvaluationContext Context
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the analysis process.
        /// </summary>
        /// <param name="context">An <see cref="Context"/> containing contextual information for the analysis process.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        public void Initialize(EvaluationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.cache = new Dictionary<string, Collection<Violation>>();

            if (!string.IsNullOrEmpty(context.Settings.StyleCopSettings))
            {
                // The policy has its own settings, write them to a temp file so they can be used.
                this.settingsFilePath = Path.GetTempFileName();
                File.WriteAllText(this.settingsFilePath, context.Settings.StyleCopSettings);
            }

            this.exclusions = new Dictionary<PolicyExclusionType, Collection<PolicyExclusion>>();

            if (context.Settings.Exclusions != null && context.Settings.Exclusions.Count > 0)
            {
                foreach (PolicyExclusionConfigInfo exclusionConfig in context.Settings.Exclusions)
                {
                    Collection<PolicyExclusion> tempCollection = null;

                    if (this.exclusions.ContainsKey(exclusionConfig.ExclusionType))
                    {
                        tempCollection = this.exclusions[exclusionConfig.ExclusionType];
                    }
                    else
                    {
                        tempCollection = new Collection<PolicyExclusion>();
                        this.exclusions.Add(exclusionConfig.ExclusionType, tempCollection);
                    }

                    PolicyExclusion exclusion = PolicyExclusionFactory.Instance.Create(exclusionConfig);
                    if (exclusion != null)
                    {
                        tempCollection.Add(exclusion);
                    }
                }
            }

            this.console = new StyleCopConsole(this.settingsFilePath, context.Settings);
            this.console.ViolationEncountered += new EventHandler<ViolationEventArgs>(this.ViolationEncounteredCallback);
            this.Context = context;
        }

        /// <summary>
        /// Performs the analysis.
        /// </summary>
        /// <returns>The policy failures, if any, that occurred.</returns>
        public PolicyFailure[] Analyze()
        {
            PolicyFailure[] retval = null;

            if (!this.IsAnySelectedWorkItemExcluded())
            {
                IList<CodeProject> projects = this.GetProjectsToAnalyze();
                if (projects != null && projects.Count > 0)
                {                 
                    this.console.Analyze(projects);

                    List<PolicyFailure> failures = new List<PolicyFailure>();

                    if (this.cache != null && this.cache.Count > 0)
                    {
                        foreach (KeyValuePair<string, Collection<Violation>> pair in this.cache)
                        {
                            failures.Add(this.BuildPolicyFailure(pair));
                        }
                    }

                    retval = failures.ToArray();
                }
            }

            return retval;
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
        /// Evaluates the collection of exclusions.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <param name="exclusions">The collection of exclusions to test for the item.</param>
        /// <returns><b>true</b> if the item is excluded; otherwise, <b>false</b>.</returns>
        private static bool EvaluateExclusions(object item, Collection<PolicyExclusion> exclusions)
        {
            bool excluded = false;

            if (item != null && exclusions != null && exclusions.Count > 0)
            {
                foreach (PolicyExclusion exclusion in exclusions)
                {
                    if (!exclusion.Enabled)
                    {
                        continue;
                    }

                    if (exclusion.Evaluate(item))
                    {
                        excluded = true;
                        break;
                    }
                }
            }

            return excluded;
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"><b>true</b> to release managed resources along with unmanaged resources; otherwise, <b>false</b>.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(this.settingsFilePath) && File.Exists(this.settingsFilePath))
                {
                    // Delete the temporary file storing the policy StyleCop settings.
                    File.Delete(this.settingsFilePath);
                    this.settingsFilePath = null;
                }

                if (this.console != null)
                {
                    this.console.Dispose();
                }

                this.cache = null;
            }
        }

        /// <summary>
        /// Occurs when a source analysis violation is encountered during the analysis.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that sent the event.</param>
        /// <param name="e">An <see cref="ViolationEventArgs"/> containing event data.</param>
        private void ViolationEncounteredCallback(object sender, ViolationEventArgs e)
        {
            Collection<Violation> violations = null;

            var path = e.Violation.SourceCode == null ? string.Empty : e.Violation.SourceCode.Path;

            if (!this.cache.ContainsKey(path))
            {
                violations = new Collection<Violation>();
                this.cache.Add(path, violations);
            }
            else
            {
                violations = this.cache[path];
            }

            violations.Add(e.Violation);
        }

        /// <summary>
        /// Returns a collection of files from the current pending check-in which need to be analyzed.
        /// </summary>
        /// <returns>The collection of files to analyze.</returns>
        private Collection<FileInfo> GetFilesToAnalyze()
        {
            Collection<FileInfo> files = new Collection<FileInfo>();

            // Only have StyleCop check the changes which are trying to be checked in, do not check everything the user has checked out.
            if (this.Context.PendingCheckin.PendingChanges != null && this.Context.PendingCheckin.PendingChanges.CheckedPendingChanges != null && this.Context.PendingCheckin.PendingChanges.CheckedPendingChanges.Length > 0)
            {
                foreach (PendingChange pendingChange in this.Context.PendingCheckin.PendingChanges.CheckedPendingChanges)
                {
                    if (pendingChange.ItemType != ItemType.File || string.Compare(Path.GetExtension(pendingChange.FileName), ".cs", true, CultureInfo.CurrentCulture) != 0 || this.IsChangeIgnored(pendingChange))
                    {
                        // The change is not the proper type of change which will require source analysis, skip it.
                        continue;
                    }

                    FileInfo file = new FileInfo(pendingChange.LocalItem);
                    if (file != null && file.Exists)
                    {
                        if (this.IsFolderExcluded(file.Directory) || this.IsFileExcluded(file))
                        {
                            // The file has been excluded from the policy, skip it.
                            continue;
                        }

                        files.Add(file);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Generates a file map for all files that need to be analyzed.
        /// </summary>
        /// <returns>A dictionary of files mapped to the project they should belong to.</returns>
        private Dictionary<string, Collection<FileInfo>> GenerateProjectFileMap()
        {
            Dictionary<string, Collection<FileInfo>> fileMap = new Dictionary<string, Collection<FileInfo>>();

            // Create the projects that will contain the code files to analyze.
            foreach (FileInfo file in this.GetFilesToAnalyze())
            {
                Collection<FileInfo> files = null;

                string path = string.Empty;

                if (this.Context.Settings.AllowProjectToOverridePolicy)
                {
                    string projectSettingsPath = this.console.Core.Environment.GetParentSettingsPath(file.FullName);
                    if (projectSettingsPath != null)
                    {
                        path = projectSettingsPath;
                    }
                }

                if (fileMap.ContainsKey(path))
                {
                    files = fileMap[path];
                }
                else
                {
                    files = new Collection<FileInfo>();
                    fileMap.Add(path, files);
                }

                files.Add(file);
            }

            return fileMap;
        }

        /// <summary>
        /// Returns a collection of <see cref="CodeProject"/> objects containing all projects to analyze.
        /// </summary>
        /// <returns>A collection of <see cref="CodeProject"/> objects.</returns>
        private IList<CodeProject> GetProjectsToAnalyze()
        {
            Collection<CodeProject> projects = new Collection<CodeProject>();
         
            foreach (KeyValuePair<string, Collection<FileInfo>> pair in this.GenerateProjectFileMap())
            {
                var location = File.Exists(pair.Key) ? Path.GetDirectoryName(pair.Key) : "C:\\"; // location must be valid
                CodeProject project = new CodeProject(pair.Key.GetHashCode(), location, new StyleCop.Configuration(new string[0]));

                foreach (FileInfo codeFile in pair.Value)
                {
                    this.console.Core.Environment.AddSourceCode(project, codeFile.FullName, null);
                }

                projects.Add(project);
            }

            return projects;
        }

        /// <summary>
        /// Builds a new policy failure.
        /// </summary>
        /// <param name="pair">The file/policy pair whose failure(s) to build.</param>
        /// <returns>An array of <see cref="PolicyFailure"/> objects.</returns>
        private PolicyFailure BuildPolicyFailure(KeyValuePair<string, Collection<Violation>> pair)
        {
            return new SourceAnalysisPolicyFailure(string.Format(CultureInfo.CurrentCulture, Resources.Message_SourceViolationsFound, pair.Key), this.Context.Policy, pair.Value);
        }

        /// <summary>
        /// Indicates whether a particular exclusion type is active.
        /// </summary>
        /// <param name="exclusionType">The exclusion type to check.</param>
        /// <returns><c>true</c> if the exclusion type is active; otherwise, <c>false</c>.</returns>
        private bool IsExclusionTypeActive(PolicyExclusionType exclusionType)
        {
            return this.exclusions != null && this.exclusions.ContainsKey(exclusionType);
        }

        /// <summary>
        /// Indicates whether any selected work items have been excluded.
        /// </summary>
        /// <returns><b>true</b> if the check-in is excluded from the policy, otherwise <b>false</b>.</returns>
        private bool IsAnySelectedWorkItemExcluded()
        {
            bool excluded = false;

            if (this.IsExclusionTypeActive(PolicyExclusionType.WorkItemId) || this.IsExclusionTypeActive(PolicyExclusionType.WorkItemField))
            {
                // The exclusion is enabled, query the server for any selected work items.
                WorkItemCheckinInfo[] workItems = this.Context.PendingCheckin.WorkItems.CheckedWorkItems;
                if (workItems != null && workItems.Length > 0)
                {
                    foreach (WorkItemCheckinInfo selectedWorkItem in workItems)
                    {
                        if (this.IsWorkItemExcluded(selectedWorkItem))
                        {
                            excluded = true;
                            break;
                        }
                    }
                }
            }

            return excluded;
        }

        /// <summary>
        /// Indicates whether a pending change is ignored.
        /// </summary>
        /// <param name="pendingChange">The pending change to test.</param>
        /// <returns><b>true</b> if the change is ignored, otherwise <b>false</b>.</returns>
        private bool IsChangeIgnored(PendingChange pendingChange)
        {
            return (pendingChange.IsAdd && !Utilities.IsFlagSet(this.Context.Settings.EvaluateOn, EvaluateOnType.Add)) ||
                (pendingChange.IsBranch && !Utilities.IsFlagSet(this.Context.Settings.EvaluateOn, EvaluateOnType.Branch)) ||
                (pendingChange.IsEdit && !Utilities.IsFlagSet(this.Context.Settings.EvaluateOn, EvaluateOnType.Edit)) ||
                (pendingChange.IsMerge && !Utilities.IsFlagSet(this.Context.Settings.EvaluateOn, EvaluateOnType.Merge)) ||
                (pendingChange.IsRename && !Utilities.IsFlagSet(this.Context.Settings.EvaluateOn, EvaluateOnType.Rename));
        }

        /// <summary>
        /// Indicates whether the work item has been excluded from the policy.
        /// </summary>
        /// <param name="selectedWorkItem">The work item to check.</param>
        /// <returns><b>true</b> if the work item is excluded; otherwise, <b>false</b>.</returns>
        private bool IsWorkItemExcluded(WorkItemCheckinInfo selectedWorkItem)
        {
            return this.IsItemExcluded(selectedWorkItem, PolicyExclusionType.WorkItemId) || this.IsItemExcluded(selectedWorkItem, PolicyExclusionType.WorkItemField);
        }

        /// <summary>
        /// Indicates whether the directory has been excluded from the policy.
        /// </summary>
        /// <param name="directory">The directory to check.</param>
        /// <returns><b>true</b> if the directory is excluded; otherwise, <b>false</b>.</returns>
        private bool IsFolderExcluded(DirectoryInfo directory)
        {
            return this.IsItemExcluded(directory, PolicyExclusionType.Directory);
        }

        /// <summary>
        /// Indicates whether the file has been excluded from the policy.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns><b>true</b> if the file is excluded; otherwise, <b>false</b>.</returns>
        private bool IsFileExcluded(FileInfo file)
        {
            return this.IsItemExcluded(file, PolicyExclusionType.FileName);
        }

        /// <summary>
        /// Indicates whether the item has been excluded from the policy.
        /// </summary>
        /// <param name="item">The object to test.</param>
        /// <param name="exclusionType">The type of exclusion to check.</param>
        /// <returns><b>true</b> if the file is excluded; otherwise, <b>false</b>.</returns>
        private bool IsItemExcluded(object item, PolicyExclusionType exclusionType)
        {
            bool retval = false;

            if (this.IsExclusionTypeActive(exclusionType))
            {
                retval = EvaluateExclusions(item, this.exclusions[exclusionType]);
            }

            return retval;
        }

        #endregion
    }
}