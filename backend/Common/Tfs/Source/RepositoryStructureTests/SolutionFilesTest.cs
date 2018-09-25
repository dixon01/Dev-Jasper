// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionFilesTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionFilesTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Gorba.Common.Tfs.RepositoryStructureTests.Utility;

    using Microsoft.Build.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test that verifies package solution files.
    /// </summary>
    [TestClass]
    public class SolutionFilesTest : TfsFileTestBase
    {
        private const string SandboxDirectory = "Sandbox";
        private const string ReferencesSolutionFolder = "References";

        private static readonly Regex GeneratedProjectFileRegex = new Regex(@"\.(CF|FX)\d\d\.csproj$");
        private static readonly Regex VNextProjectFileRegex = new Regex(@"\.vNext(\.Tests)?\.csproj$");

        /// <summary>
        /// Tests that all package solutions contain all
        /// local projects found in the Source directory.
        /// </summary>
        [TestMethod]
        public void TestSolutionsContainAllLocalProjects()
        {
            var solutions = this.GetAllPackageSolutions();
            foreach (var solutionFile in solutions)
            {
                Assert.IsNotNull(solutionFile.DirectoryName);
                var sourcesDir = new DirectoryInfo(Path.Combine(solutionFile.DirectoryName, SourceDirectory));
                if (!sourcesDir.Exists)
                {
                    continue;
                }

                var solution = LoadSolution(solutionFile);
                var localProjects =
                    solution.Projects.Where(
                        p => p.RelativePath.StartsWith(SourceDirectory + Path.DirectorySeparatorChar)).ToList();

                foreach (var sourceDir in sourcesDir.GetDirectories("*", SearchOption.AllDirectories))
                {
                    var projects = sourceDir.GetFiles("*.*proj").ToList();
                    projects.RemoveAll(f => GeneratedProjectFileRegex.IsMatch(f.Name));
                    projects.RemoveAll(f => VNextProjectFileRegex.IsMatch(f.Name));
                    Assert.IsFalse(
                        projects.Count > 1, "There is more than one project file in {0}", sourceDir.FullName);
                    foreach (var projectFile in projects)
                    {
                        var path = projectFile.FullName.Substring(solutionFile.DirectoryName.Length + 1);
                        var removed = localProjects.RemoveAll(p => p.RelativePath == path);
                        Assert.AreEqual(
                            1,
                            removed,
                            "Project {0} was found {1} times in the solution {2}",
                            projectFile.FullName,
                            removed,
                            solutionFile.Name);
                    }
                }

                Assert.AreEqual(
                    0,
                    localProjects.Count,
                    "{0} project(s) are not referenced in {1}: {2}",
                    localProjects.Count,
                    solutionFile.FullName,
                    TfsFileTestBase.AggregateStrings(localProjects.Select(p => p.ProjectName)));
            }
        }

        /// <summary>
        /// Tests that all package solutions contain only references outside
        /// the package that are really required by the "local" projects.
        /// </summary>
        [TestMethod]
        public void TestSolutionsContainOnlyRequiredReferenceProjects()
        {
            var solutions = this.GetAllPackageSolutions();
            foreach (var solutionFile in solutions)
            {
                Assert.IsNotNull(solutionFile.DirectoryName);
                var sourceDir = new DirectoryInfo(Path.Combine(solutionFile.DirectoryName, SourceDirectory));
                if (!sourceDir.Exists)
                {
                    continue;
                }

                var sourceDirName = SourceDirectory + Path.DirectorySeparatorChar;
                var solution = LoadSolution(solutionFile);
                var localProjects =
                    solution.Projects.Where(
                        p => p.RelativePath.StartsWith(sourceDirName)).Where(
                            FilterUnknownProjects).Select(
                                p => new ProjectInfo(p, solutionFile)).ToList();

                var referenceProjects =
                    solution.Projects.Where(
                        p => !p.RelativePath.StartsWith(sourceDirName)).Where(
                            FilterUnknownProjects).Select(
                                p => new ProjectInfo(p, solutionFile)).ToList();

                var unusedReferences = referenceProjects.ToList();

                foreach (var project in localProjects)
                {
                    RemoveReferencedProjects(project, unusedReferences);
                }

                unusedReferences.RemoveAll(p => TfsFileTestBase.IsTestProject(p.Instance));

                Assert.AreEqual(
                    0,
                    unusedReferences.Count,
                    "{0} reference project(s) are not used in {1}: {2}",
                    unusedReferences.Count,
                    solutionFile.FullName,
                    TfsFileTestBase.AggregateStrings(unusedReferences.Select(p => p.Project.ProjectName)));
            }
        }

        /// <summary>
        /// Tests that all package solutions have reference projects in the
        /// "References" folder and non-references not in that folder.
        /// </summary>
        [TestMethod]
        public void TestSolutionsHaveReferenceProjectsInRightPlace()
        {
            var solutions = this.GetAllPackageSolutions();
            foreach (var solutionFile in solutions)
            {
                Assert.IsNotNull(solutionFile.DirectoryName);
                var sourceDir = new DirectoryInfo(Path.Combine(solutionFile.DirectoryName, SourceDirectory));
                if (!sourceDir.Exists)
                {
                    continue;
                }

                var sourceDirName = SourceDirectory + Path.DirectorySeparatorChar;
                var solution = LoadSolution(solutionFile);
                var projects = solution.Projects.Where(FilterUnknownProjects).ToList();
                var solutionFolders =
                    solution.Projects.Where(p => p.ProjectType == SolutionProjectType.SolutionFolder).ToList();
                var referencesFolder = solutionFolders.FirstOrDefault(p => p.ProjectName == ReferencesSolutionFolder);

                foreach (var project in projects)
                {
                    if (project.RelativePath.StartsWith(sourceDirName))
                    {
                        // local project SHOULD NOT be inside references
                        if (referencesFolder == null)
                        {
                            continue;
                        }

                        Assert.IsFalse(
                            IsProjectInSolutionFolder(project, referencesFolder, solutionFolders),
                            "Solution {0} has a local project in \"{1}\" solution folder: {2}",
                            solutionFile.FullName,
                            ReferencesSolutionFolder,
                            project.ProjectName);
                    }
                    else
                    {
                        // reference project SHOULD be inside references
                        Assert.IsNotNull(
                            referencesFolder,
                            "Solution {0} contains no \"{1}\" solution folder but has an external reference: {2}",
                            solutionFile.FullName,
                            ReferencesSolutionFolder,
                            project.RelativePath);

                        Assert.IsTrue(
                            IsProjectInSolutionFolder(project, referencesFolder, solutionFolders),
                            "Solution {0} has a reference project outside \"{1}\" solution folder: {2}",
                            solutionFile.FullName,
                            ReferencesSolutionFolder,
                            project.ProjectName);
                    }
                }
            }
        }

        /// <summary>
        /// Tests that all package solutions have no references to the Sandbox folder.
        /// </summary>
        [TestMethod]
        public void TestSolutionsHaveNoReferenceToSandbox()
        {
            var solutions = this.GetAllPackageSolutions();
            foreach (var solutionFile in solutions)
            {
                Assert.IsNotNull(solutionFile.DirectoryName);
                var sourceDir = new DirectoryInfo(Path.Combine(solutionFile.DirectoryName, SourceDirectory));
                if (!sourceDir.Exists)
                {
                    continue;
                }

                var solution = LoadSolution(solutionFile);
                foreach (var project in solution.Projects)
                {
                    Assert.IsFalse(
                        project.RelativePath.Contains(SandboxDirectory),
                        "Solution {0} references a project from the {1}: {2}",
                        solutionFile.FullName,
                        SandboxDirectory,
                        project.RelativePath);
                }
            }
        }

        /// <summary>
        /// Tests that all package solutions have only references to local projects
        /// in the "Source" directory.
        /// </summary>
        [TestMethod]
        public void TestSolutionsHaveNoReferenceToLocalProjectsOutsideSource()
        {
            var solutions = this.GetAllPackageSolutions();
            foreach (var solutionFile in solutions)
            {
                Assert.IsNotNull(solutionFile.DirectoryName);
                var sourceDir = new DirectoryInfo(Path.Combine(solutionFile.DirectoryName, SourceDirectory));
                if (!sourceDir.Exists)
                {
                    continue;
                }

                var sourceDirName = SourceDirectory + Path.DirectorySeparatorChar;
                var solution = LoadSolution(solutionFile);
                foreach (
                    var project in solution.Projects.Where(p => p.ProjectType != SolutionProjectType.SolutionFolder))
                {
                    if (!project.RelativePath.StartsWith(@"..\"))
                    {
                        Assert.IsTrue(
                            project.RelativePath.StartsWith(sourceDirName),
                            "Solution {0} references a local project from outside \"{1}\": {2}",
                            solutionFile.FullName,
                            TfsFileTestBase.SourceDirectory,
                            project.RelativePath);
                    }
                }
            }
        }

        private static bool IsProjectInSolutionFolder(
            SolutionProject project, SolutionProject referencesFolder, ICollection<SolutionProject> solutionFolders)
        {
            var parentGuid = project.ParentProjectGuid;
            while (parentGuid != null)
            {
                if (parentGuid.Equals(referencesFolder.ProjectGuid, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                var parent = parentGuid;
                parentGuid =
                    solutionFolders.Where(f => f.ProjectGuid == parent).Select(
                        f => f.ParentProjectGuid).FirstOrDefault();
            }

            return false;
        }

        private static void RemoveReferencedProjects(ProjectInfo project, ICollection<ProjectInfo> unusedReferences)
        {
            var directory = project.Instance.Directory;
            var references = project.Instance.Items.Where(
                i => i.ItemType == "ProjectReference").Select(i => i.EvaluatedInclude);
            foreach (var reference in references)
            {
                var referencePath = Path.GetFullPath(Path.Combine(directory, reference));
                var refProject =
                    unusedReferences.FirstOrDefault(
                        p => p.Instance.FullPath.Equals(referencePath, StringComparison.InvariantCultureIgnoreCase));

                if (refProject == null)
                {
                    continue;
                }

                unusedReferences.Remove(refProject);
                RemoveReferencedProjects(refProject, unusedReferences);
            }
        }

        private class ProjectInfo
        {
            public ProjectInfo(SolutionProject project, FileInfo solutionFile)
            {
                this.Instance = CreateProjectInstance(solutionFile.Directory, project);
                this.Project = project;
            }

            public ProjectInstance Instance { get; private set; }

            public SolutionProject Project { get; private set; }
        }
    }
}