// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderStructureTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FolderStructureTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test that just looks at the folder structure of $/Gorba/Main.
    /// </summary>
    [TestClass]
    public class FolderStructureTest : TfsTestBase
    {
        private static readonly string[] AllowedPackageSubfolders =
            {
                "3rdParty", "Build", "Deploy", "Documents",
                "Icons", "Source", "Spikes", "Tools"
            };

        /// <summary>
        /// Checks that in none of the root or product folders there are solution files.
        /// </summary>
        [TestMethod]
        public void TestNoSolutionsInRoots()
        {
            AssertNoSolutionFile(this.LocalMainFolder);

            var folders = this.GetProductFolders();
            foreach (var folder in folders)
            {
                AssertNoSolutionFile(folder);
            }
        }

        /// <summary>
        /// Checks that there is a solution file with the right name in every package folder.
        /// </summary>
        [TestMethod]
        public void TestSolutionsInPackages()
        {
            var productFolders = this.GetProductFolders();
            foreach (var productFolder in productFolders)
            {
                foreach (var packageFolder in TfsTestBase.GetNonThirdPartyFolders(productFolder))
                {
                    var solutionName = string.Format("Gorba.{0}.{1}.sln", productFolder.Name, packageFolder.Name);
                    var solutionFile =
                        packageFolder.GetFiles(solutionName, SearchOption.TopDirectoryOnly)
                            .FirstOrDefault(f => f.Name == solutionName);
                    Assert.IsNotNull(solutionFile, "Package solution file doesn't exist: {0}", solutionName);
                }
            }
        }

        /// <summary>
        /// Checks that in all package folders there are only the allowed sub-folders.
        /// </summary>
        [TestMethod]
        public void TestPackageFoldersHaveOnlyAllowedSubfolders()
        {
            var productFolders = this.GetProductFolders();
            foreach (var productFolder in productFolders)
            {
                foreach (var packageFolder in TfsTestBase.GetNonThirdPartyFolders(productFolder))
                {
                    foreach (var directory in packageFolder.GetDirectories().Where(
                        d => !d.Name.StartsWith("_ReSharper") && d.Name != "TestResults"))
                    {
                        Assert.IsTrue(
                            AllowedPackageSubfolders.Contains(directory.Name),
                            "Folder {0} not allowed inside {1}",
                            directory.Name,
                            packageFolder.FullName);
                    }
                }
            }
        }

        private static void AssertNoSolutionFile(DirectoryInfo directory)
        {
            Assert.IsFalse(HasSolutionFile(directory), "There shouldn't be a solution file in {0}", directory.FullName);
        }

        private static bool HasSolutionFile(DirectoryInfo directory)
        {
            return directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).Any();
        }
    }
}
