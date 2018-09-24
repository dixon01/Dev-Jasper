// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandQueue.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Dispatching
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Queue for <see cref="UpdateCommand"/> that are sent to an <see cref="IUpdateSink"/>.
    /// </summary>
    internal class UpdateCommandQueue : UpdateQueueBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandQueue"/> class.
        /// </summary>
        /// <param name="updateSink">
        /// The update sink.
        /// </param>
        /// <param name="queueBaseDirectory">
        /// The base directory for all queues of this type.
        /// </param>
        /// <param name="poolDirectory">
        /// The pool directory.
        /// </param>
        public UpdateCommandQueue(IUpdateSink updateSink, string queueBaseDirectory, string poolDirectory)
            : base(updateSink, queueBaseDirectory, poolDirectory)
        {
            this.UpdateSink = updateSink;
        }

        /// <summary>
        /// Gets the update sink for which this queue is responsible.
        /// </summary>
        public IUpdateSink UpdateSink { get; private set; }

        /// <summary>
        /// Deserializes all update commands from the queue directory and sends them to the update sink.
        /// </summary>
        protected override void DoNotify()
        {
            var commands = new List<UpdateCommand>();
            var refFiles = new List<string>();
            foreach (var commandRefFile in Directory.GetFiles(this.QueueDirectory))
            {
                if (commandRefFile.IndexOf(FileDefinitions.UpdateCommandExtension, StringComparison.InvariantCulture)
                    == 0)
                {
                    continue;
                }

                this.Logger.Trace("Found command reference: {0}", commandRefFile);
                refFiles.Add(commandRefFile);
                var configurator = new Configurator(this.GetPoolFile(commandRefFile));
                var command = configurator.Deserialize<UpdateCommand>();
                commands.Add(command);
            }

            this.Logger.Debug("Found {0} commands", commands.Count);
            if (commands.Count == 0)
            {
                return;
            }

            this.UpdateSink.HandleCommands(commands, null);

            foreach (var commandFile in refFiles)
            {
                Logger.Trace("Deleted command: {0}", commandFile);
                File.Delete(commandFile);
                this.DeleteFileFromPool(commandFile);
            }
        }
    }
}