// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadingFilesState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The DownloadingFilesState state.
    /// </summary>
    public class DownloadingFilesState : State
    {
        private readonly List<FileToDownloader> filesToDownload;

        private bool running;
        private Thread threadDownloader;
        private ManualResetEvent eventStopper;

        private Context stateMachineContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadingFilesState"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="filesToDownload">The files to download.</param>
        public DownloadingFilesState(Context stateMachine, List<FileToDownloader> filesToDownload)
        {
            this.stateMachineContext = stateMachine;
            this.filesToDownload = filesToDownload;
            this.NextStates = new List<string>
                {
                    "CleaningResourcesState",
                    "SendDownloadResponseState",
                    "WaitDownloadRequest",
                    "DownloadingRemaingFilesState"
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadingFilesState"/> class.
        /// </summary>
        /// <param name="stateMachine">The state Machine.</param>
        /// <param name="tripletFileToDwnDatagram">The datagram containing the triplets with the files to download.</param>
        public DownloadingFilesState(Context stateMachine, Triplet tripletFileToDwnDatagram)
        {
            this.running = false;
            this.stateMachineContext = stateMachine;
            this.filesToDownload = new List<FileToDownloader>();

            if (tripletFileToDwnDatagram == null)
            {
                // no files to extract from the CTU datagram
                return;
            }

            // ok, let's extract the files from the CTU datagram.
            if (tripletFileToDwnDatagram.Tag == TagName.DownloadStart)
            {
                var fileTriplet = tripletFileToDwnDatagram as DownloadStart;
                if (fileTriplet != null)
                {
                    var file = new FileToDownloader(
                        this.stateMachineContext.FtpServerIP,
                        fileTriplet.FileAbsPath,
                        fileTriplet.FileSize,
                        fileTriplet.FileCrc);
                    this.filesToDownload.Add(file);
                }
            }
        }

        /// <summary>
        /// Handles the things to do for this state.
        /// </summary>
        /// <param name="stateMachine">
        /// The state machine's context.
        /// </param>
        /// <param name="triplet">
        /// The triplet.
        /// </param>
        public override void Handle(Context stateMachine, Triplet triplet)
        {
            this.stateMachineContext = stateMachine;
            if (!this.running)
            {
                this.Start();
            }

            if (triplet == null)
            {
                // no trigger.
                // I stay in this state.
                return;
            }            

            // in this state we can trigger after a:
            // 1) Download Start (again)
            // 2) Download Progress Request
            // 3) Download Abort
            // 4) << download finished >> event
            // 5) << error >> event
            switch (triplet.Tag)
            {
                case TagName.DownloadAbort:
                    {
                        this.Stop();

                        // now it's the time to go to the CleaningResourcesState
                        var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Abort.");
                        stateMachine.RaiseLogMessageProduced(logEvent);
                        stateMachine.SetNewState(new CleaningResourcesState(this.filesToDownload));
                        stateMachine.Trigger();
                    }

                    break;

                case TagName.DownloadProgressRequest:
                    {
                        // it's the time to go to the SendDownloadResponseState
                        var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Progress Request.");
                        stateMachine.RaiseLogMessageProduced(logEvent);
                        stateMachine.SetNewState(new SendDownloadResponseState(this.filesToDownload));
                        stateMachine.Trigger();
                    }

                    break;

                case TagName.DownloadStart:
                    {
                        var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Start.");
                        stateMachine.RaiseLogMessageProduced(logEvent);
                        this.EnqueueFile(triplet);

                        // after the "append" routine, I've to stay in this state.
                        // so, here I don't update the state machine.
                    }

                    break;

                default:
                    {
                        var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, string.Format("Received {0}. Discarded.", triplet.Tag));
                        stateMachine.RaiseLogMessageProduced(logEvent);
                    }

                    break;
            }
        }

        /// <summary>
        /// Starts the download process of all the files in the list.
        /// </summary>
        protected void Start()
        {
            if (this.running)
            {
                // already started.
                // I avoid to start it twice.
                return;
            }

            bool allError = this.AreAllFilesInError();
            if (allError)
            {
                // nothing to do.
                // just wait a new CTU that can trigger again the state machine.
                return;
            }

            this.eventStopper = new ManualResetEvent(false);
            this.threadDownloader = new Thread(this.Run) { Name = "Th_Downloader", IsBackground = true };
            this.threadDownloader.Start();
            this.running = true;
        }

        /// <summary>
        /// Stops the download
        /// (doesn't delete the files already downloaded).
        /// </summary>
        protected void Stop()
        {
            if (!this.running)
            {
                // already stopped.
                // I avoid to stop it twice.
                return;
            }

            // set event "Stop"
            this.eventStopper.Set();
            this.threadDownloader.Join();
            this.threadDownloader = null;
            this.running = false;
        }

        private void Run()
        {
            int index = 0;
            while (!this.eventStopper.WaitOne(50, true))
            {
                FileToDownloader currentFile;
                lock (this.filesToDownload)
                {
                    currentFile = this.filesToDownload[index];
                    if ((int)currentFile.CurrentStatus < 0 && !currentFile.Notified)
                    {
                        // the current file has encountered an error.
                        this.ManageErrorEvent(currentFile, currentFile.CurrentStatus);
                        continue;
                    }

                    if ((int)currentFile.CurrentStatus < 0)
                    {
                        // I've to take the next one.
                        index = (index + 1) % this.filesToDownload.Count;
                        currentFile = this.filesToDownload[index];
                    }
                }

                bool isThereADownloadRunning = this.IsThereAlreadyARunningDownload();
                if (isThereADownloadRunning)
                {
                    // I've to wait before the completion (with sucess or insucess)
                    // of the running download process. I can only notify the current progress.
                    var progressFileEvent = new DownloadFileNotificationEventArgs(currentFile.AbsName, currentFile.Progress, currentFile.CurrentStatus);
                    this.stateMachineContext.RaiseDownloadFileNotificationProduced(progressFileEvent);
                    continue;
                }

                // ok, I can start the download
                currentFile.Start();
                if (currentFile.BytesRemaining == 0)
                {
                    // download finished for that file.
                    // have I downloaded all the files ?
                    this.ManageDownloadFinishedEvent();

                    // in any case I send another log with the current progress status.
                    var progressFileEvent = new DownloadFileNotificationEventArgs(currentFile.AbsName, currentFile.Progress, currentFile.CurrentStatus);
                    this.stateMachineContext.RaiseDownloadFileNotificationProduced(progressFileEvent);

                    // I've to take the next one.
                    index = (index + 1) % this.filesToDownload.Count;
                    continue;
                }

                DownloadStatusCode statusCode = this.IsErrorOccuredToFile(currentFile);
                if ((int)statusCode < 0 && !currentFile.Notified)
                {
                    this.ManageErrorEvent(currentFile, statusCode);
                    break;
                }

                // for the moment I didn't see error for this file.
                // I've only to wait for its termination (with or whitout success).
                // I can only notify about its progress.
                var downloadFileEvent = new DownloadFileNotificationEventArgs(currentFile.AbsName, currentFile.Progress, currentFile.CurrentStatus);
                this.stateMachineContext.RaiseDownloadFileNotificationProduced(downloadFileEvent);
            }

            // clean-up operations to be placed here
            foreach (var fileDownloader in this.filesToDownload)
            {
                if (fileDownloader.IsRunning)
                {
                    fileDownloader.Stop();
                }
            }

            // inform the main thread that this thread stopped
            this.eventStopper.Set();
        }

        private void ManageDownloadFinishedEvent()
        {
            // download finished for that file.
            // have I downloaded all the files ?
            bool allDownloaded = this.AreAllFilesDownloaded();
            if (allDownloaded)
            {
                // this thread has finished its task. I exit.
                // it's the time to go to the state WaitDownloadRequest
                var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "All files downloaded.");
                this.stateMachineContext.RaiseLogMessageProduced(logEvent);
                this.stateMachineContext.SetNewState(new WaitDownloadRequestState(this.filesToDownload));
                this.running = false;
                this.eventStopper.Set();
            }
        }

        private void ManageErrorEvent(FileToDownloader currentFile, DownloadStatusCode statusCode)
        {
            // during the download of the current file, an error is occured.
            // now it's mandatory to send some CTU log to the TopBox.                    
            var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Error occured.");
            this.stateMachineContext.RaiseLogMessageProduced(logEvent);
            logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Sending error log.");
            this.stateMachineContext.RaiseLogMessageProduced(logEvent);
            this.SendErrorLog(currentFile, statusCode);

            // now it's the time to go to the state DownloadingRemaingFilesState
            currentFile.Notified = true;
            this.stateMachineContext.SetNewState(new DownloadingRemaingFilesState(this.stateMachineContext, this.filesToDownload));
            this.stateMachineContext.Trigger();
            this.running = false;
            this.eventStopper.Set();
        }

        private void EnqueueFile(Triplet triplet)
        {
            if (triplet.Tag == TagName.DownloadStart)
            {
                var fileTriplet = triplet as DownloadStart;
                if (fileTriplet != null)
                {
                    lock (this.filesToDownload)
                    {
                        FileToDownloader oldFile =
                            this.filesToDownload.Find(f => f.AbsName.Equals(fileTriplet.FileAbsPath));
                        if (oldFile != null)
                        {
                            // a file with the same name is alreay in the list.
                            // I avoid to insert it again in the list.
                            return;
                        }

                        // I can insert the new file in the list.
                        var file = new FileToDownloader(
                            this.stateMachineContext.FtpServerIP,
                            fileTriplet.FileAbsPath,
                            fileTriplet.FileSize,
                            fileTriplet.FileCrc);
                        this.filesToDownload.Add(file);
                    }
                }
            }
        }

        private bool IsThereAlreadyARunningDownload()
        {
            lock (this.filesToDownload)
            {
                foreach (var fileToDownloadInfo in this.filesToDownload)
                {
                    if (fileToDownloadInfo.CurrentStatus == DownloadStatusCode.Downloading)
                    {
                        // this file is currently in the downloading status.
                        return true;
                    }
                }
            }

            return false;
        }

        private bool AreAllFilesDownloaded()
        {
            lock (this.filesToDownload)
            {
                foreach (var fileToDownloadInfo in this.filesToDownload)
                {
                    if (fileToDownloadInfo.BytesRemaining != 0)
                    {
                        // this file is not yet completely downloaded.
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AreAllFilesInError()
        {
            lock (this.filesToDownload)
            {
                foreach (var fileToDownloadInfo in this.filesToDownload)
                {
                    if ((int)fileToDownloadInfo.CurrentStatus > 0)
                    {
                        // this file is not in error status.
                        return false;
                    }
                }
            }

            return true;
        }

        private DownloadStatusCode IsErrorOccuredToFile(FileToDownloader currentFile)
        {
            if (ErrorContainer.Instance.FileAbsName.Equals(currentFile.AbsName))
            {
                // to the current download process I've to apply a specific error code.
                currentFile.CurrentStatus = ErrorContainer.Instance.ErrorCode;

                var downloadFileEvent = new DownloadFileNotificationEventArgs(currentFile.AbsName, currentFile.Progress, currentFile.CurrentStatus);
                this.stateMachineContext.RaiseDownloadFileNotificationProduced(downloadFileEvent);
                return currentFile.CurrentStatus;
            }

            if (currentFile.BytesRemaining != 0)
            {
                // I check the socket status
                bool ftpError = currentFile.FtpErrors;
                if (ftpError)
                {
                    currentFile.CurrentStatus = DownloadStatusCode.ConnectionError;
                    return currentFile.CurrentStatus;
                }
            }

            if (currentFile.BytesRemaining == 0)
            {
                // I check about the file's integrity.
                bool isFileCorrupted = currentFile.FileErrors;
                if (isFileCorrupted)
                {
                    currentFile.CurrentStatus = DownloadStatusCode.GeneralError;
                    return currentFile.CurrentStatus;
                }
            }

            // everything appears proceeding without errors.
            return currentFile.CurrentStatus;
        }

        private void SendErrorLog(FileToDownloader fileWithError, DownloadStatusCode errorCode)
        {
            string msg = string.Format("File: {0} failed ({1})", fileWithError.AbsName, errorCode.ToString());
            Triplet logTriplet = new LogMessage(msg);
            var logTripletEvent = new TripletsProducedEventArgs();
            logTripletEvent.Triplets.Add(logTriplet);
            this.stateMachineContext.RaiseTripletsProduced(logTripletEvent);
        }
    }
}
