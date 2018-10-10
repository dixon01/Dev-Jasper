// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaTfs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GorbaTfs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Static helper class for Gorba TFS specific constants and methods.
    /// </summary>
    public static class GorbaTfs
    {
        /// <summary>
        /// The full URL of the Gorba TFS default collection
        /// </summary>
        public static readonly string Url = "https://tfsgorba.gorba.com:8443/tfs/DefaultCollection";

        /// <summary>
        /// The team project name ("Gorba").
        /// </summary>
        public static readonly string TeamProjectName = "Gorba";

        /// <summary>
        /// Path to $/Gorba/Main
        /// </summary>
        public static readonly string MainPath = "$/Gorba/Main";

        /// <summary>
        /// Path to $/Gorba/Meta
        /// </summary>
        public static readonly string MetaPath = "$/Gorba/Meta";

        /// <summary>
        /// Third party folder name: 3rdParty
        /// </summary>
        public static readonly string ThirdPartyFolder = "3rdParty";

        private static readonly Regex BuildServerPathRegex = new Regex(@"[\\/]\d+[\\/]");

        /// <summary>
        /// Gets the currently used locally mapped main folder.
        /// This method fails with an assertion exception, if no valid workspace can be found.
        /// </summary>
        /// <returns>
        /// the main folder, never null.
        /// </returns>
        public static DirectoryInfo GetCurrentLocalMainFolder()
        {
            var tfsProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(Url));
            var vcs = tfsProjectCollection.GetService<VersionControlServer>();
            var workspaces = vcs.QueryWorkspaces(null, null, Environment.MachineName);
            var workspace = workspaces.Where(w => w.Folders.Any(IsCurrentWorkspace))
                .OrderBy(w => w.LastAccessDate).LastOrDefault();

            const string NoValidWorkspace =
                "Couldn't find a valid workspace to run tests on, "
                + "make sure you have {0} as a root folder in your workspace";
            Assert.IsNotNull(
                workspace,
                NoValidWorkspace,
                MainPath);

            var mapping = workspace.Folders.First(f => f.ServerItem == MainPath);
            return new DirectoryInfo(mapping.LocalItem);
        }

        private static bool IsCurrentWorkspace(WorkingFolder f)
        {
            if (f.ServerItem != MainPath)
            {
                return false;
            }

            var dllPath = Assembly.GetExecutingAssembly().Location;
            if (dllPath.StartsWith(f.LocalItem, StringComparison.InvariantCultureIgnoreCase))
            {
                // this is the current workspace on a local developer machine
                return true;
            }

            if (!BuildServerPathRegex.IsMatch(f.LocalItem))
            {
                // we are not on the build server
                return false;
            }

            var buildRoot = Path.GetFullPath(Path.Combine(f.LocalItem, @"..\.."));
            return dllPath.StartsWith(buildRoot, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
