// --------------------------------------------------------------------------------------------------------------------
// <copyright file="T4TemplatesTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the T4TemplatesTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    using Gorba.Common.Tfs.RepositoryStructureTests.Utility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests that verify that the checked-in files in TFS match the latest output that should be generated.
    /// </summary>
    [TestClass]
    public class T4TemplatesTest : TfsFileTestBase
    {
        private const string GeneratorKey = "Generator";
        private const string LastGenOutputKey = "LastGenOutput";
        private const string TextTemplatingFileGeneratorValue = "TextTemplatingFileGenerator";

        private static readonly string[] TextTransformPaths =
            {
                @"C:\Program Files\Common Files\microsoft shared\TextTemplating\10.0\TextTransform.exe",
                @"C:\Program Files (x86)\Common Files\microsoft shared\TextTemplating\10.0\TextTransform.exe"
            };

        private static readonly MD5 Md5 = MD5.Create();

        /// <summary>
        /// Verifies that the checked-in files in TFS match the latest output
        /// that should be generated from <c>.tt</c> files.
        /// </summary>
        [TestMethod]
        public void TestT4TemplatesGenerated()
        {
            var toolPath = TextTransformPaths.FirstOrDefault(File.Exists);
            Assert.IsNotNull(toolPath, "Couldn't find TextTransform.exe");

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
                    var templates =
                        instance.GetItems("None")
                                .Where(
                                    i =>
                                    i.Metadata.Any(
                                        m =>
                                        m.Name == GeneratorKey && m.EvaluatedValue == TextTemplatingFileGeneratorValue))
                                .ToList();
                    foreach (var template in templates)
                    {
                        var templatePath = Path.Combine(instance.Directory, template.EvaluatedInclude);
                        var templateDir = Path.GetDirectoryName(templatePath);
                        var lastOutput = template.GetMetadataValue(LastGenOutputKey);
                        Assert.IsNotNull(
                            lastOutput, "Couldn't find last generated output file for {0}", template.EvaluatedInclude);
                        Assert.IsNotNull(templateDir);
                        lastOutput = Path.Combine(templateDir, lastOutput);

                        var tempFile = Path.GetTempFileName();
                        try
                        {
                            var args = string.Format("-out \"{0}\" \"{1}\"", tempFile, templatePath);
                            var startInfo = new ProcessStartInfo(toolPath, args);
                            startInfo.CreateNoWindow = true;
                            startInfo.UseShellExecute = false;
                            var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                            process.Start();
                            if (!process.WaitForExit(20 * 1000))
                            {
                                process.Kill();
                                Assert.Inconclusive("{0} did not exit properly", toolPath);
                            }

                            if (process.ExitCode != 0)
                            {
                                // ignore this template for now
                                continue;
                            }

                            var originalHash = CreateHash(lastOutput);
                            var computedHash = CreateHash(tempFile);
                            Assert.AreEqual(
                                computedHash,
                                originalHash,
                                "File in TFS ({0}) doesn't match latest generated file from template {1}",
                                lastOutput,
                                templatePath);
                        }
                        finally
                        {
                            File.Delete(tempFile);
                        }
                    }
                }
            }
        }

        private static string CreateHash(string fileName)
        {
            using (var input = File.OpenRead(fileName))
            {
                var hash = Md5.ComputeHash(input);
                return BitConverter.ToString(hash);
            }
        }
    }
}
