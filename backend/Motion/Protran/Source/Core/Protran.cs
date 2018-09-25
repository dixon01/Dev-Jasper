// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Protran.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Protran type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Configuration.Protran.XimpleProtocol;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Protocols;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Main class of this library. Creating and starting this object
    /// will give the "life" to the whole translation engine.
    /// If you want to stop this engine, you have to dispose this object.
    /// If you want to re-start this engine, you have to dispose this object,
    /// re-create it and than re-start it.
    /// This class is thread safe. Each public method is protected by a semaphore.
    /// </summary>
    public class Protran : IDisposable, IManageableObject
    {
        /// <summary>
        /// The management name.
        /// </summary>
        internal static readonly string ManagementName = "Protran";

        private static readonly Logger Logger = LogHelper.GetLogger<Protran>();

        /// <summary>
        /// The container of all the dictionary' entries.
        /// </summary>
        private readonly ConfigManager<Dictionary> dictionaryLoader;

        /// <summary>
        /// The container of all the configurations.
        /// </summary>
        private ConfigLoader cfgLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="Protran"/> class.
        /// This Protran instance will have a null arguments options.
        /// </summary>
        public Protran()
        {
            // prepare service container stuff
            SetupCoreServices();

            this.dictionaryLoader = new ConfigManager<Dictionary>();
            this.dictionaryLoader.XmlSchema = Dictionary.Schema;
            this.dictionaryLoader.FileName = PathManager.Instance.GetPath(FileType.Config, "dictionary.xml");
            this.dictionaryLoader.EnableCaching = true;

            // ok. Now it's the time to acquire some information
            // so, let's start to load the config file...
            if (!this.LoadConfigurations())
            {
                // configuration file not loaded.
                return;
            }

            // configure the persistence service according to our settings
            var persistenceService =
                ServiceLocator.Current.GetInstance<IPersistenceService>() as IPersistenceServiceImpl;
            if (persistenceService != null)
            {
                persistenceService.Configure(
                    PathManager.Instance.CreatePath(
                        FileType.Data, this.cfgLoader.ProtranConfig.Persistence.PersistenceFile),
                    this.cfgLoader.ProtranConfig.Persistence.DefaultValidity,
                    this.cfgLoader.ProtranConfig.Persistence.IsEnabled);
            }

#if LTG_INFOTRANSITE
            // We require these protocols regardless of the config settings, add if missing
            this.AddRequiredLuminatorProtocols(this.cfgLoader.ProtranConfig);
#endif
            this.ProtocolHost = new ProtocolHost(this.cfgLoader.ProtranConfig.Protocols, this.dictionaryLoader.Config);
        
            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(ManagementName, root, this);
            root.AddChild(provider);

            // whole initialization ended.
        }

        /// <summary>
        /// Add the required protocols for Luminator - Infotransite
        /// </summary>
        /// <param name="protranConfig"></param>
        private void AddRequiredLuminatorProtocols(ProtranConfig protranConfig)
        {
            if (protranConfig.Protocols.Exists(m => m.Name == "XimpleProtocol") == false)
            {
                // we LTG require this, exclude if not binary
                if (File.Exists("Luminator.Motion.Protran.XimpleProtocol.dll"))
                {
                    Logger.Warn("XimpleProtocol not found in settings, Adding it as a default now!");
                    var protocolConfig = new ProtocolConfig() { Name = "XimpleProtocol", Enabled = true };
                    this.cfgLoader.ProtranConfig.Protocols.Add(protocolConfig);
                }
            }

            if (protranConfig.Protocols.Exists(m => m.Name == "PeripheralProtocol") == false)
            {
                // we LTG require this, exclude if not binary
                if (File.Exists("Luminator.Motion.Protran.PeripheralProtocol.dll"))
                {
                    Logger.Warn("PeripheralProtocol not found in settings, Adding it as a default now!");
                    var protocolConfig = new ProtocolConfig() { Name = "PeripheralProtocol", Enabled = true };
                    this.cfgLoader.ProtranConfig.Protocols.Add(protocolConfig);
                }
            }

            if (protranConfig.Protocols.Exists(m => m.Name == "AdHocMessagingProtocol") == false)
            {
                // we LTG require this, exclude if not binary
                if (File.Exists("Luminator.Motion.Protran.AdHocMessagingProtocol.dll"))
                {
                    Logger.Warn("AdHocMessagingProtocol not found in settings, Adding it as a default now!");
                    var protocolConfig = new ProtocolConfig() { Name = "AdHocMessagingProtocol", Enabled = true };
                    this.cfgLoader.ProtranConfig.Protocols.Add(protocolConfig);
                }
            }
        }

        /// <summary>
        /// Gets the manager of all the things to deal with the protocols.
        /// </summary>
        public ProtocolHost ProtocolHost { get; private set; }

        /// <summary>
        /// Registers all <see cref="Gorba.Motion.Protran.Core"/> services with service container.
        /// </summary>
        public static void SetupCoreServices()
        {
            IPersistenceService persistence;
            try
            {
                persistence = ServiceLocator.Current.GetInstance<IPersistenceService>();
                if (persistence != null)
                {
                    return;
                }
            }
            catch (Exception)
            {
                // no persistence service found, creating one
            }

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            persistence = PersistenceServiceFactory.CreatePersistenceService();
            serviceContainer.RegisterInstance(persistence);
        }

        /// <summary>
        /// Closes and deletes all the objects created.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                Logger.Info("Closing all");

                // saving persistence here before disposing of everything
                var persistence = ServiceLocator.Current.GetInstance<IPersistenceService>() as IPersistenceServiceImpl;
                if (persistence != null)
                {
                    persistence.Save();
                }

                if (this.ProtocolHost != null)
                {
                    this.ProtocolHost.Dispose();
                }

                this.ProtocolHost = null;
            }
        }

        /// <summary>
        /// Starts all Protran protocols
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (this.ProtocolHost != null)
                {
                    // I start the protocol manager.
                    // Registration must be completed before this.
                    this.ProtocolHost.StartProtocols();
                }
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Protocols", parent, this.ProtocolHost);
            yield return parent.Factory.CreateManagementProvider("XimpleCache", parent, this.ProtocolHost.XimpleCache);
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<ulong>("Ximple count", this.ProtocolHost.XimpleCount, true);
        }

        /// <summary>
        /// Loads all the configurations stored into the config file.
        /// During the load process, if the config file does not exist
        /// or it is corrupted, will be automatically created and loaded
        /// the default configuration file.
        /// Attention !!!
        /// If this function fails, you have to close this application.
        /// </summary>
        /// <returns>True if the load operation ends with success, else false.</returns>
        private bool LoadConfigurations()
        {
            this.cfgLoader = new ConfigLoader(PathManager.Instance.GetPath(FileType.Config, "protran.xml"));
            return this.cfgLoader.Load();
        }
    }
}
