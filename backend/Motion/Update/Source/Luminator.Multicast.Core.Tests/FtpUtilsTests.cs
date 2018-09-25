using Microsoft.VisualStudio.TestTools.UnitTesting;
using Luminator.Multicast.Core;

namespace Luminator.Multicast.Core.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FtpUtilsTests
    {
        #region Public Methods and Operators
        [Ignore]
        [TestMethod]
        public void FtpFileDeleteTest()
        {
            var ftpUtils = new FtpUtils("ftp://swdev230/", "Gorba", "Asdf1234");
            var result = ftpUtils.FtpFileUpload("test.zip", "testing.zip");
            result = ftpUtils.FtpFileExists(new Uri("ftp://swdev230/testing.zip"));
            Assert.AreEqual(result, true);
            result = ftpUtils.FtpFileDelete(new Uri("ftp://swdev230/testing.zip"));
            Assert.AreEqual(result, true);
            result = ftpUtils.FtpFileExists(new Uri("ftp://swdev230/testing.zip"));
            Assert.AreEqual(result, false);
        }

        [Ignore]
        [TestMethod]
        public void FtpFileExistsTest()
        {
            var ftpUtils = new FtpUtils("ftp://swdev230/", "Gorba", "Asdf1234");
            var result = ftpUtils.FtpFileExists(new Uri("ftp://swdev230/testing.zip"));
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        [DeploymentItem("Resources\\facade.txt")]
        public void FtpFileUpEmptyFileTest()
        {
            var ftpUtils = new FtpUtils("ftp://swdev230/", "Gorba", "Asdf1234");
            ftpUtils.FtpFileUpload("facade.txt", "connected.txt");
        }

        [TestMethod]
        public void FtpFileUpTest()
        {
            var ftpUtils = new FtpUtils("ftp://swdev230/", "Gorba", "Asdf1234");
            ftpUtils.FtpFileUpload("test.zip", "Testing.zip");
        }

        [TestMethod]
        public void FtpCreateSlaveFileAtFtpTest()
        {
            var ftpUtils = new FtpUtils("ftp://swdev230/", "Gorba", "Asdf1234");
            var result = ftpUtils.CreateSlaveFile("ftp://swdev230/", "Gorba", "Asdf1234");
            Assert.IsTrue(result, " Ftp Creation of Slave File Failed.");
        }



        [TestMethod]

        public void DeleteAllSlaveFiles()
        {
            var ftpUtils = new FtpUtils("ftp://swdev230/", "Gorba", "Asdf1234");
            var result = ftpUtils.DeleteAllSlaveFiles("ftp://swdev230/", "Gorba", "Asdf1234");
            Assert.IsTrue(result, " Ftp Delete of Slave Files Failed.");
        }

        [Ignore]
        [TestMethod]
        public void FtpSendFileAndGetCompletionEventTest()
        {
            var waiter = new AutoResetEvent(false);
            var client = new WebClient();
            client.Credentials = new NetworkCredential("Gorba", "Asdf1234");
            var uri = new Uri("ftp://swdev230/" + Guid.NewGuid() + ".zip");

            // Specify that that UploadFileCallback method gets called
            // when the file upload completes.
            client.UploadFileCompleted += UploadFileCallback;

            client.UploadFileAsync(uri, WebRequestMethods.Ftp.UploadFile, "test.zip", waiter);

            // Block the main application thread. Real applications
            // can perform other tasks while waiting for the upload to complete.
            waiter.WaitOne();
            Console.WriteLine("File upload is complete.");
        }

        [TestMethod]
        public void IsValidConnectionTest()
        {
            var ftpUtils = new FtpUtils();
            var result = ftpUtils.IsValidConnection("ftp://swdev230/");
            Assert.AreEqual(result, true);
        }

        #endregion

        #region Methods

        private static void UploadFileCallback(object sender, UploadFileCompletedEventArgs e)
        {
            Console.WriteLine("In UploadFileCallback " + Guid.NewGuid());
            var waiter = (AutoResetEvent)e.UserState;
            try
            {
                var reply = Encoding.UTF8.GetString(e.Result);
                Console.WriteLine(reply);
            }
            finally
            {
                // If this thread throws an exception, make sure that
                // you let the main application thread resume.
                waiter.Set();
            }
        }

        #endregion

        [TestMethod()]
        public void CreateTempFileTest()
        {
            var ftpUtils = new FtpUtils();
            Console.WriteLine($"Current Directory is {Directory.GetCurrentDirectory()}");
            var result = ftpUtils.CreateTempFile("temp.txt");
            Assert.IsTrue(result, "Could not create temporary file");

        }
    }
}