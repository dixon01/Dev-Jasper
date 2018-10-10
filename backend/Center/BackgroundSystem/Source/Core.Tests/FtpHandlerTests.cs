namespace Gorba.Center.BackgroundSystem.Core.Tests
{
    using System;
    using System.Diagnostics;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.Ftp;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog;

    [TestClass()]
    public class FtpHandlerTests
    {
        private FtpUpdateClientConfig config;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [TestInitialize]
        public void Initialize()
        {
            this.config = new FtpUpdateClientConfig
            {
                Host = "10.210.1.234",
                Username = "Gorba",
                Password = "Asdf1234",
                RepositoryBasePath = "/"
            };
        }

        [TestMethod()]
        public void ContinueDownloadFileTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void CreateDirectoryTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void DeleteFileTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void DirectoryExistsTest()
        {
            var ftpHandler = new FtpHandler(config);
            var result = ftpHandler.DirectoryExists("Resources");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DownloadFileTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void FileExistsTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void FtpHandlerConstructTest()
        {
            var result = new FtpHandler(this.config);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FtpGetEntries()
        {
            try
            {
                Assert.IsNotNull(this.config);
                var ftpHandler = new FtpHandler(this.config);

                var path = "/";
                var entries = ftpHandler.GetEntries(path);
                Logger.Info( "Check Entries ;{0}", entries )
                ;
              
                Assert.IsNotNull(entries);
                foreach (var item in entries)
                {
                    Debug.WriteLine(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Assert.Fail("!Snap " + e.Message);
            }
        }

        [TestMethod]
        public void FtpRequiredDirectoryExist()
        {
            bool success = true;
            try
            {
                var paths = new[] { "/", "Resources", "Commands", "Feedback" };
                Assert.IsNotNull(this.config);
                var ftpHandler = new FtpHandler(this.config);
                foreach (var path in paths)
                {
                    Debug.WriteLine("Check if Ftp Directory exist Path=" + path);
                    var exists = ftpHandler.DirectoryExists(path);
                    if (!exists)
                    {
                        success = false;
                        Debug.WriteLine("   !Path not found " + path);
                    }
                    else
                    {
                        Debug.WriteLine("   Success found " + path);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Assert.Fail("!Snap " + e.Message);
            }

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void GetFileSizeTest()
        {
            Assert.Inconclusive("TODO ");
        }

        /// <summary>
        /// The initialize.
        /// </summary>
       

        [TestMethod()]
        public void MoveFileTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.Inconclusive("TODO ");
        }

        [TestMethod()]
        public void UploadStreamTest()
        {
            Assert.Inconclusive("TODO ");
        }
    }
}