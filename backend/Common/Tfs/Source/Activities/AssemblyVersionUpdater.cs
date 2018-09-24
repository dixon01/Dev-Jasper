// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyVersionUpdater.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssemblyVersionUpdater type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Implementation of the version update mechanism.
    /// </summary>
    public class AssemblyVersionUpdater
    {
        private static readonly Regex VersionInfoRegex = new Regex(
            @"(\[\s*assembly:\s*(System\.Reflection\.)?AssemblyFileVersion\("")(\d+\.\d+\.)\d+\.\d+(""\s*\)\s*])");

        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyVersionUpdater"/> class.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// if <see cref="log"/> is null.
        /// </exception>
        public AssemblyVersionUpdater(ILog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            this.log = log;
        }

        /// <summary>
        /// Gets or sets the C# code file that contains the <see cref="AssemblyFileVersion"/> attribute.
        /// </summary>
        public string SolutionVersionFile { get; set; }

        /// <summary>
        /// Gets or sets the TFS build settings.
        /// </summary>
        public BuildSettings BuildSettings { get; set; }

        /// <summary>
        /// Gets or sets the build workspace.
        /// </summary>
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets the newly created assembly file version.
        /// </summary>
        public string AssemblyFileVersion { get; private set; }

        /// <summary>
        /// Executes the update with the given properties.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// if <see cref="SolutionVersionFile"/> does not exist on the local disk.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// if something went wrong during the version update.
        /// </exception>
        public void Execute()
        {
            if (!File.Exists(this.SolutionVersionFile))
            {
                throw new FileNotFoundException("Could not find solution version file", this.SolutionVersionFile);
            }

            int buildNumber = this.GetBuildNumber(DateTime.Now);
            int revisionNumber = this.GetRevisionNumber();

            try
            {
                this.UpdateVersionFile(buildNumber, revisionNumber);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Could not update version file", ex);
            }
        }

        /// <summary>
        /// Gets the build number derived from the given date.
        /// The build number is YYWW (where YY is the year of Monday of the week of the given date
        /// and WW is the week number of the given date)
        /// </summary>
        /// <param name="date">the date to compute from</param>
        /// <returns>the build number</returns>
        private int GetBuildNumber(DateTime date)
        {
            // always take Monday for the week and year number
            date -= new TimeSpan(((int)date.DayOfWeek + 6) % 7, 0, 0, 0);

            var info = DateTimeFormatInfo.InvariantInfo;
            int week = info.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return ((date.Year % 100) * 100) + week;
        }

        /// <summary>
        /// Gets the revision number which is the latest change set ID of the directory of the
        /// <see cref="SolutionVersionFile"/>.
        /// </summary>
        /// <returns>the revision number.</returns>
        private int GetRevisionNumber()
        {
            try
            {
                var localPath = Path.GetDirectoryName(this.SolutionVersionFile);
                var vcs = this.Workspace.VersionControlServer;
                var history = vcs.QueryHistory(
                    localPath, VersionSpec.Latest, 0, RecursionType.Full, null, null, null, 1, false, true, false);
                return history.Cast<Changeset>().Select(historyItem => historyItem.ChangesetId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                this.log.Warning("Could not get revision number from version control: {0}", ex);
                return 9999;
            }
        }

        /// <summary>
        /// Updates the solution version file with the given build and revision numbers.
        /// </summary>
        /// <param name="build">the build number.</param>
        /// <param name="revision">the revision number.</param>
        private void UpdateVersionFile(int build, int revision)
        {
            var filePath = this.SolutionVersionFile;
            this.log.Message("Update verison in {0} to x.y.{1}.{2}", filePath, build, revision);

            var fileAttributes = File.GetAttributes(filePath);
            try
            {
                File.SetAttributes(filePath, fileAttributes & ~FileAttributes.ReadOnly);

                MatchEvaluator replace = match =>
                    {
                        var version = string.Format("{0}{1}.{2}", match.Groups[3].Value, build, revision);
                        if (string.IsNullOrEmpty(this.AssemblyFileVersion))
                        {
                            this.AssemblyFileVersion = version;
                        }

                        return match.Groups[1].Value + version + match.Groups[4].Value;
                    };
                var lines = File.ReadAllLines(filePath).Select(line => VersionInfoRegex.Replace(line, replace));
                File.WriteAllLines(filePath, lines);
                this.log.Message("AssemblyFileVersion = \"{0}\"", this.AssemblyFileVersion);
            }
            finally
            {
                File.SetAttributes(filePath, fileAttributes);
            }
        }
    }
}
