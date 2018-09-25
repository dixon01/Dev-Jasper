// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpUpdateProviderTests.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpUpdateProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Update
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.Ftp;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Utility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class FtpUpdateProviderTests
    {
        
        [TestMethod]
        public void DownloadUploadedFiles_IntegrationTest_WillDownloadATestUploadedFile()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var testFilePath = @"\Uploads\TFT-ZZ-ZZ-ZZ\MyTestFile.csv";
            List<string> tempFiles = new List<string>();
            var tempDirectory = @"D:\Temp";

            var ftpHandler = UploadTestFileToFTPServer(testFilePath);

            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler, new NullProgressMonitor());
            provider.DownloadUploadedFiles(uploadedFiles, CompressionAlgorithm.None, new NullProgressMonitor(), ftpHandler, tempFiles, tempDirectory);

            ftpHandler.DeleteFile(testFilePath);

            foreach (var uploadedFile in uploadedFiles)
            {
                string tempPath = tempDirectory + uploadedFile.Value + FileDefinitions.TempFileExtension;
                Assert.IsTrue(File.Exists(tempPath));
                File.Delete(tempPath);
            }
        }

        [TestMethod]
        public void DownloadUploadedFiles_WillDownloadUploadedFilesOnTheFTPServer_IntoTempFiles()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());
            var mockProgress = new Mock<IProgressMonitor>();
            var tempDirectory = @"D:\temp";

            var tempFiles = new List<string>();
            var receivedFiles = provider.DownloadUploadedFiles(uploadedFiles, CompressionAlgorithm.None, mockProgress.Object, ftpHandler.Object, tempFiles, tempDirectory);

            Assert.AreEqual(uploadedFiles.Count, receivedFiles.Count);

            foreach (var uploadedFile in uploadedFiles)
            {
                string tempPath = tempDirectory + uploadedFile.Value + FileDefinitions.TempFileExtension;
                // Verify that tempfiles contains the uploaded file name with a temp extension
                Assert.IsTrue(tempFiles.Exists(f => f == tempPath));

                // Verify that we asked the ftp handler to download the file from the remote path to the local temp file name.
                ftpHandler.Verify(ftp => ftp.DownloadFile(uploadedFile.Value, tempPath, It.IsAny<long>(), It.IsAny<IPartProgressMonitor>()));
            }
        }

        [TestMethod]
        public void DownloadUploadedFiles_WillReturn_ReceivedFiles_ThatMatch_UploadedFiles()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());
            var mockProgress = new Mock<IProgressMonitor>();

            var tempFiles = new List<string>();
            var receivedFiles = provider.DownloadUploadedFiles(uploadedFiles, CompressionAlgorithm.None, mockProgress.Object, ftpHandler.Object, tempFiles, @"D:\temp");

            Assert.AreEqual(uploadedFiles.Count, receivedFiles.Count);

            // Verify that we have a received file for each uploaded file.
            foreach (var uploadedFile in uploadedFiles)
            {
                Assert.IsTrue(receivedFiles.Exists(f => f.FileName == Path.GetFileName(uploadedFile.Value)));
            }
        }

        [TestMethod]
        public void DownloadUploadedFiles_WillShowProgress_ForReceivedFiles()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var mockProgress = new Mock<IProgressMonitor>();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var tempFiles = new List<string>();

            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());
            var receivedFiles = provider.DownloadUploadedFiles(uploadedFiles, CompressionAlgorithm.None, mockProgress.Object, ftpHandler.Object, tempFiles, @"D:\temp");

            Assert.AreEqual(uploadedFiles.Count, receivedFiles.Count);

            for (int i = 0; i < receivedFiles.Count; i++)
            {
                var fileName = Path.GetFileName(uploadedFiles[i].Value);
                var progress = (i + 1) / (uploadedFiles.Count + 1.0);
                mockProgress.Verify(p => p.Progress(progress, fileName));
            }
        }

        [TestMethod]
        public void DownloadUploadedFiles_WillPreserve_OriginalFileDirectoryStructure()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var mockProgress = new Mock<IProgressMonitor>();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var tempFiles = new List<string>();
            var tempDirectory = @"D:\temp";

            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());
            var receivedFiles = provider.DownloadUploadedFiles(uploadedFiles, CompressionAlgorithm.None, mockProgress.Object, ftpHandler.Object, tempFiles, tempDirectory);

            Assert.AreEqual(uploadedFiles.Count, receivedFiles.Count);

            for (int i = 0; i < receivedFiles.Count; i++)
            {
                var fileName = uploadedFiles[i].Value;
                string receivedPath = ((FileReceivedLogFile)receivedFiles[i]).FilePath;
                Assert.AreEqual(tempDirectory + fileName + FileDefinitions.TempFileExtension, receivedPath);
            }
        }

        /// <summary>
        /// Place a single file in a test TFT folder, and verify FindUploadFiles will find the file.
        /// </summary>
        [TestMethod]
        public void FindUploadedFiles_Integration_Test_WillFind_SinglePlantedFile()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var testFilePath = @"\Uploads\TFT-ZZ-ZZ-ZZ\MyTestFile.csv";

            var ftpHandler = UploadTestFileToFTPServer(testFilePath);

            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler, new NullProgressMonitor());

            ftpHandler.DeleteFile(testFilePath);

            Assert.IsTrue(uploadedFiles.Any(f => f.Key == "TFT-ZZ-ZZ-ZZ"));
            Assert.IsTrue(uploadedFiles.Any(f => f.Value == testFilePath));
        }

        [TestMethod]
        public void FindUploadedFiles_WillFind_TFTFilesToUpload()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());

            Assert.AreEqual(2, uploadedFiles.Count);
        }

        [TestMethod]
        public void FindUploadedFiles_WillIgnore_DirectoriesInATFTFolder()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());

            foreach (var file in uploadedFiles)
            {
                Assert.IsFalse(file.Value.Contains("AnotherInvalidFile.csv"));
            }
        }

        [TestMethod]
        public void FindUploadedFiles_WillIgnore_FilesThatAreNotIn_ATFTFolder()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());

            foreach (var file in uploadedFiles)
            {
                Assert.IsFalse(file.Value.Contains("InvalidFile.csv"));
            }
        }

        [TestMethod]
        public void FindUploadedFiles_WillSubstitute_BackSlashesForForwardSlashes()
        {
            FtpUpdateProvider provider = new FtpUpdateProvider();
            var ftpHandler = this.CreateMockFTPHandlerWithSimpleDirectoryStructure();
            var uploadedFiles = provider.FindUploadedFiles(@"\Uploads", ftpHandler.Object, new NullProgressMonitor());

            Assert.IsFalse(uploadedFiles.Any(f => f.Value.Contains(@"/")));
            Assert.IsTrue(uploadedFiles.Any(f => f.Value.Contains(@"\Uploads\TFT-01-02-03\TestFile.csv")));
        }

        private static void UploadTestFile(FtpHandler ftpHandler, string filePath, string testData)
        {
            // Define a test file, with test data, and send it to the ftp server.
            if (ftpHandler.FileExists(filePath))
            {
                ftpHandler.DeleteFile(filePath);
            }

            ftpHandler.CreateDirectory(@"\Uploads\TFT-ZZ-ZZ-ZZ", true);
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                ftpHandler.UploadStream(memoryStream, filePath, testData.Length, new NullProgressMonitor());
            }
        }

        private static FtpHandler UploadTestFileToFTPServer(string testFilePath)
        {
            // This is the current dev ftp server.
            var config = new FtpUpdateProviderConfig { Host = @"swdevicntr.luminatorusa.com", Username = @"FTPuser", Password = "Luminator1" };

            var ftpHandler = new FtpHandler(config);

            UploadTestFile(ftpHandler, testFilePath, "Test IS Test Data");
            return ftpHandler;
        }

        private Mock<IFtpHandler> CreateMockFTPHandlerWithSimpleDirectoryStructure()
        {
            var mockFtpHandler = new Mock<IFtpHandler>();

            // Create this test FTP directory structure. Invalid files and directories should be ignored.
            // Uploads
            // InvalidFile.csv
            // TFT-01-02-03
            // TestFile.csv
            // TestSub
            // TFT-AA-BB-CC
            // AATestFile.csv
            // InvalidDirectory
            // AnotherInvalidFile.csv
            this.GetEntriesReturns(mockFtpHandler, @"\Uploads", "TFT-01-02-03", "TestSub", "InvalidFile.csv");
            this.GetEntriesReturns(mockFtpHandler, @"\Uploads/TFT-01-02-03", "TestFile.csv");
            this.GetEntriesReturns(mockFtpHandler, @"\Uploads/TestSub", "TFT-AA-BB-CC");
            this.GetEntriesReturns(mockFtpHandler, @"\Uploads/TestSub/TFT-AA-BB-CC", "AATestFile.csv", "InvalidDirectory");
            this.GetEntriesReturns(mockFtpHandler, @"\Uploads/TestSub/TFT-AA-BB-CC/InvalidDirectory", "AnotherInvalidFile.csv");

            this.DirectoryExists(mockFtpHandler, @"\Uploads/TFT-01-02-03");
            this.DirectoryExists(mockFtpHandler, @"\Uploads/TestSub");
            this.DirectoryExists(mockFtpHandler, @"\Uploads/TestSub/TFT-AA-BB-CC");

            this.FileExists(mockFtpHandler, @"\Uploads/TestSub/TFT-AA-BB-CC/AATestFile.csv");
            this.FileExists(mockFtpHandler, @"\Uploads/TFT-01-02-03/TestFile.csv");

            return mockFtpHandler;
        }

        private void DirectoryExists(Mock<IFtpHandler> mock, string path, bool exists = true)
        {
            mock.Setup(m => m.DirectoryExists(path)).Returns(exists);
        }

        private void FileExists(Mock<IFtpHandler> mock, string path, bool exists = true)
        {
            mock.Setup(m => m.FileExists(path)).Returns(exists);
        }

        private void GetEntriesReturns(Mock<IFtpHandler> mock, string directory, params string[] entriesInDirectory)
        {
            for (var index = 0; index < entriesInDirectory.Length; index++)
            {
                var entry = entriesInDirectory[index];
                entriesInDirectory[index] = directory + @"/" + entry;
            }

            mock.Setup(m => m.GetEntries(directory)).Returns(entriesInDirectory);
        }
    }
}