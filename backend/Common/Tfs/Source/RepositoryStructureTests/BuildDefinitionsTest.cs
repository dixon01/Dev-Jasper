// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildDefinitionsTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BuildDefinitionsTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Tfs.RepositoryStructureTests.Utility;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests all build definitions in the Gorba team project.
    /// </summary>
    [TestClass]
    public class BuildDefinitionsTest : TfsFileTestBase
    {
        private const string CommonTfsBuildDefinition = "Common_Tfs";

        private const string IconsPath = "/Icons";

        private IBuildDefinition[] allBuildDefinitions;

        /// <summary>
        /// Initializes the test environment.
        /// </summary>
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            var tfsProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(GorbaTfs.Url));
            var buildServer = tfsProjectCollection.GetService<IBuildServer>();
            this.allBuildDefinitions = buildServer.QueryBuildDefinitions(GorbaTfs.TeamProjectName);
        }

        /// <summary>
        /// Tests that all packages have a build definition which is set for
        /// continuous integration.
        /// </summary>
        [TestMethod]
        public void TestPackagesHaveContinuousIntegrationBuildDefinition()
        {
            foreach (var solution in this.GetAllPackageSolutions())
            {
                var buildDefinition = this.GetStandardBuildDefinition(solution);
                Assert.IsNotNull(
                    buildDefinition,
                    "Couldn't find default build definition with Continuous Integration enabled for {0}",
                    solution.FullName);
            }
        }

        /// <summary>
        /// Tests that all package build definitions contain the right
        /// Solution Version Files (SolutionAssemblyProductInfo.cs) and
        /// don't contain any unused ones.
        /// </summary>
        [TestMethod]
        public void TestPackageBuildDefinitionVersionFiles()
        {
            foreach (var tuple in this.GetAllPackageBuildDefinitions())
            {
                var buildDefinition = tuple.Item2;

                if (buildDefinition.Name == CommonTfsBuildDefinition)
                {
                    // we can't test our own Build Definition since we have a special workspace setting
                    continue;
                }

                var parameters = WorkflowHelpers.DeserializeProcessParameters(buildDefinition.ProcessParameters);

                object solutionVersionFilesParam;
                parameters.TryGetValue("SolutionVersionFile", out solutionVersionFilesParam);
                if (!(solutionVersionFilesParam is string))
                {
                    continue;
                }

                var solutionVersionFiles =
                    ((string)solutionVersionFilesParam).Split(';').Where(f => !string.IsNullOrEmpty(f)).ToList();

                var buildSettings = parameters["BuildSettings"] as BuildSettings;
                Assert.IsNotNull(buildSettings, "Couldn't find build settings for {0}", buildDefinition.Name);
                var project = buildSettings.ProjectsToBuild[0];
                var projectPath = project.Substring(0, project.LastIndexOf('/'))
                    .Replace(GorbaTfs.MainPath, string.Empty).Replace('/', '\\').Trim('\\');

                Assert.AreEqual(
                    0,
                    solutionVersionFiles.FindIndex(
                        f => f.IndexOf(projectPath, StringComparison.OrdinalIgnoreCase) >= 0),
                    "First SolutionAssemblyProductInfo.cs must be the one of the built solution: {0}",
                    buildDefinition.Name);

                foreach (
                    var mapping in
                        this.GetStandardMappings(buildDefinition).Where(m => !m.ServerItem.EndsWith(IconsPath)))
                {
                    var expectedFileName = mapping.LocalItem.EndsWith("\\Source")
                                               ? mapping.LocalItem + "\\SolutionAssemblyProductInfo.cs"
                                               : mapping.LocalItem + "\\Source\\SolutionAssemblyProductInfo.cs";
                    var solutionVersionFile = solutionVersionFiles.FirstOrDefault(expectedFileName.EndsWith);
                    Assert.IsNotNull(
                        solutionVersionFile,
                        "Couldn't find {0} in 'Solution Version Files' config of {1}",
                        expectedFileName,
                        buildDefinition.Name);

                    solutionVersionFiles.Remove(solutionVersionFile);
                }

                Assert.AreEqual(
                    0,
                    solutionVersionFiles.Count,
                    "Unused 'Solution Version Files' found in {0}: {1}",
                    buildDefinition.Name,
                    TfsFileTestBase.AggregateStrings(solutionVersionFiles));
            }
        }

        /// <summary>
        /// Tests all package build definitions that they only contain one
        /// platform for "Release" and that only one project is built.
        /// </summary>
        [TestMethod]
        public void TestPackageBuildDefinitionBuildItems()
        {
            foreach (var tuple in this.GetAllPackageBuildDefinitions())
            {
                var solutionFile = tuple.Item1;
                var buildDefinition = tuple.Item2;
                var parameters = WorkflowHelpers.DeserializeProcessParameters(buildDefinition.ProcessParameters);

                var buildSettings = parameters["BuildSettings"] as BuildSettings;
                Assert.IsNotNull(buildSettings, "Couldn't find build settings for {0}", buildDefinition.Name);
                Assert.AreEqual(
                    1,
                    buildSettings.PlatformConfigurations.Count,
                    "Expected exactly one platform configuration for {0}",
                    buildDefinition.Name);

                Assert.AreEqual(
                    "Release",
                    buildSettings.PlatformConfigurations[0].Configuration,
                    "Expected Release build for {0}",
                    buildDefinition.Name);

                Assert.AreEqual(
                    1,
                    buildSettings.ProjectsToBuild.Count,
                    "Expected exactly one project to build for {0}",
                    buildDefinition.Name);

                var project = buildSettings.ProjectsToBuild[0];

                Assert.IsTrue(
                    project.EndsWith("/" + solutionFile.Name),
                    "Build definition {0} doesn't build the default solution: {1}",
                    buildDefinition.Name,
                    solutionFile.Name);

                if (buildDefinition.Name == CommonTfsBuildDefinition)
                {
                    // special case: we have to get the entire tree for Common_Tfs,
                    // therefore we don't have a mapping for the solution directory
                    continue;
                }

                var projectPath = project.Substring(0, project.LastIndexOf('/'));

                var solutionMapping = buildDefinition.Workspace.Mappings.FirstOrDefault(
                    m => m.ServerItem == projectPath);
                Assert.IsNotNull(
                    solutionMapping,
                    "Didn't find folder mapping for solution to build: '{0}' in '{1}'",
                    project,
                    buildDefinition.Name);
            }
        }

        /// <summary>
        /// Tests all package build definitions to have the right workspace.
        /// It verifies that all necessary paths are mapped and that no unnecessary ones are mapped.
        /// </summary>
        [TestMethod]
        public void TestPackageBuildDefinitionWorkspaces()
        {
            foreach (var tuple in this.GetAllPackageBuildDefinitions())
            {
                var solutionFile = tuple.Item1;
                var buildDefinition = tuple.Item2;

                this.VerifyWorkspaceMappings(solutionFile, buildDefinition);

                this.VerifyCorrectWorkspaceMappingNesting(buildDefinition);
            }
        }

        private void VerifyWorkspaceMappings(FileInfo solutionFile, IBuildDefinition buildDefinition)
        {
            Assert.IsNotNull(solutionFile.DirectoryName);

            var thirdPartyItem = GorbaTfs.MainPath + "/" + GorbaTfs.ThirdPartyFolder;
            var thirdParty =
                buildDefinition.Workspace.Mappings.FirstOrDefault(
                    m => m.ServerItem.Equals(thirdPartyItem, StringComparison.InvariantCultureIgnoreCase));
            if (thirdParty != null)
            {
                Assert.Fail("Direct mapping for {0} not allowed in {1}", thirdParty.ServerItem, buildDefinition.Name);
            }

            var unusedMappings = new List<IWorkspaceMapping>(buildDefinition.Workspace.Mappings);

            // remove the "$/Gorba/Meta" mapping since we can't verify (for now) if we need it
            unusedMappings.RemoveAll(m => m.ServerItem.StartsWith(GorbaTfs.MetaPath));

            // remove mappings for /Icons directories since they are handled specially
            unusedMappings.RemoveAll(m => m.ServerItem.EndsWith(IconsPath));

            var solution = LoadSolution(solutionFile);

            foreach (var project in
                solution.Projects.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat))
            {
                var directoryName = solutionFile.DirectoryName;
                Assert.IsNotNull(directoryName);
                var fullPath = Path.Combine(directoryName, project.RelativePath);
                var sourceControlName = this.LocalToSourceControlPath(fullPath);

                var mapping =
                    buildDefinition.Workspace.Mappings.FirstOrDefault(m => sourceControlName.StartsWith(m.ServerItem));
                Assert.IsNotNull(
                    mapping,
                    "Couldn't find directory mapping for {0} in {1}",
                    sourceControlName,
                    buildDefinition.Name);

                unusedMappings.Remove(mapping);

                var instance = CreateProjectInstance(solutionFile.Directory, project);
                foreach (var dllReference in
                    instance.GetItems("Reference")
                        .Select(r => r.GetMetadataValue("HintPath"))
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .Select(p => Path.GetFullPath(Path.Combine(instance.Directory, p))))
                {
                    sourceControlName = this.LocalToSourceControlPath(dllReference);
                    mapping =
                        buildDefinition.Workspace.Mappings.FirstOrDefault(
                            m => sourceControlName.StartsWith(m.ServerItem));
                    Assert.IsNotNull(
                        mapping,
                        "Couldn't find directory mapping for reference to {0} in {1} in {2}",
                        sourceControlName,
                        project.ProjectName,
                        buildDefinition.Name);

                    unusedMappings.Remove(mapping);
                }
            }

            Assert.AreEqual(
                0,
                unusedMappings.Count,
                "Unused mappings found in {0}: {1}",
                buildDefinition.Name,
                TfsFileTestBase.AggregateStrings(unusedMappings.Select(m => m.ServerItem)));
        }

        /// <summary>
        /// Verifies that the workspace only contains mappings to either a package folder
        /// or its "Source" directory (this is recommended for project references).
        /// </summary>
        /// <param name="buildDefinition">
        /// The build definition.
        /// </param>
        private void VerifyCorrectWorkspaceMappingNesting(IBuildDefinition buildDefinition)
        {
            foreach (var mapping in this.GetStandardMappings(buildDefinition))
            {
                if (mapping.ServerItem == GorbaTfs.MainPath && buildDefinition.Name == CommonTfsBuildDefinition)
                {
                    // special case: we have to get the entire tree for Common_Tfs
                    continue;
                }

                Assert.IsTrue(
                    mapping.ServerItem.StartsWith(GorbaTfs.MainPath),
                    "Workspace mapping {0} of {3} is not in {1} or {2}",
                    mapping.ServerItem,
                    GorbaTfs.MainPath,
                    GorbaTfs.MetaPath,
                    buildDefinition.Name);

                if (mapping.ServerItem.EndsWith(IconsPath))
                {
                    // special case: ignore any mappings for icon paths from different applications
                    continue;
                }

                var pathParts = mapping.ServerItem.Substring(GorbaTfs.MainPath.Length).Split('/');
                Assert.IsTrue(
                    pathParts.Length == 3 || (pathParts.Length == 4 && pathParts[3] == TfsFileTestBase.SourceDirectory),
                    "Workspace mapping {0} of {1} doesn't point to a package root or its Source subdirectory",
                    mapping.ServerItem,
                    buildDefinition.Name);
            }
        }

        private IEnumerable<IWorkspaceMapping> GetStandardMappings(IBuildDefinition buildDefinition)
        {
            return buildDefinition.Workspace.Mappings.Where(
                m =>
                !m.ServerItem.StartsWith(GorbaTfs.MetaPath)
                && !m.ServerItem.Contains("/" + GorbaTfs.ThirdPartyFolder));
        }

        private string LocalToSourceControlPath(string path)
        {
            path = Path.GetFullPath(path);

            Assert.IsTrue(path.StartsWith(this.LocalMainFolder.FullName));
            var rootRelative = path.Substring(this.LocalMainFolder.FullName.Length);
            var sourceControlName = GorbaTfs.MainPath + rootRelative.Replace('\\', '/');
            return sourceControlName;
        }

        private IEnumerable<Tuple<FileInfo, IBuildDefinition>> GetAllPackageBuildDefinitions()
        {
            return
                this.GetAllPackageSolutions()
                    .Select(s => new Tuple<FileInfo, IBuildDefinition>(s, this.GetStandardBuildDefinition(s)))
                    .Where(t => t.Item2 != null);
        }

        private IBuildDefinition GetStandardBuildDefinition(FileInfo solution)
        {
            var fileName = Path.GetFileNameWithoutExtension(solution.Name);
            Assert.IsNotNull(fileName, "Bad solution file name: {0}", solution.Name);
            var expectedBuildName = fileName.Replace('.', '_');

            // expectedBuildName also contains "Gorba_", so we use EndsWith()
            return this.allBuildDefinitions.FirstOrDefault(
                d =>
                expectedBuildName.EndsWith(d.Name)
                && d.ContinuousIntegrationType == ContinuousIntegrationType.Individual
                && d.Enabled);
        }
    }
}
