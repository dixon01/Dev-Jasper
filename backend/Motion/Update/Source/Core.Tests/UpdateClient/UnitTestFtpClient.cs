// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UnitTestFtpClient.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------



namespace Gorba.Motion.Update.Core.Tests.UpdateClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The unit test ftp client.</summary>
    [TestClass]
    public class UnitTestFtpClient
    {
        #region Constants

        public const string Password = "Asdf1234";

        public const string UserName = "gorba";

        #endregion

        #region Public Methods and Operators

        /// <summary>Checks if the given directory exists on the FTP server.
        ///     Returns false if the path points to a file.</summary>
        /// <param name="host">The host.</param>
        /// <param name="path">The path.</param>
        /// <param name="userName">The ftp user Name.</param>
        /// <param name="password">The ftp password.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool DirectoryExists(string host, string path, string userName= UserName, string password = Password)
        {
            path = CreateDirectoryPath(path);
            var uri = CreateUri(host, path);

            var request = CreateRequest(uri, userName, password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            try
            {
                HandleResponse(request);
                Debug.WriteLine("Success Found Ftp Folder " + uri);
                Console.WriteLine("Success Found Ftp Folder " + uri);
                return true;
            }
            catch (WebException ex)
            {
                Console.WriteLine("Failed to find Ftp Folder " + uri + " Cause: " + ex.Message);
                Debug.WriteLine("Failed to find Ftp Folder " + uri);
                Debug.WriteLine("Exception " + ex.Message);
                return false;
            }
        }

        /// <summary>The ftp folder_ bogus not exists.</summary>
        [TestMethod]
        public void FtpFolder_BogusNotExists()
        {
            bool found = DirectoryExists("10.210.1.230", "Bogus");
            Assert.IsFalse(found);
        }

        /// <summary>The ftp folder_ commands exists.</summary>
        [TestMethod]
        public void FtpFolder_CommandsExists()
        {
            bool found = DirectoryExists("10.210.1.230", "Commands");
            Assert.IsTrue(found);
        }

        /// <summary>The ftp folder_ commands exists_ swde v 234.</summary>
        [TestMethod]
        public void FtpFolder_CommandsExists_SWDEV234()
        {
            bool found = DirectoryExists("10.210.1.234", "Commands");
            Assert.IsTrue(found);
        }

        /// <summary>The ftp folder_ resources exists.</summary>
        [TestMethod]
        public void FtpFolder_ResourcesExists()
        {
            bool found = DirectoryExists("10.210.1.230", "Resources");
            Assert.IsTrue(found);
        }

        /// <summary>The ftp folder_ resources exists_ swde v 234.</summary>
        [TestMethod]
        public void FtpFolder_ResourcesExists_SWDEV234()
        {
            bool found = DirectoryExists("10.210.1.234", "Resources");
            Assert.IsTrue(found);
        }

        /// <summary>The ftp folder_ root exists.</summary>
        [TestMethod]
        public void FtpFolder_RootExists()
        {
            bool found = DirectoryExists("10.210.1.230", "/");
            Assert.IsTrue(found);
        }

        /// <summary>The ftp folder exists_ swde v 230.</summary>
        [TestMethod]
        public void FtpFolderExists_SWDEV230()
        {
            FtpFolderTest("10.210.1.230");
        }

        /// <summary>The ftp folder_ commands exists_ swde v 234.</summary>
        [TestMethod]
        public void FtpFolderExists_SWDEV234()
        {
            FtpFolderTest("10.210.1.234");

            /*
            ftp log

            2016-06-25 16:26:30 10.210.3.48 - 10.210.1.234 21 ControlChannelOpened - - 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 - 10.210.1.234 21 USER gorba 331 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 PASS *** 230 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 OPTS utf8+on 200 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 PWD - 257 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 CWD Commands 250 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /Commands
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 TYPE I 200 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 PASV - 227 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 57348 DataChannelOpened - - 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 57348 DataChannelClosed - - 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 NLST - 226 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /Commands
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 CWD / 250 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 CWD Feedback 250 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /Feedback
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 PASV - 227 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 57349 DataChannelOpened - - 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 57349 DataChannelClosed - - 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 NLST - 226 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /Feedback
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 CWD / 250 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 CWD Resources 250 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /Resources
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 PASV - 227 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 57350 DataChannelOpened - - 0 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 57350 DataChannelClosed - - 64 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 -
            2016-06-25 16:26:30 10.210.3.48 SWDEV234\Gorba 10.210.1.234 21 NLST - 550 64 0 5242ddb2-95f5-43b1-b19d-6173e04ac6f0 /Resources
            */
        }

        /// <summary>The ftp folder_ commands exists_ swde v 234.</summary>
        /// <param name="host">The host.</param>
        public void FtpFolderTest(string host)
        {
            bool found = DirectoryExists(host, "Commands");
            Assert.IsTrue(found, "Commands folder not found");

            found = DirectoryExists(host, "Feedback");
            Assert.IsTrue(found, "Feedback folder not found");

            for (int i = 0; i < 3; i++)
            {
                found = DirectoryExists(host, "Resources");
                Thread.Sleep(1000);
            }

            Assert.IsTrue(found, "Resources folder not found");
        }

        /// <summary>The list ftp folders_ swde v 230.</summary>
        [TestMethod]
        public void ListFtpFolders_SWDEV230()
        {
            var uriBuilder = new UriBuilder(Uri.UriSchemeFtp, "10.210.1.230", 21);
            var uri = uriBuilder.Uri;
            var folders = GetFtpFolders(uri, UserName, Password);
            foreach (var folder in folders)
            {
                Debug.WriteLine(folder);
            }
        }

        /// <summary>The list ftp folders_ swde v 234.</summary>
        [TestMethod]
        public void ListFtpFolders_SWDEV234()
        {
            var uriBuilder = new UriBuilder(Uri.UriSchemeFtp, "10.210.1.234", 21);
            var uri = uriBuilder.Uri;
            var folders = GetFtpFolders(uri, UserName, Password);
            foreach (var folder in folders)
            {
                Debug.WriteLine(folder);
            }
        }

        #endregion

        #region Methods

        static string NormalizePath(string path)
        {
            path = path.Replace('\\', '/');
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            return path;
        }

        static string CreateDirectoryPath(string path)
        {
            if (!path.EndsWith("\\") && !path.EndsWith("/"))
            {
                path += "/";
            }

            return path;
        }

        static FtpWebRequest CreateRequest(Uri uri, string userName = UserName, string password = Password)
        {
            Debug.WriteLine("WebRequest for " + uri);
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.UseBinary = true;
            request.Credentials = new NetworkCredential(userName, password);
            return request;
        }

        static Uri CreateUri(string host, string path, int port = 21)
        {
            path = NormalizePath(path);
            var uriBuilder = new UriBuilder(Uri.UriSchemeFtp, host, port, path);
            return uriBuilder.Uri;

            // return new Uri(string.Format("ftp://{0}:{1}/%2f{2}", host, port, path));
        }

        private List<string> GetFtpFolders(Uri uri, string userName, string password)
        {
            var ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
            ftpRequest.Credentials = new NetworkCredential(userName, password);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            var response = (FtpWebResponse)ftpRequest.GetResponse();
            var directories = new List<string>();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
            }

            return directories;
        }

        static FtpWebResponse GetResponse(FtpWebRequest request)
        {
            var response = (FtpWebResponse)request.GetResponse();
            string s = string.Format(
                "Request {0} for {1} returned {2}: {3}",
                request.Method,
                request.RequestUri.AbsolutePath,
                response.StatusCode,
                response.StatusDescription);
            Debug.WriteLine(s);
            Console.WriteLine(s);
            return response;
        }

        static void HandleResponse(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            try
            {
                response = GetResponse(request);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                Debug.WriteLine("Close Web Requset");
            }
        }

        #endregion
    }
}