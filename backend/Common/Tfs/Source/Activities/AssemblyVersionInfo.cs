// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyVersionInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssemblyVersionInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    using System;
    using System.Activities;
    using System.IO;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// The assembly version info activity.
    /// </summary>
    [BuildActivity(HostEnvironmentOption.All)]
    public class AssemblyVersionInfo : CodeActivity
    {
        /// <summary>
        /// Gets or sets BuildSettings.
        /// </summary>
        public InArgument<BuildSettings> BuildSettings { get; set; }

        /// <summary>
        /// Gets or sets SourcesDirectory.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> SourcesDirectory { get; set; }

        /// <summary>
        /// Gets or sets FileName.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FileNames { get; set; }

        /// <summary>
        /// Gets or sets the TFS BuildNumber.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> TfsBuildNumber { get; set; }

        /// <summary>
        /// Gets or sets Workspace.
        /// </summary>
        public InArgument<Workspace> Workspace { get; set; }

        /// <summary>
        /// Gets or sets NewAssemblyFileVersion.
        /// </summary>
        public OutArgument<string> NewAssemblyFileVersion { get; set; }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            var log = new Log(context);
            try
            {
                var sourcesDirectory = this.SourcesDirectory.Get(context);
                string[] solutionVersionFiles = this.FileNames.Get(context).Split(Path.PathSeparator);
                BuildSettings buildSettings = this.BuildSettings.Get(context);
                Workspace workspace = this.Workspace.Get(context);

                var assemblyFileVersion = string.Empty;

                foreach (var solutionVersionFile in solutionVersionFiles)
                {
                    var fileName = Path.Combine(sourcesDirectory, solutionVersionFile);
                    var updater = new AssemblyVersionUpdater(log)
                        { SolutionVersionFile = fileName, BuildSettings = buildSettings, Workspace = workspace };
                    updater.Execute();

                    if (string.IsNullOrEmpty(assemblyFileVersion))
                    {
                        assemblyFileVersion = updater.AssemblyFileVersion;
                    }
                }

                this.NewAssemblyFileVersion.Set(context, assemblyFileVersion);
            }
            catch (Exception ex)
            {
                log.Warning("Could not update assembly version: {0}", ex);
            }
        }
    }
}