// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Providers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Repository;

    using NLog;

    /// <summary>
    /// Base class for <see cref="IUpdateProvider"/> implementation.
    /// </summary>
    /// <typeparam name="TConfig">
    /// The type of <see cref="UpdateProviderConfigBase"/> that is used to configure
    /// this class.
    /// </typeparam>
    public abstract class UpdateProviderBase<TConfig> : UpdateSinkBase, IUpdateProvider
        where TConfig : UpdateProviderConfigBase
    {
        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProviderBase{TConfig}"/> class.
        /// </summary>
        protected UpdateProviderBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the config.
        /// This property is valid after calling <see cref="Configure"/>.
        /// </summary>
        public TConfig Config { get; private set; }

        /// <summary>
        /// Gets the unique name of this provider.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.Config.Name;
            }
        }

        /// <summary>
        /// Gets the update context.
        /// This property is valid after calling <see cref="Configure"/>.
        /// </summary>
        protected IUpdateContext Context { get; private set; }

        /// <summary>
        /// Starts the update provider
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.DoStart();
        }

        /// <summary>
        /// Stops the update provider
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            this.DoStop();
        }

        /// <summary>
        /// Configures the update provider
        /// </summary>
        /// <param name="config">
        /// Update provider configuration
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        public virtual void Configure(UpdateProviderConfigBase config, IUpdateContext context)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName + "-" + config.Name);
            this.Config = config as TConfig;
            if (this.Config == null)
            {
                throw new ArgumentException(string.Format("Expected {0} as config", typeof(TConfig).Name), "config");
            }

            this.Context = context;
        }

        /// <summary>
        /// Implementation of the <see cref="Start"/> method.
        /// </summary>
        protected abstract void DoStart();

        /// <summary>
        /// Implementation of the <see cref="Stop"/> method.
        /// </summary>
        protected abstract void DoStop();

        /// <summary>
        /// Creates a default repository configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="RepositoryConfig"/> valid for the current version of update.
        /// </returns>
        protected virtual RepositoryConfig CreateDefaultRepositoryConfig()
        {
            return new RepositoryConfig
                       {
                           Versions =
                               {
                                   new RepositoryVersionConfig
                                       {
                                           ValidFrom = new Version(1, 0),
                                           Compression = CompressionAlgorithm.None,
                                           ResourceDirectory = "Resources",
                                           CommandsDirectory = "Commands",
                                           FeedbackDirectory = "Feedback"
                                       }
                               }
                       };
        }

        /// <summary>
        /// Gets the hash of all resources required for an update command
        /// (every hash will be exactly once in the list).
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// An enumeration of all resource hashes.
        /// </returns>
        protected IEnumerable<string> GetAllResourceHashes(UpdateCommand command)
        {
            var resources = new Dictionary<string, string>();

            this.GetAllResourceHashes(command, resources);
            return resources.Keys;
        }

        /// <summary>
        /// Notify the transfer failed status for the given command
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="reason">
        /// The reason for the failure.
        /// </param>
        protected void NotifyFailedStatus(UpdateCommand command, string reason)
        {
            var updateStateInfo = new[] { this.CreateTransferFailedFeedback(command, reason) };
            this.RaiseFeedbackReceived(
                new FeedbackEventArgs(new IReceivedLogFile[0], updateStateInfo, new IReceivedLogFile[0]));
        }

        private void GetAllResourceHashes(UpdateCommand command, IDictionary<string, string> resources)
        {
            foreach (var file in this.GetAllFiles(command))
            {
                resources[file.Hash] = file.Hash;
            }
        }
    }
}