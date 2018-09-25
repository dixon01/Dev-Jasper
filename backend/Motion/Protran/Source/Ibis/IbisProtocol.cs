// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Protocols;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.Ibis.Channels;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Ibis.Remote;
    using Gorba.Motion.Protran.Ibis.TimeSync;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Class that represents the manager of the IBIS communications.
    /// </summary>
    public sealed class IbisProtocol : IProtocol, IIbisConfigContext, IManageable
    {
        /// <summary>
        /// The name of the configuration file.
        /// Attention (this is not the absolute file's name).
        /// </summary>
        private const string IbisCfgFileName = "ibis.xml";

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogHelper.GetLogger<IbisProtocol>();

        /// <summary>
        /// Event used to manage the running status of this protocol.
        /// </summary>
        private readonly AutoResetEvent liveEvent = new AutoResetEvent(false);

        /// <summary>
        /// The list that will contain all the telegram handlers created
        /// by the telegrams factory.
        /// </summary>
        private readonly List<IInputHandler> inputHandlers = new List<IInputHandler>();

        private readonly List<IbisChannel> ibisChannels = new List<IbisChannel>();

        /// <summary>
        /// A reference to the manager about the IBIS configurations.
        /// </summary>
        private readonly ConfigMng configMng;

        private readonly ProtranXimpleCache ximpleCache = new ProtranXimpleCache();

        private readonly List<SimplePort> ibisPorts = new List<SimplePort>();

        private bool isHandlerIterationActive;

        private IbisTimeSync timeSync;

        /// <summary>
        /// Variable that tells how many XIMPLE were been sent to
        /// the host (Protran) of this protocol.
        /// </summary>
        private int ximplesSentCounter;

        private IProtocolHost host;

        private GenericUsageHandler connectionStatusUsage;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisProtocol"/> class.
        /// Allocates and initializes all the required things for the IBIS protocol.
        /// </summary>
        public IbisProtocol()
        {
            this.configMng = new ConfigMng();
            try
            {
                var fullPathIbisCfgFile = PathManager.Instance.GetPath(FileType.Config, IbisCfgFileName);

                Logger.Info("Loading config file: {0}", fullPathIbisCfgFile);
                this.configMng.Load(fullPathIbisCfgFile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Error on loading ibis.xml file");
                return;
            }

            Process.GetCurrentProcess().PriorityClass = this.configMng.IbisConfig.Behaviour.ProcessPriority;

            Logger.Info("Ibis cfg. file loaded.");

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance(this);
        }

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Event that is fired whenever the this protocol creates
        /// a new ximple object.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get
            {
                return "IBIS";
            }
        }

        /// <summary>
        /// Gets the IBIS config.
        /// </summary>
        public IbisConfig Config
        {
            get
            {
                return this.configMng.IbisConfig;
            }
        }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary.
        /// </param>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;

            this.connectionStatusUsage = new GenericUsageHandler(
                this.Config.Behaviour.ConnectionStatusUsedFor, dictionary);

            this.CreateChannels();
            this.CreateHandlers();
            this.CreateTimeSync();
        }

        /// <summary>
        /// Deletes and closes all the objects created
        /// by this object.
        /// </summary>
        public void Stop()
        {
            this.liveEvent.Set();
            this.CloseAll();
        }

        /// <summary>
        /// Starts all the internal activities to this Protocol.
        /// </summary>
        /// <param name="protocolHost">The owner of this protocol.</param>
        public void Run(IProtocolHost protocolHost)
        {
            this.host = protocolHost;

            // do this after creating all hanlders
            this.inputHandlers.Sort((left, right) => left.Priority - right.Priority);

            foreach (var handler in this.inputHandlers)
            {
                handler.XimpleCreated += this.HandlerCreatedXimple;
                handler.StatusChanged += this.HandlerStatusChanged;
                handler.StartCheck();
            }

            foreach (var channel in this.ibisChannels)
            {
                channel.Open();
            }

            for (int i = 0; i < this.Config.Behaviour.IbisAddresses.Count; i++)
            {
                var port = new SimplePort(
                    string.Format("IbisAddress{0}", i),
                    true,
                    false,
                    new IntegerValues(1, 15),
                    this.Config.Behaviour.IbisAddresses[i]);
                GioomClient.Instance.RegisterPort(port);
                this.ibisPorts.Add(port);
            }

            if (this.timeSync != null)
            {
                this.timeSync.Start();
            }

            this.RaiseStarted(EventArgs.Empty);

            // ok. Now we can wait for the end of this protocol.
            this.liveEvent.WaitOne();

            // at this line of code, to the protocol was ordered to be stopped.
            this.CloseAll();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return
                parent.Factory.CreateManagementProvider("Handlers", parent, new ManageHandlers(this.inputHandlers));
            foreach (var channel in this.ibisChannels)
            {
                yield return parent.Factory.CreateManagementProvider("Channel", parent, channel);
            }
        }

        string IIbisConfigContext.GetAbsolutePathRelatedToConfig(string file)
        {
            return this.configMng.GetAbsolutePathRelatedToConfig(file);
        }

        /// <summary>
        /// Creates all the required channel as specified
        /// in the configuration file.
        /// Examples: we can have ethernet channels and/or serial channels
        /// or more than one serial channel, or more than one ethernet channel
        /// and so on.
        /// For each channel, is also set the referring recorder, if configured.
        /// </summary>
        private void CreateChannels()
        {
            var factory = ServiceLocator.Current.GetInstance<ChannelFactory>();

            var channels = factory.CreateChannels(this);
            foreach (var channel in channels)
            {
                channel.Dictionary = this.Dictionary;
                channel.XimpleCreated += (s, e) => this.SendXimple(e.Ximple);
                channel.TelegramReceived += this.ChannelOnTelegramReceived;

                if (channel.RemoteComputer != null)
                {
                    channel.RemoteComputer.StatusChanged += this.RemoteComputerOnStatusChanged;
                }

                this.ibisChannels.Add(channel);
            }
        }

        private void RemoteComputerOnStatusChanged(object sender, EventArgs e)
        {
            var ximple = new Ximple();
            if (this.AddRemoteComputerStatus(sender as RemoteComputer, ximple))
            {
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }
        }

        private void ChannelOnTelegramReceived(object sender, TelegramReceivedEventArgs e)
        {
            var telegram = e.Telegram;
            var name = telegram.GetType().Name;
            Logger.Info("Received input: {0}", name);
            if (this.Dictionary == null)
            {
                // the dictionary is null.
                // even if I've received a valid datagram
                // I cannot translate it to a XIMPLE.
                Logger.Error("Dictionary null. Translation failed (No XIMPLE sent).");
                return;
            }

            if (!e.Config.Enabled)
            {
                Logger.Info("{0} not enabled. XIMPLE translation skipped.", name);
                return;
            }

            Ximple ximple;
            lock (this.ximpleCache)
            {
                this.ximpleCache.Clear();
                bool found = false;
                this.isHandlerIterationActive = true;
                foreach (var inputHandler in this.inputHandlers)
                {
                    if (!inputHandler.Accept(telegram))
                    {
                        continue;
                    }

                    found = true;
                    try
                    {
                        Logger.Debug("Handling {0} with {1}", name, inputHandler);
                        inputHandler.HandleInput(telegram);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "{0}: {1}", name, ex.Message);
                    }
                }

                if (!found)
                {
                    Logger.Warn("No handler found for {0}", name);
                }

                // now it's the time to get the XIMPLE from the cache.
                // all the previous handlers have filled it with their information.
                ximple = this.ximpleCache.Dump();
                this.isHandlerIterationActive = false;
            }

            if (ximple.Cells.Count == 0)
            {
                // TODO: this can be dangerous if we only set CMDs, but no cells
                return;
            }

            // now we can really pass the XIMLPE to the handler below
            // because the telegram above has produced some cell.
            this.SendXimple(ximple);
            Logger.Debug("Ximple dumped and raised.");
        }

        /// <summary>
        /// Creates all input handlers using the factory.
        /// </summary>
        private void CreateHandlers()
        {
            var factory = ServiceLocator.Current.GetInstance<TelegramHandlerFactory>();
            foreach (var telegram in this.Config.Telegrams)
            {
                var handler = factory.CreateHandler(telegram, this);
                if (handler != null)
                {
                    this.inputHandlers.Add(handler);
                }
            }
        }

        /// <summary>
        /// Function called whenever a XIMPLE was created and ready to be sent
        /// to Protran core.
        /// </summary>
        /// <param name="sender">
        /// The sender that has invoked this function.
        /// </param>
        /// <param name="e">
        /// The event containing the XIMPLE.
        /// </param>
        private void HandlerCreatedXimple(object sender, XimpleEventArgs e)
        {
            var ximple = e.Ximple;
            if (ximple == null)
            {
                // no XIMPLE to send.
                return;
            }

            lock (this.ximpleCache)
            {
                if (this.isHandlerIterationActive)
                {
                    this.ximpleCache.Add(ximple);
                    Logger.Debug("Ximple cached.");
                    return;
                }
            }

            // the XIMPLE structure was created.
            this.SendXimple(e.Ximple);
        }

        /// <summary>
        /// Function called whenever a ITelegramHandler has
        /// changed its status depending on the data received.
        /// </summary>
        /// <param name="sender">
        /// The sender that has invoked this function.
        /// </param>
        /// <param name="e">
        /// The event containing the status.
        /// </param>
        private void HandlerStatusChanged(object sender, EventArgs e)
        {
            if (this.ibisChannels.Count == 0)
            {
                return;
            }

            var localStatus = Status.Ok;
            foreach (var handler in this.inputHandlers)
            {
                var telegramHandler = handler as ITelegramHandler;
                if ((telegramHandler == null || telegramHandler.Enabled) && handler.Status > localStatus)
                {
                    localStatus = handler.Status;
                }
            }

            // I update the current channel's status
            // with the status just received.
            foreach (var channel in this.ibisChannels)
            {
                channel.CurrentStatus = localStatus;
            }
        }

        /// <summary>
        /// Adds the remote computer status flag to an existing ximple structure.
        /// </summary>
        /// <param name="computer">the remote computer (can be null)</param>
        /// <param name="ximple">the ximple to which the flag is added.</param>
        /// <returns>true if the status was added</returns>
        private bool AddRemoteComputerStatus(RemoteComputer computer, Ximple ximple)
        {
            if (computer == null)
            {
                return false;
            }

            var cell = this.connectionStatusUsage.AddCell(
                ximple, computer.Status == RemoteComputerStatus.Active ? "1" : "0");
            return cell != null;
        }

        /// <summary>
        /// Creates the <see cref="IbisTimeSync"/> object.
        /// </summary>
        private void CreateTimeSync()
        {
            var config = this.Config.TimeSync;
            var ds006 = this.Config.Telegrams.Find(t => t is DS006Config) as DS006Config;
            if (config == null || ds006 == null || !config.Enabled)
            {
                return;
            }

            this.timeSync = new IbisTimeSync();
            this.timeSync.Configure(config, ds006, this);
            this.inputHandlers.Add(this.timeSync);
        }

        /// <summary>
        /// Close all the running threads and delete all the resources
        /// allocated during the program's execution.
        /// </summary>
        private void CloseAll()
        {
            lock (this)
            {
                Logger.Info("Closing all...");

                if (this.timeSync != null)
                {
                    this.timeSync.Stop();
                    this.timeSync = null;
                }

                foreach (var channel in this.ibisChannels)
                {
                    channel.Close();
                }

                this.ibisChannels.Clear();

                foreach (var simplePort in this.ibisPorts)
                {
                    GioomClient.Instance.DeregisterPort(simplePort);
                }

                Logger.Info("All closed. Good bye.");
            }
        }

        private void SendXimple(Ximple ximple)
        {
            // a XIMPLE is ready to be sent.
            // if we have a XIMPLE, it means that the remote board computer should be active.
            // if it is active, I'll add a cell in the previously XIMPLE with this additional info.
            foreach (var channel in this.ibisChannels)
            {
                if (channel.RemoteComputer == null || channel.RemoteComputer.Status != RemoteComputerStatus.Active)
                {
                    continue;
                }

                this.AddRemoteComputerStatus(channel.RemoteComputer, ximple);
                break;
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        /// <summary>
        /// Function called whenever a XIMPLE was created and ready to be sent
        /// to Protran core.
        /// </summary>
        /// <param name="e">
        /// The event containing the XIMPLE.
        /// </param>
        private void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var ximple = e.Ximple;
            if (ximple == null || this.host == null)
            {
                // no XIMPLE to send.
                return;
            }

            // the XIMPLE structure was created.
            // I can launch it to Protran. This last will do the rest.
            this.host.OnDataFromProtocol(this, ximple);
            Logger.Info("IbisProtocol sent a XIMPLE to Protran ({0}).", ++this.ximplesSentCounter);
            Logger.Trace(ximple.ToXmlString);

            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private class ManageHandlers : IManageable
        {
            private readonly List<IInputHandler> inputHandlers;

            public ManageHandlers(List<IInputHandler> inputHandlers)
            {
                this.inputHandlers = inputHandlers;
            }

            IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
            {
                foreach (var handler in this.inputHandlers)
                {
                    yield return parent.Factory.CreateManagementProvider(
                        string.Format("{0}", handler), parent, handler);
                }
            }
        }
    }
}
