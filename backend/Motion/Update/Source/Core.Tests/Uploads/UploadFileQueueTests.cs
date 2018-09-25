namespace Gorba.Motion.Update.Core.Tests.Uploads
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Motion.Update.Core.Dispatching;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The tests for UploadFileQueue
    /// </summary>
    [TestClass]
    public class UploadFileQueueTests
    {
        private const string UnitTestUploadFile = UnitTestUploadsFolderRoot + @"\TestFile.txt";

        private const string UnitTestUploadsFolderRoot = @"D:\UnitTests\Uploads";

        // NOTE: This isn't working, I need to figure out how to handle the flushing.
        /// <summary>
        /// Component availability changes should trigger an upload.
        /// For example, if the ftp server is available or not, the usb drive is plugged in or not, etc.
        /// </summary>
        [TestMethod]
        public void ComponentAvailabilityChanges_ShouldCause_FilesToUpload()
        {
            this.CreateTestUploadFiles();
            FakeUpdateSource updateSource = this.CreateFakeUpdateSource();
            var uploadFileQueue = this.CreateUploadFileQueue(updateSource);
            uploadFileQueue.Flush();
            updateSource.TriggerComponentChange();

            Thread.Sleep(1000);

            var filesUploaded = updateSource.FilesToUpload.ToList();
            Assert.IsNotNull(filesUploaded);
            Assert.IsTrue(filesUploaded.Count > 0);
            Assert.AreEqual(Path.GetFileName(UnitTestUploadFile), filesUploaded[0].FileName);
        }

        /// <summary>
        /// Creating an Upload file queue should create the local uploads folder, if it doesn't already exist.
        /// </summary>
        [TestMethod]
        public void UploadFileQueue_Constructor_WillCreate_LocalUploadsDirectory()
        {
            var updateSource = this.CreateFakeUpdateSource();
            if (Directory.Exists(UnitTestUploadsFolderRoot))
            {
                Directory.Delete(UnitTestUploadsFolderRoot, true);
            }

            this.CreateUploadFileQueue(updateSource);

            Assert.IsTrue(Directory.Exists(UnitTestUploadsFolderRoot));
        }

        private FakeUpdateSource CreateFakeUpdateSource(bool isAvailable = true)
        {
            var fakeUpdateSource = new FakeUpdateSource(isAvailable);

            return fakeUpdateSource;
        }

        private void CreateTestUploadFiles()
        {
            string testFileContents = "This Is Test data";
            File.WriteAllText(UnitTestUploadFile, testFileContents);
        }

        private UploadFileQueue CreateUploadFileQueue(
            IUpdateSource updateSource,
            string uploadFolder = UnitTestUploadsFolderRoot)
        {
            var uploadQueue = new UploadFileQueue(updateSource, uploadFolder, @"D:\Pools");
            return uploadQueue;
        }
    }

    /// <summary>
    /// The fake update source.
    /// </summary>
    internal class FakeUpdateSource : IUpdateSource
    {
        private readonly bool isAvailable;

        public FakeUpdateSource(bool isAvailable = true)
        {
            this.isAvailable = isAvailable;
        }

        public event EventHandler<UpdateCommandsEventArgs> CommandsReceived;

        public string UploadsDirectory { get; set; }

        public event EventHandler IsAvailableChanged;

        public IEnumerable<IReceivedLogFile> FilesToUpload { get; set; }

        public bool IsAvailable
        {
            get
            {
                return this.isAvailable;
            }
        }

        public IEnumerable<IReceivedLogFile> LogFiles { get; set; }

        public string Name { get; }

        public IProgressMonitor ProgressMonitor { get; set; }

        public IEnumerable<UpdateStateInfo> StateInfos { get; set; }

        public void SendFeedback(
            IEnumerable<IReceivedLogFile> logFiles,
            IEnumerable<UpdateStateInfo> stateInfos,
            IProgressMonitor progressMonitor)
        {
        }

        public void UploadFiles()
        {
            
        }

        public void TriggerComponentChange()
        {
            this.IsAvailableChanged?.Invoke(this, new EventArgs());
        }

        public void UploadFiles(IEnumerable<IReceivedLogFile> uploadFiles)
        {
            this.FilesToUpload = uploadFiles;
        }
    }
}