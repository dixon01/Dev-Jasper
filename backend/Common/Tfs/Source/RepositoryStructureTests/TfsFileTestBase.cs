// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TfsFileTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TfsFileTestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Tfs.RepositoryStructureTests.Utility;

    using Microsoft.Build.Execution;

    /// <summary>
    /// Base class for all tests that require solution and project file information.
    /// </summary>
    public abstract class TfsFileTestBase : TfsTestBase
    {
        /// <summary>
        /// The "Source" directory.
        /// </summary>
        protected static readonly string SourceDirectory = "Source";

        /// <summary>
        /// Returns true if the project is MS Build compatible.
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <returns>
        /// true if the project is MS Build compatible.
        /// </returns>
        protected static bool FilterUnknownProjects(SolutionProject project)
        {
            return project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat;
        }

        /// <summary>
        /// Returns true if the project is a C# or VB test project.
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <returns>
        /// true if the project is a C# or VB test project.
        /// </returns>
        protected static bool IsTestProject(ProjectInstance project)
        {
            // GUID {3AC096D0-A1C2-E12C-1390-A8335801FDAB} is used to signal it is a test project
            var types = project.GetPropertyValue("ProjectTypeGuids");
            return types != null && types.Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");
        }

        /// <summary>
        /// Aggregates the given strings for display using a comma.
        /// </summary>
        /// <param name="strings">
        /// The strings.
        /// </param>
        /// <returns>
        /// the aggregated string.
        /// </returns>
        protected static string AggregateStrings(IEnumerable<string> strings)
        {
            var list = strings.ToList();
            return list.Count == 0 ? string.Empty : list.Aggregate((a, b) => a + ", " + b);
        }

        /// <summary>
        /// Creates a <see cref="ProjectInstance"/> for a given <see cref="SolutionProject"/>.
        /// </summary>
        /// <param name="solutionDirectory">
        /// The solution directory.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <returns>
        /// a new <see cref="ProjectInstance"/>.
        /// </returns>
        protected static ProjectInstance CreateProjectInstance(DirectoryInfo solutionDirectory, SolutionProject project)
        {
            return new ProjectInstance(Path.Combine(solutionDirectory.FullName, project.RelativePath));
        }

        /// <summary>
        /// Loads the solution from the given file.
        /// </summary>
        /// <param name="solutionFile">
        /// The solution file.
        /// </param>
        /// <returns>
        /// the parser.
        /// </returns>
        protected static SolutionParser LoadSolution(FileInfo solutionFile)
        {
            var solution = new SolutionParser();
            using (var reader = solutionFile.OpenText())
            {
                solution.Load(reader);
            }

            return solution;
        }

        /// <summary>
        /// Gets all solution files that match the defined package solution
        /// format of "Gorba.x.y.sln"
        /// </summary>
        /// <returns>
        /// all package solution files.
        /// </returns>
        protected IEnumerable<FileInfo> GetAllPackageSolutions()
        {
            var productFolders = this.GetProductFolders();

            foreach (var productFolder in productFolders)
            {
                foreach (var packageFolder in GetNonThirdPartyFolders(productFolder))
                {
                    var solutionName = string.Format("Gorba.{0}.{1}.sln", productFolder.Name, packageFolder.Name);
                    var solutionFile = packageFolder.GetFiles(solutionName, SearchOption.TopDirectoryOnly).FirstOrDefault(
                        f => f.Name == solutionName);
                    if (solutionFile != null && solutionFile.Exists)
                    {
                        yield return solutionFile;
                    }
                }
            }
        }
    }
}