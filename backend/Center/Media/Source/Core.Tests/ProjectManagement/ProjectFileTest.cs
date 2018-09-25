// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectFileTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectFileTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.ProjectManagement
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Unit tests for the <see cref="ProjectFile"/>.
    /// </summary>
    [TestClass]
    public class ProjectFileTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>
        /// The test context.
        /// </value>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            // Setting the local file system to a strict mock object to be sure that no other test is using it
            var localFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(localFileSystem.Object);
        }

        /// <summary>
        /// Test to save a project file and load it back.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        [DeploymentItem(@"ProjectManagement\Resource2.txt")]
        [DeploymentItem(@"ProjectManagement\Resource3.txt")]
        [TestMethod]
        public void SaveAndLoadTwiceWithoutChangesTest()
        {
            // hash for the Resource.txt with the text 'Resource test'
            const string Content1 = "Resource test";
            const string Hash1 = "2B5A90F1D592319548932AB8FCC8010D";

            // hash for the Resource2.txt with the text 'Second test'
            const string Content2 = "Second test";
            const string Hash2 = "7C59D650FF1D5319CC9A3626CD48668D";

            // hash for the Resource3.txt with the text 'Third test'
            const string Content3 = "Third test";
            const string Hash3 = "F4774E7CFCE3018602BD9A4D910E6084";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            var inputPath1 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var testableInputPath1 = CopyToTestFileSystem(localFileSystem, inputPath1);
            var inputPath2 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource2.txt");
            var testableInputPath2 = CopyToTestFileSystem(localFileSystem, inputPath2);
            var inputPath3 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource3.txt");
            var testableInputPath3 = CopyToTestFileSystem(localFileSystem, inputPath3);
            var outputPath = @"C:\ProjectFile.icm";
            var projectFile = ProjectFile.CreateNew(
                outputPath);
            projectFile.MediaProject = new MediaProjectDataModel
                    {
                        Resources =
                            {
                                new ResourceInfo
                                    {
                                        Filename = testableInputPath1.FullName,
                                        Hash = Hash1
                                    }
                            }
                    };
            localResources.AddResource(Hash1, testableInputPath1.FullName, false);
            projectFile.SaveAsync(localResources).Wait();

            var loadedProjectFile = ProjectFile.Load(outputPath);
            Assert.AreEqual(Path.GetFileName(outputPath), loadedProjectFile.FileName);
            Assert.AreEqual(1, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash1, Content1);
            loadedProjectFile.SaveAsync(localResources).Wait();

            loadedProjectFile = ProjectFile.Load(outputPath);
            Assert.AreEqual(Path.GetFileName(outputPath), loadedProjectFile.FileName);
            Assert.AreEqual(1, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash1, Content1);

            loadedProjectFile.MediaProject.Resources.Add(
                new ResourceInfo { Filename = testableInputPath2.FullName, Hash = Hash2 });
            localResources.AddResource(Hash2, testableInputPath2.FullName, false);
            loadedProjectFile.SaveAsync(localResources).Wait();

            loadedProjectFile = ProjectFile.Load(outputPath);
            Assert.AreEqual(Path.GetFileName(outputPath), loadedProjectFile.FileName);
            Assert.AreEqual(2, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash1, Content1);
            VerifyFile(loadedProjectFile, Hash2, Content2);

            loadedProjectFile.MediaProject.Resources.RemoveAt(0);
            loadedProjectFile.MediaProject.Resources.Insert(
                0, new ResourceInfo { Filename = testableInputPath3.FullName, Hash = Hash3 });
            localResources.AddResource(Hash3, testableInputPath3.FullName, false);
            loadedProjectFile.SaveAsync(localResources).Wait();

            loadedProjectFile = ProjectFile.Load(outputPath);
            Assert.AreEqual(Path.GetFileName(outputPath), loadedProjectFile.FileName);
            Assert.AreEqual(2, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash3, Content3);
            VerifyFile(loadedProjectFile, Hash2, Content2);
        }

        /// <summary>
        /// Test to save a project file and load it back.
        /// The MediaProject property is replaced with a new object.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        [DeploymentItem(@"ProjectManagement\Resource2.txt")]
        [TestMethod]
        public void SaveAndLoadTwiceReplacingMediaProjectTest()
        {
            // hash for the Resource.txt with the text 'Resource test'
            const string Content1 = "Resource test";
            const string Hash1 = "2B5A90F1D592319548932AB8FCC8010D";

            // hash for the Resource2.txt with the text 'Second test'
            const string Content2 = "Second test";
            const string Hash2 = "7C59D650FF1D5319CC9A3626CD48668D";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            var inputPath1 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var testableInputPath1 = CopyToTestFileSystem(localFileSystem, inputPath1);
            var inputPath2 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource2.txt");
            var testableInputPath2 = CopyToTestFileSystem(localFileSystem, inputPath2);
            const string OutputPath = @"C:\ProjectFile.icm";
            var projectFile = ProjectFile.CreateNew(
                OutputPath);
            projectFile.MediaProject = new MediaProjectDataModel
                {
                    Resources = { new ResourceInfo { Filename = testableInputPath1.FullName, Hash = Hash1 } }
                };
            localResources.AddResource(Hash1, testableInputPath1.FullName, false);
            projectFile.SaveAsync(localResources).Wait();

            var loadedProjectFile = ProjectFile.Load(OutputPath);
            Assert.AreEqual(Path.GetFileName(OutputPath), loadedProjectFile.FileName);
            Assert.AreEqual(1, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash1, Content1);
            loadedProjectFile.SaveAsync(localResources).Wait();

            loadedProjectFile = ProjectFile.Load(OutputPath);
            Assert.AreEqual(Path.GetFileName(OutputPath), loadedProjectFile.FileName);
            Assert.AreEqual(1, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash1, Content1);

            loadedProjectFile.MediaProject.Resources.Add(
                new ResourceInfo { Filename = testableInputPath2.FullName, Hash = Hash2 });
            localResources.AddResource(Hash2, testableInputPath2.FullName, false);
            loadedProjectFile.SaveAsync(localResources).Wait();

            loadedProjectFile = ProjectFile.Load(OutputPath);
            Assert.AreEqual(Path.GetFileName(OutputPath), loadedProjectFile.FileName);
            Assert.AreEqual(2, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash1, Content1);
            VerifyFile(loadedProjectFile, Hash2, Content2);

            loadedProjectFile.MediaProject = new MediaProjectDataModel
                {
                    Resources = { new ResourceInfo { Filename = testableInputPath1.Name, Hash = Hash2 } }
                };

            loadedProjectFile.SaveAsync(localResources).Wait();

            loadedProjectFile = ProjectFile.Load(OutputPath);
            Assert.AreEqual(Path.GetFileName(OutputPath), loadedProjectFile.FileName);
            Assert.AreEqual(1, loadedProjectFile.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile, Hash2, Content2);
        }

        /// <summary>
        /// Test to save a project file and load it back multiple times, with different names and combinations of
        /// resources.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        [TestMethod]
        [ExpectedException(typeof(UpdateException), AllowDerivedTypes = false)]
        public void GettingNonExistingResourceTest()
        {
            // hash for the Resource.txt with the text 'Resource test'
            const string Hash1 = "2B5A90F1D592319548932AB8FCC8010D";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            const string OutputPath = @"C:\GettingNotExistingResourceTest.icm";

            // Saving the empty file to OutputPath1
            var projectFile = ProjectFile.CreateNew(OutputPath);
            projectFile.MediaProject = new MediaProjectDataModel();
            projectFile.SaveAsync(localResources).Wait();

            var loadedProjectFile1 = ProjectFile.Load(OutputPath);
            loadedProjectFile1.GetResource(Hash1);
        }

        /// <summary>
        /// Test to save a project file without <see cref="ProjectFile.MediaProject"/> property set.
        /// An <see cref="InvalidOperationException"/> is expected.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void SaveProjectFileWithNullMediaProjectPropertyTest()
        {
            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            const string OutputPath = @"C:\SaveProjectFileWithNullMediaProjectPropertyTest.icm";

            // Saving the empty file to OutputPath
            var projectFile = ProjectFile.CreateNew(OutputPath);
            try
            {
                projectFile.SaveAsync(localResources).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
            }
        }

        /// <summary>
        /// Test to save a project file and load it back multiple times, with different names and combinations of
        /// resources.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [DeploymentItem(@"ProjectManagement\Resource2.txt")]
        [DeploymentItem(@"ProjectManagement\Resource3.txt")]
        [TestMethod]
        public void ProjectFileComplexScenarioTest()
        {
            // hash for the Resource.txt with the text 'Resource test'
            const string Hash1 = "2B5A90F1D592319548932AB8FCC8010D";

            // hash for the Resource2.txt with the text 'Second test'
            const string Content2 = "Second test";
            const string Hash2 = "7C59D650FF1D5319CC9A3626CD48668D";

            // hash for the Resource3.txt with the text 'Third test'
            const string Content3 = "Third test";
            const string Hash3 = "F4774E7CFCE3018602BD9A4D910E6084";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            var inputPath2 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource2.txt");
            var testableInputPath2 = CopyToTestFileSystem(localFileSystem, inputPath2);
            var inputPath3 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource3.txt");
            var testableInputPath3 = CopyToTestFileSystem(localFileSystem, inputPath3);
            const string OutputPath1 = @"C:\ProjectFileComplexScenarioTest1.icm";
            const string OutputPath2 = @"C:\ProjectFileComplexScenarioTest2.icm";

            // Saving the empty file to OutputPath1
            var projectFile = ProjectFile.CreateNew(
                OutputPath1);
            projectFile.MediaProject = new MediaProjectDataModel();
            projectFile.SaveAsync(localResources).Wait();

            var loadedProjectFile1 = ProjectFile.Load(OutputPath1);
            Assert.AreEqual(0, loadedProjectFile1.MediaProject.Resources.Count);

            localResources.AddResource(Hash3, testableInputPath3.FullName, false);
            localResources.AddResource(Hash2, testableInputPath2.FullName, false);
            var projectFile2 = ProjectFile.CreateNew(OutputPath2);
            projectFile2.MediaProject = new MediaProjectDataModel
                {
                    Resources =
                        {
                            new ResourceInfo { Filename = "Resource2.txt", Hash = Hash2, Type = ResourceType.Image },
                            new ResourceInfo { Filename = "Resource3.txt", Hash = Hash3, Type = ResourceType.Video }
                        }
                };
            projectFile2.SaveAsync(localResources).Wait();
            var loadedProjectFile2 = ProjectFile.Load(OutputPath2);
            Assert.AreEqual(2, loadedProjectFile2.MediaProject.Resources.Count);
            VerifyFile(loadedProjectFile2, Hash2, Content2);
            VerifyFile(loadedProjectFile2, Hash3, Content3);

            var loadedProjectFile3 = ProjectFile.Load(OutputPath2);
            loadedProjectFile3.MediaProject = new MediaProjectDataModel
                {
                    Resources =
                        {
                            new ResourceInfo { Filename = "Resource.txt", Hash = Hash1, Type = ResourceType.Image }
                        }
                };

            // Expecting UpdateException because the resource with Hash1 was not added to the local resources
            var updateExceptionThrown = false;
            try
            {
                loadedProjectFile3.SaveAsync(localResources).Wait();
            }
            catch (AggregateException e)
            {
                var inner = e.InnerException;
                if (inner != null && inner.GetType() == typeof(UpdateException))
                {
                    updateExceptionThrown = true;
                }
            }

            Assert.IsTrue(updateExceptionThrown, "UpdateException not thrown as expected");

            // Verify that the project file was not corrupted
            VerifyFile(loadedProjectFile2, Hash2, Content2);
            VerifyFile(loadedProjectFile2, Hash3, Content3);
        }

        /// <summary>
        /// Test to load a project file and add an already existing resource. It is expected that the size of the file
        /// doesn't change (avoid resource duplication).
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [DeploymentItem(@"ProjectManagement\ProjectWithResource2AndResource3.icm")]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        [TestMethod]
        public void LoadTestProjectFileAddingNonExistingResourceTest()
        {
            // hash for the Resource.txt with the text 'Resource test'
            const string Content1 = "Resource test";
            const string Hash1 = "2B5A90F1D592319548932AB8FCC8010D";

            // hash for the Resource2.txt with the text 'Second test'
            const string Content2 = "Second test";
            const string Hash2 = "7C59D650FF1D5319CC9A3626CD48668D";

            // hash for the Resource3.txt with the text 'Second test'
            const string Content3 = "Third test";
            const string Hash3 = "F4774E7CFCE3018602BD9A4D910E6084";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            var inputPath1 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var testableInputPath1 = CopyToTestFileSystem(localFileSystem, inputPath1);
            var inputProjectPath = Path.Combine(
                this.TestContext.DeploymentDirectory, "ProjectWithResource2AndResource3.icm");
            var outputFileInfo = CopyToTestFileSystem(localFileSystem, inputProjectPath);
            var projectFile = ProjectFile.Load(outputFileInfo.FullName);
            VerifyFile(projectFile, Hash3, Content3);
            VerifyFile(projectFile, Hash2, Content2);
            projectFile.MediaProject.Resources.Add(new ResourceInfo
                {
                    Filename = testableInputPath1.Name, Hash = Hash1, Type = ResourceType.Video
                });
            localResources.AddResource(Hash1, testableInputPath1.FullName, false);
            projectFile.SaveAsync(localResources).Wait();
            VerifyFile(projectFile, Hash3, Content3);
            VerifyFile(projectFile, Hash2, Content2);
            VerifyFile(projectFile, Hash1, Content1);
        }

        /// <summary>
        /// Test to load a project file and add a non-existing resource. It is expected that the size of the file will
        /// be increased.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [DeploymentItem(@"ProjectManagement\ProjectWithResource2AndResource3.icm")]
        [DeploymentItem(@"ProjectManagement\Resource3.txt")]
        [TestMethod]
        public void LoadTestProjectFileAddingExistingResourceTest()
        {
            // hash for the Resource2.txt with the text 'Second test'
            const string Content2 = "Second test";
            const string Hash2 = "7C59D650FF1D5319CC9A3626CD48668D";

            // hash for the Resource3.txt with the text 'Second test'
            const string Content3 = "Third test";
            const string Hash3 = "F4774E7CFCE3018602BD9A4D910E6084";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            var inputPath3 = Path.Combine(this.TestContext.DeploymentDirectory, "Resource3.txt");
            var testableInputPath3 = CopyToTestFileSystem(localFileSystem, inputPath3);
            var inputProjectPath = Path.Combine(
                this.TestContext.DeploymentDirectory, "ProjectWithResource2AndResource3.icm");
            var outputFileInfo = CopyToTestFileSystem(localFileSystem, inputProjectPath);
            var projectFile = ProjectFile.Load(outputFileInfo.FullName);
            var size = outputFileInfo.Size;
            VerifyFile(projectFile, Hash3, Content3);
            VerifyFile(projectFile, Hash2, Content2);
            projectFile.MediaProject.Resources.Add(new ResourceInfo
                {
                    Filename = testableInputPath3.Name, Hash = Hash3, Type = ResourceType.Video
                });
            projectFile.MediaProject.Resources.Add(new ResourceInfo
                {
                    Filename = testableInputPath3.Name, Hash = Hash3, Type = ResourceType.Video
                });
            projectFile.SaveAsync(localResources).Wait();
            Assert.AreEqual(size, outputFileInfo.Size);
        }

        /// <summary>
        /// Test to load a project file, add a project file, save the project and load it again.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\ProjectWithResource2AndResource3.icm")]
        public void LoadTestProjectAddingInfomediaConfig()
        {
            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);
            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            var inputProjectPath = Path.Combine(
               this.TestContext.DeploymentDirectory, "ProjectWithResource2AndResource3.icm");
            var outputFileInfo = CopyToTestFileSystem(localFileSystem, inputProjectPath);
            var projectFile = ProjectFile.Load(outputFileInfo.FullName);
            projectFile.MediaProject.InfomediaConfig = Helpers.CreateTestInfomediaConfigDataModel();
            projectFile.SaveAsync(localResources).Wait();
            var loadedprojectFile = ProjectFile.Load(outputFileInfo.FullName);
            Assert.IsNotNull(loadedprojectFile.MediaProject.InfomediaConfig);
            var infomediaConfig = loadedprojectFile.MediaProject.InfomediaConfig;
            Assert.AreEqual("Test cycle", infomediaConfig.CyclePackages[0].StandardCycles[0].Reference);
        }

        private static IWritableFileInfo CopyToTestFileSystem(IWritableFileSystem fileSystem, string actualPath)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var path = Path.Combine(@"C:\", Path.GetFileName(actualPath));
            // ReSharper restore AssignNullToNotNullAttribute
            var destinationPathInfo = fileSystem.CreateFile(path);
            using (var destinationStream = destinationPathInfo.OpenWrite())
            {
                using (var sourceStream = File.OpenRead(actualPath))
                {
                    sourceStream.CopyTo(destinationStream);
                }
            }

            return destinationPathInfo;
        }

        private static void VerifyFile(ProjectFile projectFile, string hash, string content)
        {
            var resource = projectFile.GetResource(hash);
            using (var stream = resource.OpenRead())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var storedContent = streamReader.ReadToEnd();
                    Assert.AreEqual(content, storedContent);
                }
            }
        }
    }
}