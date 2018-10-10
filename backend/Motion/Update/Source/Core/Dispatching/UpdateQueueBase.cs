// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateQueueBase.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateQueueBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Dispatching
{
    using System;
    using System.IO;
    using System.Threading;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;

    using NLog;

    /// <summary>
    /// Base class for both update queues.
    /// </summary>
    internal abstract class UpdateQueueBase
    {
        /// <summary>
        /// The update component.
        /// </summary>
        protected readonly IUpdateComponent Component;

        /// <summary>
        /// The NLog logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly string poolDirectory;

        private bool flushing;

        /// <summary>
        /// The parent queue directory.
        /// </summary>
        private string parentQueueDirectory;

        private bool shouldFlush;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQueueBase"/> class.
        /// </summary>
        /// <param name="component">
        /// The component for which this queue is responsible.
        /// </param>
        /// <param name="queueBaseDirectory">
        /// The base directory for all queues of this type.
        /// </param>
        /// <param name="poolDirectory">
        /// The pool directory.
        /// </param>
        protected UpdateQueueBase(IUpdateComponent component, string queueBaseDirectory, string poolDirectory)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName + "-" + component.Name);

            this.Component = component;
            this.poolDirectory = poolDirectory;
            this.InitializeQueueDirectories(component, queueBaseDirectory);
        }

        /// <summary>
        /// Gets or sets the queue directory for this queue.
        /// </summary>
        protected string QueueDirectory { get; set; }

        /// <summary>
        /// Tells this queue to flush its contents to the update component as soon as possible.
        /// </summary>
        public void Flush()
        {
            this.Logger.Info("Flushing");

            lock (this)
            {
                if (this.flushing)
                {
                    this.shouldFlush = true;
                    return;
                }

                this.flushing = true;
                this.shouldFlush = false;
            }

            ThreadPool.QueueUserWorkItem(s => this.Notify());
        }

        /// <summary>
        /// The initialize queue directories.
        /// </summary>
        /// <param name="component">
        /// The component.
        /// </param>
        /// <param name="queueBaseDirectory">
        /// The queue base directory.
        /// </param>
        public virtual void InitializeQueueDirectories(IUpdateComponent component, string queueBaseDirectory)
        {
            this.QueueDirectory = Path.Combine(queueBaseDirectory, component.Name);

            Directory.CreateDirectory(this.QueueDirectory);

            this.Component.IsAvailableChanged += this.ComponentOnIsAvailableChanged;

            this.parentQueueDirectory = PathManager.Instance.CreatePath(
                FileType.Data,
                "Queues" + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Fires when the update component's (ftp server, flash drive, etc) availability changes.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments
        /// </param>
        protected void ComponentOnIsAvailableChanged(object sender, EventArgs e)
        {
            if (!this.Component.IsAvailable)
            {
                return;
            }

            lock (this)
            {
                if (!this.shouldFlush)
                {
                    return;
                }
            }

            // the component has become available and we have to notify it
            this.Flush();
        }

        /// <summary>
        /// Deletes the file from pool if it is not referenced in the queue directory.
        /// </summary>
        /// <param name="refFileName">
        /// The reference file name.
        /// </param>
        protected void DeleteFileFromPool(string refFileName)
        {
            var hash = this.GetHash(refFileName);

            if (UpdateDispatcher.HasReference(this.parentQueueDirectory, hash))
            {
                return;
            }

            try
            {
                var poolFile = this.GetPoolFile(refFileName);
                File.Delete(poolFile);
                this.Logger.Debug("File deleted from pool: {0}", poolFile);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't delete file from pool File: {0}", refFileName);
            }
        }

        /// <summary>
        /// Subclasses notify the update component about newly arrived data.
        /// </summary>
        protected abstract void DoNotify();

        /// <summary>
        /// Gets the pool file for a given reference file.
        /// </summary>
        /// <param name="refFileName">
        /// The reference file name.
        /// </param>
        /// <returns>
        /// The pool file name.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the pool file couldn't be found.
        /// </exception>
        protected string GetPoolFile(string refFileName)
        {
            var hash = this.GetHash(refFileName);
            foreach (var file in Directory.GetFiles(this.poolDirectory, hash + ".*"))
            {
                return file;
            }

            throw new UpdateException("Couldn't find referenced file from pool: " + refFileName);
        }

        private string GetHash(string refFileName)
        {
            var fileName = Path.GetFileName(refFileName);
            if (fileName == null)
            {
                throw new UpdateException("Couldn't get hash from reference file: " + refFileName);
            }

            var dot = fileName.IndexOf('.');
            return dot < 0 ? fileName : fileName.Substring(0, dot);
        }

        private void Notify()
        {
            while (true)
            {
                if (!this.Component.IsAvailable)
                {
                    this.Logger.Debug("Not available, postponing notification");
                    lock (this)
                    {
                        this.flushing = false;
                        this.shouldFlush = true;
                    }

                    break;
                }

                this.Logger.Debug("Notifying queue");
                try
                {
                    this.DoNotify();
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Exception while notifying");
                    this.shouldFlush = true;
                }

                lock (this)
                {
                    if (!this.shouldFlush)
                    {
                        this.flushing = false;
                        break;
                    }

                    // ok, we got a new flush command while notifying, let's notify again
                    this.shouldFlush = false;
                }
            }
        }
    }
}