// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;
    using Gorba.Motion.Protran.AbuDhabi.Transformations;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Object tasked to manage all the interactions
    /// with the ISI TCP/IP remote server.
    /// </summary>
    public class IsiClient : XimpleSourceBase, IManageableObject
    {
        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Container of all the configuration needed
        /// to interact with the remote TCP/IP ISI server.
        /// </summary>
        private readonly IsiConfig isiConfig;

        /// <summary>
        /// The container of all the subscription to realize
        /// with the remote ISI server.
        /// </summary>
        private readonly List<SubscriptionManager> subscriptionManagers;

        /// <summary>
        /// Reference to the object tasked to send cyclically ISI put messages
        /// to the remote ISI server accordingly to the received
        /// ISI get messages having the "Cyclic" feature.
        /// </summary>
        private readonly List<CyclicManager> requestManagers;

        private readonly Dictionary<string, List<IDataItemValueProvider>> dataItemValueProviders;

        private readonly GenericUsageHandler connectionStatusUsage;

        private readonly GenericUsage tickerTextUsage;

        private RemoteComputerStatus status;

        private bool avoidXimpleNotifications;

        private int isiMessageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsiClient"/> class.
        /// </summary>
        /// <param name="dictionary">the generic view Dictionary</param>
        /// <param name="config">The container of the configurations.</param>
        public IsiClient(Dictionary dictionary, AbuDhabiConfig config)
            : base(dictionary)
        {
            this.isiConfig = config.Isi;
            this.connectionStatusUsage = new GenericUsageHandler(config.Behaviour.ConnectionStatusUsedFor, dictionary);

            this.RemoteComputer = ServiceLocator.Current.GetInstance<RemoteComputer>();
            this.RemoteComputer.Configure(this.isiConfig);
            this.RemoteComputer.Connected += this.OnRemoteComputerConnected;
            this.RemoteComputer.Disconnected += this.OnRemoteComputerDisconnected;
            this.RemoteComputer.IsiMessageReceived += this.OnIsiMessageReceived;
            this.requestManagers = new List<CyclicManager>();
            this.TransformationManger = ServiceLocator.Current.GetInstance<TransformationManager>();
            this.TransformationManger.Configure(config.Transformations);
            this.subscriptionManagers = new List<SubscriptionManager>();
            foreach (var subscription in config.Subscriptions)
            {
                var mng = new SubscriptionManager(this, subscription);
                mng.XimpleCreated += this.SubscriptionManagerOnXimpleCreated;
                this.subscriptionManagers.Add(mng);
            }

            this.tickerTextUsage = this.FindUsage(DataItemName.TickerText);

            this.dataItemValueProviders = new Dictionary<string, List<IDataItemValueProvider>>();

            this.AddDataItemValueProvider(DataItemName.AppName, new StaticDataItemValueProvider("INFOTAINMENT"));
            this.AddDataItemValueProvider(
                DataItemName.SerialNumber, new StaticDataItemValueProvider("9," + Environment.MachineName));
        }

        /// <summary>
        /// Handler tasked to fire the "StatusChanged" event.
        /// </summary>
        public event EventHandler<RemoteComputerStatusEventArgs> StatusChanged;

        /// <summary>
        /// Event that is fired every time a data item is received.
        /// </summary>
        public event EventHandler<DataItemEventArgs> DataItemReceived;

        /// <summary>
        /// Gets the transformation manger.
        /// </summary>
        public TransformationManager TransformationManger { get; private set; }

        /// <summary>
        /// Gets the reference to the object tasked to represent
        /// the remote ISI board computer (that has a ISI TCP/IP server).
        /// </summary>
        public RemoteComputer RemoteComputer { get; private set; }

        /// <summary>
        /// Gets or sets the current status of this remote computer.
        /// </summary>
        public RemoteComputerStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                // first I set the value
                this.status = value;

                // and now I notify all the registerd handlers about
                // the new remote computer's status.
                if (this.StatusChanged != null)
                {
                    this.StatusChanged(this, new RemoteComputerStatusEventArgs(this.status));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ISI client
        /// is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts all the activities with the remote
        /// ISI TCP/IP server.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.ConnectToRemoteComputer();

            // the function above is non-blocking, so after its invocation
            // for sure this client has at least started its state machine.
            // so, it's running.
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops all the activities with the remote
        /// ISI TCP/IP server.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning)
            {
                return;
            }

            this.requestManagers.ForEach(manager => manager.Stop());
            this.DisconnectFromRemoteComputer();

            // the function above is blocking, so after its invocation
            // for sure this client
            this.IsRunning = false;
        }

        /// <summary>
        /// Add an <see cref="IDataItemValueProvider"/> to this client.
        /// The provider will be queried for its value when we get an
        /// IsiGet for the given value.
        /// </summary>
        /// <param name="name">
        /// The name of the data item that will be provided by this provider.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public void AddDataItemValueProvider(string name, IDataItemValueProvider provider)
        {
            List<IDataItemValueProvider> providers;
            if (!this.dataItemValueProviders.TryGetValue(name, out providers))
            {
                providers = new List<IDataItemValueProvider>();
                this.dataItemValueProviders.Add(name, providers);
            }

            providers.Add(provider);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<int>("Isi Messages Received count", this.isiMessageCount, true);
            yield return new ManagementProperty<string>("Isi Connection state", this.Status.ToString(), true);
        }

        private void OnRemoteComputerConnected(object sender, EventArgs e)
        {
        }

        private void OnRemoteComputerDisconnected(object sender, EventArgs e)
        {
            this.UnsubscribeFromRemoteComputer();

            // remove all cyclic messages:
            foreach (var cyclicManager in this.requestManagers)
            {
                cyclicManager.Stop();
            }

            this.requestManagers.Clear();
        }

        /// <summary>
        /// Tries to connect to the remote ISI TCP/IP server
        /// in a separate thread.
        /// </summary>
        private void ConnectToRemoteComputer()
        {
            Logger.Info("Connecting to the remote ISI server...");
            this.RemoteComputer.Connect();
        }

        /// <summary>
        /// Tries to disconnect from the remote ISI TCP/IP server
        /// in a separate thread.
        /// </summary>
        private void DisconnectFromRemoteComputer()
        {
            Logger.Info("Disconnecting to the remote ISI server...");
            this.RemoteComputer.Disconnect();
        }

        /// <summary>
        /// Tries to subscribe Protran to the remote ISI TCP/IP server.
        /// </summary>
        private void SubscribeToRemoteComputer()
        {
            Logger.Debug("Subscribing us to the remote ISI server...");

            foreach (var subscriptionManager in this.subscriptionManagers)
            {
                subscriptionManager.Subscribe();
            }

            Logger.Info("All Protran's subscriptions sent");
        }

        /// <summary>
        /// Unsubscribes Protran from the remote ISI TCP/IP server.
        /// </summary>
        private void UnsubscribeFromRemoteComputer()
        {
            Logger.Debug("Unsubscribing us from the remote ISI server...");

            foreach (var subscriptionManager in this.subscriptionManagers)
            {
                subscriptionManager.Unsubscribe();
            }

            Logger.Info("All Protran's subscriptions unsubscribed");
        }

        private void SubscriptionManagerOnXimpleCreated(object s, XimpleEventArgs e)
        {
            this.SendPriorityXimple(e.Ximple);

            if (this.avoidXimpleNotifications)
            {
                return;
            }

            this.connectionStatusUsage.AddCell(e.Ximple, SystemStatus.Isi.ToString(CultureInfo.InvariantCulture));

            this.RaiseXimpleCreated(e);
        }

        /// <summary>
        /// Function invoked asynchronously whenever an ISI message
        /// was sent from the remote ISI server to Protran, after the subscription phase.
        /// </summary>
        /// <param name="sender">The object that has sent this event (the RemoteComputer).</param>
        /// <param name="e">The event with the ISI message.</param>
        private void OnIsiMessageReceived(object sender, IsiMessageEventArgs e)
        {
            var computer = sender as RemoteComputer;
            if (computer == null || computer != this.RemoteComputer || e.IsiMessage == null)
            {
                return;
            }

            var get = e.IsiMessage as IsiGet;
            if (get != null)
            {
                this.HandleIsiGet(get);
                return;
            }

            var put = e.IsiMessage as IsiPut;
            if (put != null)
            {
                this.HandleIsiPut(put);
                return;
            }

            // todo: handle IsiRegister and IsiExecute when needed
            Logger.Warn("Received unknown ISI message: {0}", e.IsiMessage.GetType().Name);
        }

        /// <summary>
        /// Does all the required stuff to do when an ISI get message is received
        /// (sends back the referring ISI put to the ISI server,
        /// updates the cycle manager, updates the subscription, logs...).
        /// </summary>
        /// <param name="get">The ISI get message to manage.</param>
        private void HandleIsiGet(IsiGet get)
        {
            if (get.Cyclic > 0)
            {
                // todo: this cyclic manager has to be stopped if the connection fails
                var mgr = new CyclicManager(get);
                mgr.UpdateRequested += (s, e) => this.SendPutForGet(get);
                this.requestManagers.Add(mgr);
                mgr.Start();

                Logger.Debug("Created cyclic manager for: {0}", get.Items);
            }
            else if (get.Items.Count == 0)
            {
                // this is a "stop" message, just stop the request manager
                var manager = this.requestManagers.Find(m => m.Id == get.IsiId);
                if (manager != null)
                {
                    manager.Stop();
                }

                return;
            }

            if (get.OnChange != null && get.OnChange.Count > 0)
            {
                bool all = get.OnChange.Count == 1 && get.OnChange[0] == "*";
                var items = all ? get.Items : get.OnChange;
                foreach (var item in items)
                {
                    List<IDataItemValueProvider> providers;
                    if (this.dataItemValueProviders.TryGetValue(item, out providers))
                    {
                        foreach (var provider in providers)
                        {
                            // todo: how do we deregister this event handler?
                            provider.ValueChanged += (s, e) => this.SendPutForGet(get);
                        }
                    }
                }
            }

            // now I've to prepare a valid ISI put.
            this.SendPutForGet(get);

            if (get.Items.Contains(DataItemName.IsiClientRunsFtpTransfers))
            {
                // the get is the "IsiClientRunsFtpTransfers" request
                // so, in this case we have right now authenticated ourself.
                this.SubscribeToRemoteComputer();

                this.Status = RemoteComputerStatus.Connected;
            }
        }

        private void SendPutForGet(IsiGet get)
        {
            var put = new IsiPut { IsiId = get.IsiId };

            foreach (var item in get.Items)
            {
                List<IDataItemValueProvider> providers;
                if (this.dataItemValueProviders.TryGetValue(item, out providers))
                {
                    foreach (var provider in providers)
                    {
                        put.Items.Add(new DataItem { Name = item, Value = provider.Value });
                    }
                }
                else
                {
                    Logger.Warn("Don't know value of {0}, responding with an empty string", item);
                    put.Items.Add(new DataItem { Name = item, Value = string.Empty });
                }
            }

            // now I can send the ISI put just filled out.
            this.RemoteComputer.SendIsiMessage(put);
        }

        /// <summary>
        /// Does all the required stuffs to do when an ISI put message is received
        /// (updates the subscription, logs...).
        /// </summary>
        /// <param name="put">The ISI put message to manage.</param>
        private void HandleIsiPut(IsiPut put)
        {
            this.isiMessageCount++;
            DataItem fallbackItem = put.Items.Find(i => i.Name.Equals(DataItemName.GorbaSystemFallbackActive));
            if (fallbackItem != null)
            {
                int result;
                if (int.TryParse(fallbackItem.Value, out result))
                {
                    this.avoidXimpleNotifications = result == 1;
                }
            }

            foreach (var dataItem in put.Items)
            {
                this.RaiseDataItemReceived(new DataItemEventArgs(dataItem));
            }
        }

        private void RaiseDataItemReceived(DataItemEventArgs e)
        {
            var handler = this.DataItemReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SendPriorityXimple(Ximple input)
        {
            var output = new Ximple();

            // get the ticker text even in fallback mode
            var tickerCell = this.SearchForCellWithSpecificUsage(input, this.tickerTextUsage);
            if (tickerCell == null)
            {
                return;
            }

            input.Cells.Remove(tickerCell);
            output.Cells.Add(tickerCell);

            this.RaisePriorityXimpleCreated(new XimpleEventArgs(output));
        }

        private GenericUsage FindUsage(string dataItemName)
        {
            foreach (var subscriptionManager in this.subscriptionManagers)
            {
                foreach (var dataItem in subscriptionManager.Subscription.DataItems)
                {
                    if (dataItem.Name == dataItemName)
                    {
                        return dataItem.UsedFor;
                    }
                }
            }

            return null;
        }
    }
}
