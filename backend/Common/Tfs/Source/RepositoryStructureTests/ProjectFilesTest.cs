// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectFilesTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectFilesTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Tfs.RepositoryStructureTests.Utility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for Visual Studio project files.
    /// </summary>
    [TestClass]
    public class ProjectFilesTest : TfsFileTestBase
    {
        private const string SolutionAssemblyProductInfo = "SolutionAssemblyProductInfo.cs";

        private static readonly List<string> AllowedNonFrameworkDlls = new List<string>
            {
                // Visual Studio
                "Microsoft.VisualStudio.QualityTools.UnitTestFramework",

                // TFS
                "Microsoft.TeamFoundation",
                "Microsoft.TeamFoundation.Client",
                "Microsoft.TeamFoundation.Common",
                "Microsoft.TeamFoundation.VersionControl.Client",
                "Microsoft.TeamFoundation.Build.Client",
                "Microsoft.TeamFoundation.Build.Workflow",
                "Microsoft.TeamFoundation.TestImpact.Client",
                "Microsoft.TeamFoundation.WorkItemTracking.Client",

                // UI testing
                "Microsoft.VisualStudio.QualityTools.CodedUITestFramework",
                "Microsoft.VisualStudio.TestTools.UITesting",
                "Microsoft.VisualStudio.TestTools.UITest.Common",
                "Microsoft.VisualStudio.TestTools.UITest.Extension",

                // Azure
                "Microsoft.WindowsAzure.Diagnostics",
                "Microsoft.WindowsAzure.ServiceRuntime",

                // Other
                "System.Management.Automation",
                "System.Speech"
            };

        /// <summary>
        /// Tests that all project files are in the right folder and have the
        /// right project name according to the namespace.
        /// Also checks that the namespace configured in the project is correct.
        /// </summary>
        [TestMethod]
        public void TestProjectFileNamespaces()
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

                var namespaceParts =
                    solutionFile.DirectoryName.Split(Path.DirectorySeparatorChar).Reverse().Take(2).Reverse().ToList();

                var sourceDirName = SourceDirectory + Path.DirectorySeparatorChar;
                var solution = LoadSolution(solutionFile);
                foreach (var project in solution.Projects.Where(p => p.RelativePath.StartsWith(sourceDirName)))
                {
                    var projectNamespace =
                        project.RelativePath.Split(Path.DirectorySeparatorChar).Reverse().Skip(1).TakeWhile(
                            p => p != SourceDirectory).Reverse().ToList();

                    Assert.IsTrue(
                        projectNamespace.Where(p => !p.EndsWith(".Tests")).All(p => !p.Contains(".")),
                        "Project file name contains a dot, this is only allowed for \".Tests\" projects: {0}",
                        project.RelativePath);

                    namespaceParts.AddRange(projectNamespace);
                    try
                    {
                        var expectedNamespace = namespaceParts.Aggregate("Gorba", (a, b) => a + "." + b);

                        Assert.IsTrue(
                            expectedNamespace.EndsWith("." + project.ProjectName),
                            "Project {0} should have a name like {1}; Path: {2}",
                            project.ProjectName,
                            expectedNamespace,
                            project.RelativePath);

                        var projectFileName = Path.GetFileNameWithoutExtension(project.RelativePath);
                        Assert.IsNotNull(projectFileName, "Invalid project path: {0}", project.RelativePath);

                        Assert.IsTrue(
                            expectedNamespace.EndsWith(projectFileName),
                            "Project {0} should have a file name like {1}, but is {2}",
                            project.ProjectName,
                            expectedNamespace,
                            project.RelativePath);

                        if (project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                        {
                            var instance = CreateProjectInstance(solutionFile.Directory, project);

                            Assert.AreEqual(
                                expectedNamespace,
                                instance.GetPropertyValue("RootNamespace"),
                                "Project has wrong namespace: {0}",
                                instance.FullPath);
                        }
                    }
                    finally
                    {
                        namespaceParts.RemoveRange(
                            namespaceParts.Count - projectNamespace.Count, projectNamespace.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Tests that all C# project files contain at least one source file.
        /// </summary>
        [TestMethod]
        public void TestNoEmptyProjects()
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
                foreach (var project in solution.Projects.Where(p => p.RelativePath.StartsWith(sourceDirName)
                    && p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat))
                {
                    var instance = CreateProjectInstance(solutionFile.Directory, project);
                    var compiles = instance.GetItems("Compile").Where(
                        i => !i.EvaluatedInclude.EndsWith("AssemblyInfo.cs"));
                    Assert.IsTrue(
                        compiles.Any(),
                        "Project doesn't contain any files to compile: {0}",
                        project.RelativePath);
                }
            }
        }

        /// <summary>
        /// Tests that all C# project files contain a reference to the right "SolutionAssemblyProductInfo.cs".
        /// </summary>
        [TestMethod]
        public void TestSolutionAssemblyProductInfo()
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
                foreach (var project in solution.Projects.Where(p => p.RelativePath.StartsWith(sourceDirName)
                    && p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat
                    && !p.RelativePath.EndsWith(".Tests.csproj")))
                {
                    var instance = CreateProjectInstance(solutionFile.Directory, project);
                    var productInfo =
                        instance.GetItems("Compile")
                                .FirstOrDefault(i => i.EvaluatedInclude.EndsWith("..\\" + SolutionAssemblyProductInfo));

                    Assert.IsNotNull(
                        productInfo,
                        "Project doesn't contain reference to '{0}': {1}",
                        SolutionAssemblyProductInfo,
                        project.RelativePath);

                    var fullPath = Path.GetFullPath(Path.Combine(instance.Directory, productInfo.EvaluatedInclude));
                    var expectedPath = Path.Combine(solutionFile.DirectoryName, "Source", SolutionAssemblyProductInfo);
                    Assert.AreEqual(
                        expectedPath,
                        fullPath,
                        "Project doesn't point to the correct {0}: {1}",
                        SolutionAssemblyProductInfo,
                        project.RelativePath);
                }
            }
        }

        /// <summary>
        /// Tests that all C# project files have correct references,
        /// either to a project or a 3rd-party DLL stored in TFS.
        /// Some exceptions are allowed for installed frameworks.
        /// </summary>
        [TestMethod]
        public void TestAllReferencesInProjects()
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
                foreach (var project in solution.Projects.Where(
                    p => p.RelativePath.StartsWith(sourceDirName) && p.RelativePath.EndsWith(".csproj")))
                {
                    var instance = CreateProjectInstance(solutionFile.Directory, project);
                    var buildToolsRoot = instance.GetPropertyValue("MSBuildToolsRoot");
                    var targetedRuntimeVersion = instance.GetPropertyValue("TargetedRuntimeVersion");
                    var frameworkPath = Path.Combine(buildToolsRoot, targetedRuntimeVersion);
                    if (!Directory.Exists(frameworkPath))
                    {
                        frameworkPath = instance.GetPropertyValue("MSBuildFrameworkToolsPath");
                    }

                    foreach (var reference in instance.GetItems("Reference"))
                    {
                        var hintPath = reference.GetMetadataValue("HintPath");
                        if (!string.IsNullOrWhiteSpace(hintPath))
                        {
                            this.AssertIsInsideTfsMain(
                                Path.Combine(instance.Directory, hintPath), project.ProjectName);
                            continue;
                        }

                        var dllName = reference.EvaluatedInclude.Split(',').First();
                        if (AllowedNonFrameworkDlls.Contains(dllName))
                        {
                            continue;
                        }

                        var frameworkDll = Path.Combine(frameworkPath, dllName + ".dll");
                        if (File.Exists(frameworkDll))
                        {
                            continue;
                        }

                        var wpfDll = Path.Combine(frameworkPath, "WPF", dllName + ".dll");
                        Assert.IsTrue(
                            File.Exists(wpfDll),
                            "DLL referenced by {0} doesn't have a hint path and is not part of .NET Framework: {1}",
                            project.ProjectName,
                            reference.EvaluatedInclude);
                    }

                    foreach (var reference in instance.GetItems("ProjectReference"))
                    {
                        var referenceFile = Path.Combine(instance.Directory, reference.EvaluatedInclude);

                        this.AssertIsInsideTfsMain(referenceFile, project.ProjectName);
                    }
                }
            }
        }

        // ReSharper disable UnusedParameter.Local
        private void AssertIsInsideTfsMain(string filename, string projectName)
        {
            filename = Path.GetFullPath(filename);
            Assert.IsTrue(File.Exists(filename), "Reference in {0} doesn't exist: {1}", projectName, filename);

            Assert.IsTrue(
                filename.StartsWith(this.LocalMainFolder.FullName, StringComparison.InvariantCultureIgnoreCase),
                "Reference in {0} points outside TFS Main directory: {1}",
                projectName,
                filename);
        }

        // ReSharper restore UnusedParameter.Local
    }
}
