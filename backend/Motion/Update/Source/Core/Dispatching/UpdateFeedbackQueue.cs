// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFeedbackQueue.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateFeedbackQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Dispatching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Queue for update feedback that is sent to an <see cref="IUpdateSource"/>.
    /// </summary>
    internal class UpdateFeedbackQueue : UpdateQueueBase
    {
        private static readonly Regex LogFileReferenceRegex = new Regex(@"\.{(.+)}");

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFeedbackQueue"/> class.
        /// </summary>
        /// <param name="updateSource">
        /// The update source.
        /// </param>
        /// <param name="queueBaseDirectory">
        /// The base directory for all queues of this type.
        /// </param>
        /// <param name="poolDirectory">
        /// The pool directory.
        /// </param>
        public UpdateFeedbackQueue(IUpdateSource updateSource, string queueBaseDirectory, string poolDirectory)
            : base(updateSource, queueBaseDirectory, poolDirectory)
        {
            this.UpdateSource = updateSource;
        }

        /// <summary>
        /// Gets the update source for which this queue is responsible.
        /// </summary>
        public IUpdateSource UpdateSource { get; private set; }

        /// <summary>
        /// Deserializes all feedback from the queue directories and sends them to the update source.
        /// </summary>
        protected override void DoNotify()
        {
            var logFiles = new List<IReceivedLogFile>();
            var stateInfos = new List<UpdateStateInfo>();

            var allRefFiles = new List<string>();
            foreach (var unitDirectory in Directory.GetDirectories(this.QueueDirectory))
            {
                var unitName = Path.GetFileName(unitDirectory);
                var refFiles = Directory.GetFiles(unitDirectory);
                foreach (var refFile in refFiles)
                {
                    if (refFile.IndexOf(FileDefinitions.UpdateStateInfoExtension, StringComparison.CurrentCulture) > 0)
                    {
                        this.Logger.Trace("Found state reference: {0}", refFile);

                        allRefFiles.Add(refFile);
                        var configurator = new Configurator(this.GetPoolFile(refFile));
                        var stateInfo = configurator.Deserialize<UpdateStateInfo>();
                        stateInfos.Add(stateInfo);
                    }
                    else if (refFile.IndexOf(FileDefinitions.LogFileExtension, StringComparison.CurrentCulture) > 0)
                    {
                        this.Logger.Trace("Found log reference: {0}", refFile);

                        allRefFiles.Add(refFile);
                        var match = LogFileReferenceRegex.Match(refFile);
                        var origFileName = match.Success ? match.Groups[1].Value : refFile;

                        var logFile = new FileReceivedLogFile(unitName, origFileName, this.GetPoolFile(refFile));
                        logFiles.Add(logFile);
                    }
                }
            }

            this.Logger.Debug("Found {0} feedback references", allRefFiles.Count);

            if (allRefFiles.Count == 0)
            {
                return;
            }

            this.UpdateSource.SendFeedback(logFiles, stateInfos, null);

            foreach (var refFile in allRefFiles)
            {
                Logger.Trace("Deleted reference: {0}", refFile);
                File.Delete(refFile);
                this.DeleteFileFromPool(refFile);
            }
        }
    }
}