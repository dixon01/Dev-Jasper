// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsmClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Ism
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ftp;
    using Gorba.Motion.Protran.AbuDhabi.Config;

    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// Object tasked to manage all the interactions
    /// with the ISM FTP remote server.
    /// </summary>
    public class IsmClient : IManageableObject
    {
        /// <summary>
        /// The name of the version file that contains the data version.
        /// The file will be created during extraction of files and will contain the
        /// version of the content.
        /// </summary>
        internal static readonly string VersionFileName = "version.txt";

        /// <summary>
        /// The time in milliseconds to wait between attempts to connect to the remote FTP server
        /// </summary>
        private const int AttemptInterval = 500;

        /// <summary>
        /// The time in milliseconds to wait before reconnecting to the remote FTP server after a timeout
        /// </summary>
        private const int ReconnectInterval = 10 * 1000;

        private const string PersistenceName = "IsmPersistence";

        /// <summary>
        /// The temporary file name under which a file from the remote FTP server is downloaded
        /// </summary>
        private const string TempFilename = "TransientFileName";

        private const string TopboxDataDirectory = "Infomedia";

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Container of all the configuration needed
        /// to interact with the remote ISM FTP server.
        /// </summary>
        private readonly IsmConfig ism;

        /// <summary>
        /// Timer to wait for fixed timeout for bytes from socket.
        /// </summary>
        private readonly Timer waitTimer;

        /// <summary>
        /// Timer to poll the remote FTP server for files to download.
        /// </summary>
        private readonly Timer downloadPollTimer;

        /// <summary>
        /// The local directory path where the downloaded files for Protran and Infomedia shall be saved
        /// </summary>
        private readonly string topboxFilesPath;

        /// <summary>
        /// The local directory path where the downloaded files shall be saved
        /// </summary>
        private string localDirectoryPath = @"\ISM\ISM FTP Downloads\";

        /// <summary>
        /// The local directory path where the files shall be unzipped first
        /// </summary>
        private string tempExtractedDirectoryPath = @"\ISM\TempExtractedFiles\";

        /// <summary>
        /// The local directory path where the downloaded files for CU5 shall be saved
        /// </summary>
        private string filesPathForCu5 = @"\ISM\CU5ExtractedFiles\";

        private List<string> localFileList;

        /// <summary>
        /// The number of attempts to connect to the remote FTP server
        /// </summary>
        private int numberOfAttempts;

        /// <summary>
        /// The object tasked to directly interact with
        /// the remote ISM server, using the FTP protocol.
        /// </summary>
        private FtpClient ftpClient;

        private List<string> filesOnFtpSever;

        private bool isFtpClientRunning;

        private bool remoteFtpServerActive;

        private DownloadStatus downloadStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsmClient"/> class.
        /// </summary>
        /// <param name="ism">The container of the ISM configurations.</param>
        public IsmClient(IsmConfig ism)
        {
            if (ism == null)
            {
                throw new ArgumentNullException("ism");
            }

            this.ism = ism;

            this.waitTimer = new Timer { AutoReset = false, Interval = 10000 };
            this.waitTimer.Elapsed += (s, e) => this.ReconnectToFtpSever();

            this.downloadPollTimer = new Timer();
            this.downloadPollTimer.AutoReset = false;
            this.downloadPollTimer.Interval = this.ism.Behaviour.PollTime.TotalMilliseconds;
            this.downloadPollTimer.Elapsed += (s, e) => this.RestartDownloadProcess();

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var ismDirPath = Path.Combine(path ?? string.Empty, "ISM");
            if (Directory.Exists(ismDirPath))
            {
                try
                {
                    this.DeleteDirectory(ismDirPath);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error on deleting local ISM directory: {0}", ismDirPath);
                }
            }

            this.topboxFilesPath = this.ism.Behaviour.Download.DestinationDirectory;
        }

        /// <summary>
        /// Event that is fired whenever the this protocol creates
        /// a new ximple object.
        /// </summary>
        public event EventHandler<FtpStatusEventArgs> FtpDownloading;

        /// <summary>
        /// Starts all the activities with the remote ISM FTP server.
        /// </summary>
        public void Start()
        {
            if (this.remoteFtpServerActive)
            {
                return;
            }

            this.remoteFtpServerActive = true;

            this.isFtpClientRunning = false;
            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            persistenceService.Saving += (s, e) => this.SaveLocalFileList();
            var context = persistenceService.GetContext<List<string>>(PersistenceName);
            if (context.Value != null && context.Valid)
            {
                this.localFileList = context.Value;
            }
            else
            {
                this.localFileList = new List<string>();
            }

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (path != null)
            {
                this.localDirectoryPath = this.localDirectoryPath.Insert(0, path);
                this.filesPathForCu5 = this.filesPathForCu5.Insert(0, path);
                this.tempExtractedDirectoryPath = this.tempExtractedDirectoryPath.Insert(0, path);
            }

            // Before starting the thread tasked to download,
            // Ensure the existance of local directories used to receive and store the remote files.
            if (!CreateDirectory(this.localDirectoryPath))
            {
                return;
            }

            if (!CreateDirectory(this.filesPathForCu5))
            {
                return;
            }

            if (!CreateDirectory(this.topboxFilesPath))
            {
                return;
            }

            this.ftpClient = new FtpClient(
                this.ism.IpAddress,
                this.ism.Port,
                this.ism.Behaviour.Authentication.Login,
                this.ism.Behaviour.Authentication.Password);
            var threadIsmFtpClient = new Thread(this.Run);
            threadIsmFtpClient.IsBackground = true;
            threadIsmFtpClient.Start();
        }

        /// <summary>
        /// Stops all the activities with the remote ISM FTP server.
        /// </summary>
        public void Stop()
        {
            this.remoteFtpServerActive = false;
            this.FtpDisconnect();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("FTP Download Status", this.downloadStatus.ToString(), true);
            foreach (var file in this.localFileList)
            {
                yield return new ManagementProperty<string>("File Downloaded", file, true);
            }
        }

        private static bool CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on creating the directory: {0}");
                return false;
            }
        }

        private void FtpDisconnect()
        {
            if (this.ftpClient == null)
            {
                return;
            }

            this.isFtpClientRunning = false;
            this.ftpClient.CloseTransfer();
            this.ftpClient.Disconnect();
            this.ftpClient = null;
        }

        private void SaveLocalFileList()
        {
            var context =
                ServiceLocator.Current.GetInstance<IPersistenceService>().GetContext<List<string>>(PersistenceName);
            context.Value = this.localFileList;

            // Override default validity for FTP files to 2 years
            context.Validity = TimeSpan.FromDays(730);
        }

        private void ReconnectToFtpSever()
        {
            this.FtpDisconnect();
            Logger.Warn("Connection to remote FTP server with IP address {0} lost", this.ism.IpAddress);
            Thread.Sleep(ReconnectInterval);
            this.ftpClient = new FtpClient(
                this.ism.IpAddress,
                this.ism.Port,
                this.ism.Behaviour.Authentication.Login,
                this.ism.Behaviour.Authentication.Password);
            this.Run();
        }

        private void RestartDownloadProcess()
        {
            this.ftpClient = new FtpClient(
                this.ism.IpAddress,
                this.ism.Port,
                this.ism.Behaviour.Authentication.Login,
                this.ism.Behaviour.Authentication.Password);
            this.Run();
        }

        private void Run()
        {
            this.numberOfAttempts = 0;
            do
            {
                try
                {
                    this.ftpClient.Connect();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, 
                        "Unable to connect to remote FTP server with IP address " + this.ism.IpAddress, ex);
                    this.numberOfAttempts++;
                    Logger.Info("Connect to FtpServer attempt number: {0}", this.numberOfAttempts);
                    if (this.numberOfAttempts >= 3)
                    {
                        Logger.Info("Wait to connect after poll timeout");
                        this.downloadPollTimer.Start();
                        return;
                    }

                    Thread.Sleep(AttemptInterval);
                }
            }
            while (!this.ftpClient.IsConnected);

            this.isFtpClientRunning = true;
            Logger.Info("Connected to remote FTP server with IP address {0} successfully.", this.ism.IpAddress);
            this.DownloadFromServer();
        }

        private void DownloadFromServer()
        {
            try
            {
                this.ftpClient.ChangeDir(this.ism.Behaviour.Download.SourceDirectory);
                var files = this.ftpClient.ListFiles();
                this.filesOnFtpSever = new List<string>();
                foreach (string file in files)
                {
                    // TODO: Change the method "ExtractFileNameFromFileInfo" to a regualr expression
                    string fileName = this.ExtractFileNameFromFileInfo(file);
                    this.filesOnFtpSever.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Unable to get files list from remote FTP server.");
            }

            if (this.filesOnFtpSever != null && this.filesOnFtpSever.Count > 0)
            {
                var filesToDownload = this.FilterFilesForDownload();
                this.DownloadListOfFiles(filesToDownload);
            }
            else
            {
                Logger.Info("There are no files for download on remote FTP server.");
            }

            if (this.isFtpClientRunning)
            {
                this.isFtpClientRunning = false;
                this.FtpDisconnect();
                this.downloadPollTimer.Start();
            }
        }

        private List<string> FilterFilesForDownload()
        {
            // temporary List to hold all the files that need to be downloaded from the server
            var filesToDownload = new List<string>();

            // Check each filename on server agaisnt the localFileList and take only the new files to download
            foreach (var serverFilename in this.filesOnFtpSever)
            {
                if (!this.VerfifyFileExtensions(serverFilename))
                {
                    Logger.Info("File {0} has invalid extension. Not downloaded.", serverFilename);
                    continue;
                }

                if (!this.localFileList.Contains(serverFilename))
                {
                    filesToDownload.Add(serverFilename);
                }
            }

            return filesToDownload;
        }

        private bool VerfifyFileExtensions(string fileInformation)
        {
            var fileExtension = Path.GetExtension(fileInformation);

            // Check the filename for the valid extensions and download
            foreach (var extension in this.ism.Behaviour.Download.CuExtensions)
            {
                if (extension.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            foreach (var extension in this.ism.Behaviour.Download.TopboxExtensions)
            {
                if (extension.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void DownloadListOfFiles(List<string> fileList)
        {
            if (fileList.Count <= 0)
            {
                Logger.Info("There are no new files for download on remote FTP server.");
                return;
            }

            this.RaiseFtpDownloading(new FtpStatusEventArgs { DownloadStatus = DownloadStatus.Running });

            foreach (string filename in fileList)
            {
                File.Delete(TempFilename);

                // what returns to us the FTP "ListFiles" command ?
                // it returns a string having the following set of informartion:
                // XXXXXXXX   XXX   XXX            XXXX  XXXXXXXXXXXX   fileName.extension
                //    /\       /\    /\             /\       /\
                //    ||       ||    ||             ||       ||
                //   code     code  code           Size     Date
                // FileZilla server uses the above format. And the file name is at the end
                // Attention:
                // also the file name can have white spaces in the middle.
                if (this.DownloadFile(filename))
                {
                    // Add the new file names to the local list
                    this.localFileList.Add(filename);
                }
            }

            this.SaveLocalFileList();

            // Indicate that downloading and extraction of files has been completed
            this.RaiseFtpDownloading(new FtpStatusEventArgs { DownloadStatus = DownloadStatus.NotRunning });
        }

        private bool DownloadFile(string actualFilename)
        {
            long currentFileSize = 0;
            long fileSize;
            try
            {
                this.ftpClient.ChangeDir(this.ism.Behaviour.Download.SourceDirectory);
                fileSize = this.TryGetFileSize(actualFilename);

                if (!this.ftpClient.OpenDownload(actualFilename, TempFilename))
                {
                    throw new IOException("Couldn't download " + actualFilename);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Unable to begin downloading from FTP server.");
                this.FtpDisconnect();
                return false;
            }

            // Indicate that downloading has started
            Logger.Info("Currently downloading the file {0}", actualFilename);
            try
            {
                long bytesRead;
                do
                {
                    this.waitTimer.Start();
                    bytesRead = this.ftpClient.DoDownload();
                    currentFileSize += bytesRead;
                    this.waitTimer.Stop();
                }
                while (bytesRead > 0);

                if (currentFileSize != fileSize)
                {
                    throw new IOException("Connection to remote FTP server was lost");
                }

                this.ftpClient.CloseTransfer();

                // not needed with MiniFTP
                //// this.ftpClient.ReadResponse();
                this.ProcessDownloadedFile(TempFilename, actualFilename);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Download failed");
                return false;
            }

            return true;
        }

        private string ExtractFileNameFromFileInfo(string fileInformation)
        {
            // everything after the date/time information, is the file name.
            // the date/time information is recognizable by the character ":".
            // after the ":" there are the minutes expressed by 2 digits.
            // after the minutes there is a white space.
            // after the white space starts the file name.
            int indexColon = fileInformation.LastIndexOf(":", StringComparison.InvariantCultureIgnoreCase);
            if (indexColon < 0)
            {
                // character ":" not found.
                return string.Empty;
            }

            if (fileInformation.Length < indexColon + 2 + 1 + 1)
            {
                // there's no name inside the file's information received.
                return string.Empty;
            }

            string fileName = fileInformation.Substring(indexColon + 2 + 1 + 1);
            return fileName;
        }

        private long TryGetFileSize(string actualFilename)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    long size = this.ftpClient.GetFileSize(actualFilename);
                    if (size >= 0)
                    {
                        return size;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info(ex, "Attempt {0} to get file size failed", i);
                }
            }

            return -1;
        }

        private void ProcessDownloadedFile(string sourceFilename, string destFilename)
        {
            File.Move(sourceFilename, Path.Combine(this.localDirectoryPath, destFilename));

            var storeFilePath = this.GetStoreFilePath(destFilename);

            this.ExtractFiles(this.localDirectoryPath, destFilename);

            // if there is an "Infomedia" directory in the extracted directory, add the version.txt file
            var topboxDataPath = Path.Combine(this.tempExtractedDirectoryPath, TopboxDataDirectory);
            if (Directory.Exists(topboxDataPath))
            {
                var versionFilePath = Path.Combine(topboxDataPath, VersionFileName);
                var versionContent = Path.GetFileName(destFilename);
                Logger.Debug("Writing version file: '{0}' with content: '{1}'", versionFilePath, versionContent);
                using (var writer = File.CreateText(versionFilePath))
                {
                    writer.Write(versionContent);
                }
            }

            this.MoveFilesToDestination(storeFilePath);
            Logger.Info("Downloading and extraction finished for file: {0}", destFilename);

            this.DeleteDirectory(this.tempExtractedDirectoryPath);
        }

        private string GetStoreFilePath(string destFilename)
        {
            string fileExtension = Path.GetExtension(destFilename);
            if (fileExtension == null)
            {
                return string.Empty;
            }

            foreach (var ext in this.ism.Behaviour.Download.CuExtensions)
            {
                if (fileExtension.Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.filesPathForCu5;
                }
            }

            foreach (var ext in this.ism.Behaviour.Download.TopboxExtensions)
            {
                if (fileExtension.Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.topboxFilesPath;
                }
            }

            return string.Empty;
        }

        private void DeleteDirectory(string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);
            this.RemoveReadOnlyfromFiles(dir);
            dir.Delete(true);
        }

        private void RemoveReadOnlyfromFiles(DirectoryInfo directory)
        {
            foreach (var dir in directory.GetDirectories())
            {
                this.RemoveReadOnlyfromFiles(dir);
            }

            foreach (var file in directory.GetFiles())
            {
                if (file.IsReadOnly)
                {
                    file.IsReadOnly = false;
                }
            }
        }

        private void MoveFilesToDestination(string storeFilePath)
        {
            this.CopyToDestination(this.tempExtractedDirectoryPath, storeFilePath);
        }

        private void CopyToDestination(string sourcePath, string destPath)
        {
            var dir = new DirectoryInfo(sourcePath);
            var dirs = dir.GetDirectories();

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            // Get the file contents of the directory to copy.
            var files = dir.GetFiles();

            foreach (var file in files)
            {
                var temppath = Path.Combine(destPath, file.Name);

                file.CopyTo(temppath, false);
            }

            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destPath, subdir.Name);

                this.CopyToDestination(subdir.FullName, temppath);
            }
        }

        private void ExtractFiles(string filePath, string fileName)
        {
            var zip = new FastZip();
            zip.ExtractZip(Path.Combine(filePath, fileName), this.tempExtractedDirectoryPath, null);
        }

        private void RaiseFtpDownloading(FtpStatusEventArgs e)
        {
            this.downloadStatus = e.DownloadStatus;

            var handler = this.FtpDownloading;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
