// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaFtpClient.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles;
    using Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles.Valid;

    /// <summary>
    /// Object tasked to represent the Arriva's FTP client.
    /// Basically, this object creates/renames files on the FTP directory of Protran.
    /// </summary>
    public class ArrivaFtpClient
    {
        /// <summary>
        /// The absolute path of the FTP directory
        /// in which Protran detects the arrival of files having the departures inside.
        /// </summary>
        private readonly string absPathFtpDirectory;

        /// <summary>
        /// List that contains a set of files with departures (good or invalid files).
        /// </summary>
        private readonly List<DepartureFile> departuresFiles;

        private string hostName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaFtpClient"/> class.
        /// </summary>
        /// <param name="absPathFtpDirectory">The absolute path of the FTP directory
        /// in which Protran detects the arrival of files having the departures inside.</param>
        public ArrivaFtpClient(string absPathFtpDirectory)
        {
            this.absPathFtpDirectory = absPathFtpDirectory;
            this.departuresFiles = new List<DepartureFile>();
            this.hostName = Environment.MachineName;
            var integrationTestsAssembly = Assembly.GetExecutingAssembly();
            foreach (Type type in integrationTestsAssembly.GetTypes())
            {
                if (type.Namespace != null &&
                    type.Namespace.StartsWith("Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles."))
                {
                    var depFile = Activator.CreateInstance(type, false) as DepartureFile;
                    if (depFile != null)
                    {
                        this.departuresFiles.Add(depFile);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the name of pc to be used.
        /// </summary>
        /// <param name="name">
        /// The pc name.
        /// </param>
        public void SetHostName(string name)
        {
            this.hostName = name;
        }

        /// <summary>
        /// Starts the Arriva's FTP client.
        /// </summary>
        public void Start()
        {
            // nothing to start, for the moment.
        }

        /// <summary>
        /// Stops the Arriva's FTP client.
        /// </summary>
        public void Stop()
        {
            // nothing to stop, for the moment.
        }

        /// <summary>
        /// Sends a specific departure file.
        /// </summary>
        /// <param name="fileType">The type of the departure file.</param>
        /// <returns>True if the file was sent with success, else false.<see cref="bool"/>.</returns>
        public bool SendDeparturesFile(string fileType)
        {
            DepartureFile depFile = this.departuresFiles.Find(df => df.GetType().Name.Equals(fileType));
            if (depFile == null)
            {
                // invalid file.
                return false;
            }

            if (depFile is ValidDepartureFile)
            {
                // I make sure that its expiration time stays valid,
                // if it's expired.
                DateTime expirationTime = depFile.ExpirationTime;
                DateTime now = TimeProvider.Current.Now;
                if (now > expirationTime)
                {
                    // the expiration time hardcoded in the file is expired.
                    // now I renew it.
                    depFile.ExpirationTime = now.AddHours(1.0);
                }
            }

            // now I apply to the file, the target PC name
            depFile.PcName = this.hostName;
            int attemptsCounter = 0;
            bool cleaned = false;

            string tmpName = string.Format("{0}.{1}", this.absPathFtpDirectory, "new");
            while (attemptsCounter != 10)
            {
                try
                {
                    this.CleanDirectory(tmpName);
                    cleaned = true;
                    break;
                }
                catch (Exception)
                {
                    // I need to retry because Protran can still having
                    // locked the previous departure file.
                    attemptsCounter++;
                    Thread.Sleep(5000);
                }
            }

            if (!cleaned)
            {
                // it was impossible to write the new file in the FTP directory.
                // maybe Protran is still keeping opened the previous file.
                return false;
            }

            this.SendFileToDeparturesDirectory(depFile, tmpName);
            return true;
        }

        /// <summary>
        /// Deletes all the files from the directory tasked to host the departure files.
        /// </summary>
        /// <param name="tempFileName">The temporary name of the file with the departures.
        /// It's required just to have later the "renaming functionality".
        /// </param>
        /// <remarks>This function can throw exception if the delete fails.</remarks>
        private void CleanDirectory(string tempFileName)
        {
            if (File.Exists(this.absPathFtpDirectory))
            {
                File.Delete(this.absPathFtpDirectory);
            }

            // I give some time to the I/O
            Thread.Sleep(5000);

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
        }

        private void SendFileToDeparturesDirectory(DepartureFile departureFile, string tmpName)
        {
            using (var fileStream = new FileStream(tmpName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(departureFile.Content);
                    streamWriter.Flush();
                }
            }

            // the following line of code, actually does
            // the renaming operation required by Protran to understand
            // that a file is arrived.
            File.Move(tmpName, this.absPathFtpDirectory);
        }
    }
}
