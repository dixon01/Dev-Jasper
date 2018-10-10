// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileToDownloader.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Timers;

    using Gorba.Common.Protocols.Ftp;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Common.Utility.Core;

    using Math = System.Math;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// Container of all the information about
    /// the download process of a file.
    /// </summary>
    public class FileToDownloader
    {
        /// <summary>
        /// The IP address of the remote FTP server.
        /// </summary>
        private readonly string serverIP;

        /// <summary>
        /// The port number of the remote FTP server.
        /// </summary>
        private const int ServerPort = 21;

        /// <summary>
        /// The FTP client tasked to download the file.
        /// </summary>
        private FtpClient ftpClient;

        /// <summary>
        /// Flag that tells if an error is occured with the remote FTP server.
        /// </summary>
        private bool ftpErrors;

        /// <summary>
        /// Flag that tells if an error is occured with the file.
        /// </summary>
        private bool fileErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileToDownloader"/> class.
        /// </summary>
        /// <param name="serverIP">The server IP.</param>
        /// <param name="absName">The abs name.</param>
        /// <param name="size">The size.</param>
        /// <param name="crc">The crc.</param>
        public FileToDownloader(string serverIP, string absName, uint size, int crc)
        {
            this.CurrentStatus = DownloadStatusCode.Queued;
            this.AbsName = absName;
            this.Size = size;
            this.Crc = crc;
            this.serverIP = serverIP;
            this.DirectoryForDownloadedFiles = "./CU5_Donwloaded_Files/";
        }

        /// <summary>
        /// Gets the name of the directory that will contain (locally)
        /// the downloaded files.
        /// </summary>
        public string DirectoryForDownloadedFiles { get; private set; }

        /// <summary>
        /// Gets or sets AbsName.
        /// </summary>
        public string AbsName { get; set; }

        /// <summary>
        /// Gets Size.
        /// </summary>
        public uint Size { get; private set; }

        /// <summary>
        /// Gets Crc.
        /// </summary>
        public int Crc { get; private set; }

        /// <summary>
        /// Gets or sets BytesDownloaded.
        /// </summary>
        public long BytesDownloaded { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsRunning.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether Notified.
        /// </summary>
        public bool Notified { get; set; }

        /// <summary>
        /// Gets or sets CurrentStatus.
        /// </summary>
        public DownloadStatusCode CurrentStatus { get; set; }

        /// <summary>
        /// Gets BytesRemaining.
        /// </summary>
        public long BytesRemaining
        {
            get
            {
                long remainingBytes = this.Size - this.BytesDownloaded;
                if (remainingBytes == 0)
                {
                    this.CurrentStatus = DownloadStatusCode.Success;
                }

                return remainingBytes;
            }
        }

        /// <summary>
        /// Gets Progress.
        /// </summary>
        public int Progress
        {
            get
            {
                var progress = (float)((this.BytesDownloaded + 0.0) / (this.Size + 0.0) * 100.0);
                return (int)Math.Round(progress);
            }
        }

        /// <summary>
        /// Gets a value indicating whether are occured some errors with the FTP server
        /// (connection lost, server unreachable, download interruption, etc...)
        /// </summary>
        /// <returns>
        /// True if there are errors with the FTP server, else false.
        /// </returns>
        public bool FtpErrors
        {
            get
            {
                return this.ftpErrors;
            }

            private set
            {
                this.ftpErrors = value;
                if (this.ftpErrors)
                {
                    this.CurrentStatus = DownloadStatusCode.ConnectionError;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are some problems with the file or not
        /// (not writable directory, CRC errors, etc...).
        /// </summary>
        public bool FileErrors
        {
            get
            {
                return this.fileErrors;
            }

            private set
            {
                this.fileErrors = value;
                if (this.fileErrors)
                {
                    this.CurrentStatus = DownloadStatusCode.GeneralError;
                }
            }
        }

        /// <summary>
        /// Starts the file's download process.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning || this.CurrentStatus == DownloadStatusCode.Success)
            {
                // already running or already fully downloaded.
                return;
            }

            // now I start a thread that will try to establish the FTP connection.
            // and from now the file has to be considered as in downloading status.
            this.CurrentStatus = DownloadStatusCode.Downloading;
            this.ftpClient = new FtpClient(this.serverIP, ServerPort, "cu5", "AbuDhabi");
            var threadConnect = new Thread(this.ConnectToFtpServer) { Name = "Th_Connect", IsBackground = true };
            threadConnect.Start();
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops the file's download process.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning)
            {
                // already stopped.
                return;
            }

            // this means that an error was occured with the remote FTP server.
            if (this.ftpClient != null)
            {
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
            }
        }

        /// <summary>
        /// Checks the file's CRC32 code.
        /// </summary>
        /// <param name="fileAbsPath">
        /// The file Abs Path.
        /// </param>
        /// <returns>
        /// True if the file matches well the given CRC code, else false.
        /// </returns>
        private bool ChekCrc(string fileAbsPath)
        {
            var crc32 = new Crc32();
            int calculatedCrc;
            string hash = string.Empty;
           
            using (FileStream fs = File.Open(fileAbsPath, FileMode.Open))
            {
                foreach (byte b in crc32.ComputeHash(fs))
                {
                    hash += b.ToString("x2").ToLower();
                }

                calculatedCrc = int.Parse(hash, NumberStyles.HexNumber);
            }

            return calculatedCrc == this.Crc;
        }

        private void ConnectToFtpServer()
        {
            var timerInActivityListener = new Timer(30000);
            timerInActivityListener.Elapsed += this.OnFtpServerInactivityDetected;
            timerInActivityListener.AutoReset = false;
            timerInActivityListener.Start();

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    this.ftpClient.Connect();
                    if (this.ftpClient.IsConnected)
                    {
                        // ok. connection established.
                        break;
                    }
                }
                catch (Exception)
                {
                    // connection failed.
                    // do nothing. just retry until the maximum number of attempts after some time.
                    Thread.Sleep(2000);
                }
            }

            timerInActivityListener.Stop();
            timerInActivityListener.Dispose();

            if (!this.ftpClient.IsConnected)
            {
                // connection failed for too much time.
                // I cannot do nothing.
                this.FtpErrors = true;
                return;
            }

            // connection established.
            // now I start a thread that will download really the file.
            var threadDownloader = new Thread(this.Download) { Name = "Th_Downloader", IsBackground = true };
            threadDownloader.Start();
        }

        private void Download()
        {
            bool donwloadOk;
            const string TempFileName = "./tmp.tmp";

            // I make sure to have a clean directory before starting the download.
            try
            {
                if (File.Exists(TempFileName))
                {
                    File.Delete(TempFileName);
                }
            }
            catch (Exception)
            {
                // an error is occured on making clear the directory for the download operations.
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
                this.FileErrors = true;
                return;
            }

            try
            {
                donwloadOk = this.ftpClient.OpenDownload(this.AbsName, TempFileName);
            }
            catch (Exception)
            {
                // an error was occured on opening the download.
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
                this.FtpErrors = true;
                return;
            }

            if (!donwloadOk)
            {
                // the download "pipe" has failed during its opening phase.
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
                this.FtpErrors = true;
                return;
            }

            // the file's size is specified in the CTU triplet associated to
            // this object. So, I avoid to request that info to the remote FTP server.
            // now, it's really the time to download the file.
            var timerInActivityListener = new Timer(30000);
            timerInActivityListener.Elapsed += this.OnFtpServerInactivityDetected;
            timerInActivityListener.AutoReset = false;

            int attemptsPartialCounter = 0;
            try
            {
                do
                {
                    timerInActivityListener.Start();
                    long partial = this.ftpClient.DoDownload();
                    if (partial > 0)
                    {
                        timerInActivityListener.Stop();
                    }

                    Thread.Sleep(10);
                    if (partial == 0)
                    {
                        attemptsPartialCounter++;
                    }

                    if (attemptsPartialCounter == 5)
                    {
                        // for too much times the partial is not increased.
                        // I abort the download.
                        timerInActivityListener.Stop();
                        timerInActivityListener.Dispose();
                        this.ftpClient.CloseTransfer();
                        this.ftpClient.Disconnect();
                        this.FtpErrors = true;
                        return;
                    }

                    this.BytesDownloaded += partial;
                }
                while (this.BytesDownloaded != this.Size);
            }
            catch (Exception)
            {
                // an error was occured on getting the file.
                timerInActivityListener.Stop();
                timerInActivityListener.Dispose();
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
                this.FtpErrors = true;
                return;
            }

            if (this.BytesDownloaded != this.Size)
            {
                // an error was occured downloading the file.
                timerInActivityListener.Stop();
                timerInActivityListener.Dispose();
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
                this.FtpErrors = true;
                return;
            }

            // ok, the file was completely download with success.
            timerInActivityListener.Stop();
            timerInActivityListener.Dispose();
            this.ftpClient.CloseTransfer();
            this.ftpClient.Disconnect();
            this.FtpErrors = false;

            // now it's the time to process it.
            // 1) Ensure that exists the directory for the downloaded files.
            if (!Directory.Exists(this.DirectoryForDownloadedFiles))
            {
                try
                {
                    Directory.CreateDirectory(this.DirectoryForDownloadedFiles);
                }
                catch (Exception)
                {
                    // an error was occured on getting the directory for the downloaded files.
                    this.FileErrors = true;
                    File.Delete(TempFileName);
                    return;
                }
            }

            // 2) I've to rename the temporary file to the original file name
            var file = new FileInfo(this.AbsName);
            var localFileName = this.DirectoryForDownloadedFiles + file.Name;
            try
            {
                // I make sure to have a clean destination directory.
                if (File.Exists(localFileName))
                {
                    File.Delete(localFileName);
                }

                File.Move(TempFileName, localFileName);
            }
            catch (Exception)
            {
                // an error was occured on renaming the temporary file.
                File.Delete(TempFileName);
                this.FileErrors = true;
                return;
            }

            // 3) check the CR code.
            bool crcOk = this.ChekCrc(localFileName);
            if (!crcOk)
            {
                // the crc is wrong.
                this.CurrentStatus = DownloadStatusCode.BadCrc;
                return;
            }

            // the file seems correct.
            this.FileErrors = false;
            this.CurrentStatus = DownloadStatusCode.Success;
        }

        private void OnFtpServerInactivityDetected(object sender, ElapsedEventArgs e)
        {
            // timeout elapsed.
            // this means that an error was occured with the remote FTP server.
            if (this.ftpClient != null)
            {
                this.ftpClient.CloseTransfer();
                this.ftpClient.Disconnect();
                this.FtpErrors = true;
            }
        }
    }
}
