// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UpdateClientBase.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Update.ServiceModel.Clients
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Compatibility;

    using NLog;

    /// <summary>Base class for update clients.</summary>
    /// <typeparam name="TConfig">The type of <see cref="UpdateClientConfigBase"/> that is used to configure this class.</typeparam>
    public abstract class UpdateClientBase<TConfig> : UpdateSourceBase, IUpdateClient
        where TConfig : UpdateClientConfigBase
    {
        #region Fields

        private bool running;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the config.
        ///     This property is valid after calling <see cref="Configure" />.
        /// </summary>
        public TConfig Config { get; private set; }

        /// <summary>
        ///     Gets the unique name of this update client.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.Config.Name;
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether config changed.
        /// <see>UpdateConfig</see>
        /// <see>RestoreConfig</see>
        /// </summary>
        protected bool ConfigChanged { get; private set; }

        /// <summary>
        ///     Gets the update context.
        ///     This property is valid after calling <see cref="Configure" />.
        /// </summary>
        protected IUpdateContext Context { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Configures the update provider</summary>
        /// <param name="config">Update provider configuration</param>
        /// <param name="context">The update context.</param>
        public virtual void Configure(UpdateClientConfigBase config, IUpdateContext context)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName + "-" + config.Name);
            this.Config = config as TConfig;
            if (this.Config == null)
            {
                throw new ArgumentException(string.Format("Expected {0} as config", typeof(TConfig).Name), "config");
            }

            this.ConfigChanged = false;
            this.Context = context;
        }

        /// <summary>
        ///     Starts the update client
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.Logger.Debug("Starting");
            this.running = true;
            this.DoStart();
            this.Logger.Info("Started");
        }

        /// <summary>
        ///     Stops the update client
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.Logger.Debug("Stopping");
            this.running = false;
            this.DoStop();
            this.Logger.Info("Stopped");
        }

        /// <summary>
        /// Gets a list of files to upload
        /// </summary>
        public override void UploadFiles()
        {
            var files = this.GetUploadFiles(this.UploadsDirectory);
            this.UploadFiles(files);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Implementation of the <see cref="Start" /> method.
        /// </summary>
        protected abstract void DoStart();

        /// <summary>
        ///     Implementation of the <see cref="Stop" /> method.
        /// </summary>
        protected abstract void DoStop();

        /// <summary>Gets the list of all resource hashes.</summary>
        /// <param name="commands">The commands.</param>
        /// <returns>A dictionary containing the hashes with all <see cref="UpdateCommand"/>s that require that resource.</returns>
        protected Dictionary<string, List<UpdateCommand>> GetResourceHashes(IEnumerable<UpdateCommand> commands)
        {
            var resourceHashes = new Dictionary<string, List<UpdateCommand>>();

            foreach (var command in commands)
            {
                this.AddResourceHashes(command, resourceHashes);
            }

            return resourceHashes;
        }

        /// <summary>Notify the transfer failed status for all commands</summary>
        /// <param name="commands">The commands.</param>
        /// <param name="reason">The reason for the failure.</param>
        protected void NotifyFailedStatus(IEnumerable<UpdateCommand> commands, string reason)
        {
            var updateStateInfos = new List<UpdateStateInfo>();
            foreach (var updateCommand in commands)
            {
                updateStateInfos.Add(this.CreateTransferFailedFeedback(updateCommand, reason));
            }

            this.SendFeedback(new IReceivedLogFile[0], updateStateInfos, null);
        }

        /// <summary>Restore config, Clear ConfigChanged flag</summary>
        /// <see>UpdateConfig</see>
        /// <param name="config">The config.</param>
        protected virtual void RestoreConfig(TConfig config)
        {
            if (config != null)
            {
                this.Logger.Info("Restored Config {0} Changed", config.Name);
                this.Config = config;
                this.ConfigChanged = false;
            }
        }

        /// <summary>Checks if the given <see cref="unitName"/> is handled by any <see cref="IUpdateSink"/>.</summary>
        /// <param name="unitName">The unit name.</param>
        /// <returns>True if at least one update sink handles the given <see cref="unitName"/>.</returns>
        protected bool ShouldDownloadForUnit(string unitName)
        {
            this.Logger.Debug("ShouldDownloadForUnit  - unitName: \'{0}\'", unitName);
            foreach (var updateSink in this.Context.Sinks)
            {
                foreach (var unit in updateSink.HandledUnits)
                {
                    this.Logger.Debug("ShouldDownloadForUnit - Handled Unit: \'{0}\'", unit);
                    if (unit == UnitWildcard || unit.Equals(unitName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.Logger.Debug("ShouldDownloadForUnit - returning TRUE for Unit: \'{0}\'", unit);
                        return true;
                    }
                }
            }
            this.Logger.Debug("ShouldDownloadForUnit - returning False for Unit: \'{0}\'", unitName);
            return false;
        }

        /// <summary>Update the Config, Set the ConfigChanged flag</summary>
        /// <param name="config">The config.</param>
        protected virtual void UpdateConfig(TConfig config)
        {
            if (config != null)
            {
                this.Logger.Info("Updated Config {0} Changed", config.Name);
                this.Config = config;
                this.ConfigChanged = true;
            }
        }

        /// <summary>
        /// Get the list of files to upload
        /// </summary>
        /// <param name="sourceDirectory">
        /// The source directory.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        protected List<IReceivedLogFile> GetUploadFiles(string sourceDirectory)
        {
            var logFiles = new List<IReceivedLogFile>();

            try
            {
                var unitName = ApplicationHelper.MachineName;
                var uploadFiles = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);

                this.Logger.Trace($"Found {uploadFiles.Length} files to upload.");

                foreach (var uploadFile in uploadFiles)
                {
                    var logFile = new FileReceivedLogFile(unitName, uploadFile, uploadFile);
                    logFiles.Add(logFile);
                }
            }
            catch (Exception e)
            {
                this.Logger.Error(e);
            }

            return logFiles;
        }

        private void AddResourceHashes(UpdateCommand command, IDictionary<string, List<UpdateCommand>> hashes)
        {
            foreach (var file in this.GetAllFiles(command))
            {
                this.Logger.Trace("Resource {0} is needed for unit '{1}'", file.Hash, command.UnitId.UnitName);
                List<UpdateCommand> commands;
                if (!hashes.TryGetValue(file.Hash, out commands))
                {
                    commands = new List<UpdateCommand>();
                    hashes.Add(file.Hash, commands);
                }

                commands.Add(command);
            }
        }

        #endregion
    }
}