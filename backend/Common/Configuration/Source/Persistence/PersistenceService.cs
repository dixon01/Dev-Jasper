// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PersistenceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// <see cref="IPersistenceService"/> implementation. You should always access it
    /// through the interface.
    /// </summary>
    internal class PersistenceService : IPersistenceServiceImpl
    {
        private static readonly Logger Logger = LogHelper.GetLogger<PersistenceService>();

        private ConfigManager<PersistenceContextList> contextsManager;

        private PersistenceContextList config;

        /// <summary>
        /// Event that is fired before all contexts get serialized.
        /// </summary>
        public event EventHandler Saving;

        /// <summary>
        /// Event that is fired after all contexts were serialized.
        /// </summary>
        public event EventHandler Saved;

        /// <summary>
        /// Gets a value indicating whether Enabled.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets the FileName.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or sets the default validity period for newly created contexts.
        /// </summary>
        public TimeSpan DefaultValidity { get; set; }

        /// <summary>
        /// Configures a new instance of <see cref="PersistenceService"/> class.
        /// </summary>
        /// <param name="configFileName">The file Name.</param>
        /// <param name="defaultValidityTime">The default Validity Time.</param>
        /// <param name="enabled">The enabled.</param>
        public void Configure(string configFileName, TimeSpan defaultValidityTime, bool enabled)
        {
            if (this.config != null)
            {
                throw new NotSupportedException("Can't call Configure() twice");
            }

            this.Enabled = enabled;
            this.DefaultValidity =
                defaultValidityTime > TimeSpan.Zero ? defaultValidityTime : TimeSpan.FromMinutes(10);
            this.FileName = configFileName;
            this.InitContextManager();
        }

        /// <summary>
        /// Gets the default context for a given type.
        /// The persistence context is used to store and retrieve the value object
        /// and check its validity.
        /// </summary>
        /// <typeparam name="T">the data type stored in the context.</typeparam>
        /// <returns>the context. If necessary, a new context is created.</returns>
        public IPersistenceContext<T> GetContext<T>() where T : new()
        {
            return this.GetContext<T>(string.Empty);
        }

        /// <summary>
        /// Gets a named context for a given type.
        /// The persistence context is used to store and retrieve the value object
        /// and check its validity.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        /// <typeparam name="T">the data type stored in the context.</typeparam>
        /// <returns>the context. If necessary, a new context is created.</returns>
        public IPersistenceContext<T> GetContext<T>(string name) where T : new()
        {
            if (this.config == null)
            {
                throw new NotSupportedException("GetContext() can only be called after Configure()");
            }

            return (IPersistenceContext<T>)this.config[typeof(T), name];
        }

        /// <summary>
        /// Save the persistence data to the disk.
        /// </summary>
        public void Save()
        {
            if (!this.Enabled)
            {
                // service not allowed to write.
                return;
            }

            Logger.Debug("Saving persistence to {0}", this.contextsManager.FullConfigFileName);

            try
            {
                this.RaiseSaving(EventArgs.Empty);
                this.contextsManager.SaveConfig();
                this.RaiseSaved(EventArgs.Empty);
                Logger.Info("Saved persistence to {0}", this.contextsManager.FullConfigFileName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Could not save persistence, trying again");

                string xmlString = this.contextsManager.ToXmlString();
                using (var fileStream = new FileStream(this.contextsManager.FullConfigFileName, FileMode.Create))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(xmlString);
                        streamWriter.Flush();
                    }
                }

                Logger.Info("Persistence saved with the second attempt.");
            }
        }

        /// <summary>
        /// Raises the <see cref="Saving"/> event.
        /// </summary>
        /// <param name="e">the event arguments.</param>
        protected virtual void RaiseSaving(EventArgs e)
        {
            var handler = this.Saving;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Saved"/> event.
        /// </summary>
        /// <param name="e">the event arguments.</param>
        protected virtual void RaiseSaved(EventArgs e)
        {
            var handler = this.Saved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void InitContextManager()
        {
            if (!this.Enabled)
            {
                // service not enabled.
                this.config = new PersistenceContextList();
                return;
            }

            this.contextsManager = new ConfigManager<PersistenceContextList> { FileName = this.FileName };

            // this will try to load the file, if we fail, just create a new empty list
            try
            {
                new XmlSerializer(typeof(PersistenceContextList));
                this.config = this.contextsManager.Config;
                Logger.Info("Loaded persistence from {0}", this.contextsManager.FullConfigFileName);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not load persistence from {0}", this.contextsManager.FullConfigFileName);
                this.contextsManager.CreateConfig();
                this.config = this.contextsManager.Config;
            }

            this.config.Owner = this;
        }
    }
}