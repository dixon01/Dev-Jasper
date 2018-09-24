namespace Gorba.Center.BackgroundSystem.Core.Tests.Update
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The unit upload file manager tests.
    /// </summary>
    [TestClass]
    public class UnitUploadFileManagerTests
    {
        /// <summary>
        /// Initializes the test resetting the <see cref="DependencyResolver"/> and the services.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            DependencyResolver.Reset();
        }

        /// <summary>
        /// This should not throw an exception, we're testing that SaveUploadFileAsync does not rethrow.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveUploadFileAsync_Will_HandleFileExceptions()
        {
            var fileSystem = new Mock<IWritableFileSystem>();
            var uploadedFile = @"C:\InvalidDirectory\InvalidSub\Uploaded.txt";
            fileSystem.Setup(f => f.CreateFile(uploadedFile)).Throws<DirectoryNotFoundException>();

            UnitUploadFileManager manager = new UnitUploadFileManager(fileSystem.Object);

            string contents = "Test Data";
            this.SaveUploadedFile(manager, uploadedFile, contents);
        }

        [TestMethod]
        public async Task SaveUploadFileAsync_Will_IgnoreEmptyContents()
        {
            TestingFileSystem fileSystem = new TestingFileSystem();
            UnitUploadFileManager manager = new UnitUploadFileManager(fileSystem);

            var uploadedFile = @"C:\Uploaded.txt";
            this.SaveUploadedFile(manager, uploadedFile, string.Empty);

            var bytes = this.GetUploadedBytes(fileSystem, uploadedFile);

            Assert.IsFalse(fileSystem.ItemExists(uploadedFile));
            Assert.IsTrue(bytes.Length == 0);
        }

        [TestMethod]
        public void SaveUploadFileAsync_Will_WriteFileToFileSystem()
        {
            TestingFileSystem fileSystem = new TestingFileSystem();
            UnitUploadFileManager manager = new UnitUploadFileManager(fileSystem);

            var uploadedFile = @"C:\Uploaded.txt";
            var uploadedContents = "1234567";
            this.SaveUploadedFile(manager, uploadedFile, uploadedContents);

            var bytes = this.GetUploadedBytes(fileSystem, uploadedFile);

            Assert.IsTrue(fileSystem.ItemExists(uploadedFile));
            Assert.IsTrue(bytes.Length == uploadedContents.Length);
        }

        private byte[] GetUploadedBytes(TestingFileSystem fileSystem, string uploadedFile)
        {
            if (!fileSystem.ItemExists(uploadedFile))
            {
                return new byte[] { };
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var fileInfo = fileSystem.GetFile(uploadedFile);
                using (var fileStream = fileInfo.OpenRead())
                {
                    fileStream.CopyTo(memoryStream);
                }
                return memoryStream.ToArray();
            }
        }

        private async void SaveUploadedFile(UnitUploadFileManager manager, string uploadedFile, string uploadedContents)
        {
            var contents = Encoding.UTF8.GetBytes(uploadedContents);
            var stream = new MemoryStream(contents);
            await manager.SaveUploadFileAsync(uploadedFile, stream);
        }
    }
}