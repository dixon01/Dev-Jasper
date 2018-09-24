// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolHost.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtocolHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Protocols
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using NLog;

    /// <summary>
    /// This object has to deal with load of all the available protocols stored in a well
    /// specified directory, with the reception of data from the protocols and with the send
    /// of data to the protocols.
    /// </summary>
    public class ProtocolHost : IProtocolHost, IManageable
    {
        /// <summary>
        /// The logger used by the host.
        /// </summary>
        private static readonly Logger Logger = LogHelper.GetLogger<ProtocolHost>();

        /// <summary>
        /// The cache tasked to collect (for ever and ever)
        /// all the cells coming from the loaded protocol(s)
        /// and to be used in case of the command <code>RequestInfoDatasCmd</code>".
        /// </summary>
        private readonly ProtranXimpleCache ximpleCache;

        /// <summary>
        /// The dictionary to use to accomplish the "Generic View" feature
        /// (introduced with the Protran version 1.0.0.7).
        /// </summary>
        private readonly Dictionary dictionary;

        /// <summary>
        /// The list containing all the "Protocol"s allowed to start as told
        /// by the configuration file.
        /// </summary>
        private List<ProtocolConfig> cfgProtocolsList;

        /// <summary>
        /// Flag that indicates if this object has to continue its tasks or to stop all.
        /// </summary>
        private bool isToRun;

        /// <summary>
        /// The list of all the protocols started by Protran, according to the configuration file.
        /// </summary>
        private List<IProtocol> protocolsStarted;

        private ulong ximpleCount;

        private int ProtolcolStartedCount { get; set; }

        /// <summary>
        /// Gets The total Protocols
        /// </summary>
        int ProtolcolsCount
        {
            get
            {
                return this.cfgProtocolsList?.Count ?? 0;
            }
        }

        public bool AllProtocolsStarted
        {
            get
            {
                return this.ProtolcolStartedCount == this.ProtolcolsCount;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolHost"/> class.
        /// Due to the fact that this object extends the base object "ProtocolManager",
        /// this constructor involves also the internal operations of the "ProtocolManager"'s constructor.
        /// </summary>
        /// <param name="protocolsList">
        /// The list of all the protocols allowed to start.
        /// </param>
        /// <param name="dictionary">
        /// The generic view dictionary.
        /// </param>
        public ProtocolHost(List<ProtocolConfig> protocolsList, Dictionary dictionary)
        {
            this.dictionary = dictionary;
            this.cfgProtocolsList = protocolsList;

            // I immediately allow the acceptance of data from protocols.
            this.isToRun = true;

            this.ximpleCache = new ProtranXimpleCache();
            this.protocolsStarted = new List<IProtocol>();
        }

        /// <summary>
        /// Event that is fired whenever a protocol was started.
        /// </summary>
        public event EventHandler<ProtocolEventArgs> ProtocolStarted;

        /// <summary>
        /// Gets the ximple cache.
        /// </summary>
        public ProtranXimpleCache XimpleCache
        {
            get
            {
                return this.ximpleCache;
            }
        }

        /// <summary>
        /// Gets the total number of ximple received from different protocols in protran.
        /// </summary>
        public ulong XimpleCount
        {
            get
            {
                return this.ximpleCount;
            }
        }

        /// <summary>
        /// Disposes of all the variables created by this object.
        /// </summary>
        public void Dispose()
        {
            MessageDispatcher.Instance.Unsubscribe<XimpleMessageRequest>(this.OnXimpleMessageRequestReceived);
            this.isToRun = false;

            // at this point, the notifying process is surely ended.
            if (this.cfgProtocolsList != null)
            {
                this.cfgProtocolsList.Clear();
                this.cfgProtocolsList = null;
            }

            if (this.protocolsStarted == null)
            {
                return;
            }

            // while (this.protocolsStarted.Count != 0)
            foreach (var protocol in this.protocolsStarted)
            {
                protocol.Stop();
            }

            this.protocolsStarted.Clear();
            this.protocolsStarted = null;
        }

        /// <summary>
        /// Starts all the previously available protocols (as told by the configuration file).
        /// </summary>
        public void StartProtocols()
        {
            Logger.Info("Starting {0} Protran protocols...", this.ProtolcolsCount);

            MessageDispatcher.Instance.Subscribe<XimpleMessageRequest>(this.OnXimpleMessageRequestReceived);

            // before starting the real "Start" phase,
            // I want to perform some checks on the available protocols.
            if (this.ProtolcolsCount <= 0)
            {
                // I don't have any protocol available coming from the configuration file.
                // So, I don't have to activate any protocol.
                // I can return immediately.
                Logger.Error("No protocol to start.");
                return;
            }

            // yes, I've some protocol to start.
            // Now I've to match the protocols specified in the configuration file
            // with the real protocols objects contained into the "Gorba.Motion.Protran.Protocols" library.
            foreach (var protocolConfig in this.cfgProtocolsList)
            {
                if (string.IsNullOrEmpty(protocolConfig?.Name))
                {
                    // invalid protocol coming from the cfg file avoid starting it.
                    Logger.Warn("Ignored starting  protocol Name={0} Enabled={1}", protocolConfig?.Name, protocolConfig?.Enabled);
                    continue;
                }

                var protocol = this.CreateProtocol(protocolConfig?.Name);
                if (protocol == null)
                {
                    // invalid protocol.
                    // I cannot start an invalid protocol.
                    Logger.Warn("Ignored starting unknown protocol {0}", protocolConfig.Name);
                    continue;
                }

                if (!protocolConfig.Enabled)
                {
                    // Protocol disabled raise event that we processed it to indicate Protan is fully started once all Protocols indicate running state
                    Logger.Warn("Disabled protocol Name={0} Enabled={1}", protocolConfig?.Name, protocolConfig?.Enabled);
                    this.RaiseProtocolStarted(new ProtocolEventArgs(protocol));
                }

                // found the corresponding protocol.
                // now I will give to the protocol, all the useful information
                // stored about it from the configuration file.
                Logger.Info("Configuring " + protocol.Name);
                protocol.Configure(this.dictionary);
                protocol.Started += (s, e) => { this.RaiseProtocolStarted(new ProtocolEventArgs(protocol)); };
                Logger.Info("Protran is starting Protocol {0}", protocol.Name);
                this.Start(protocol);
                this.protocolsStarted.Add(protocol);
            }
        }

        /// <summary>
        /// Asynchronous function invoked by a protocol whenever it has some
        /// data to send (to InfoMedia.exe for example).
        /// Do not call this function by yourself.
        /// It should be used only by the allowed protocols.
        /// </summary>
        /// <param name="sender">The protocol that has sent the data.</param>
        /// <param name="data">The data sent by the protocol.</param>
        public void OnDataFromProtocol(IProtocol sender, Ximple data)
        {
            this.ximpleCount++;
            Logger.Info("Protran has received XIMPLE object ximpleCount={0}", ximpleCount);

            // a protocol has sent to me some data.
            // but before processing it, I make some checks.
            if (sender == null || data == null)
            {
                // invalid param.
                // I discard data with this kind of incoming params.
                Logger.Warn("Invalid protocol sender and/or XIMPLE object");
                return;
            }

            if (!this.isToRun)
            {
                // it was told to me to reject all the data received
                // from the protocols.
                Logger.Warn("XIMPLE object discarded this.isToRun == false");
                return;
            }

            this.ximpleCache.Add(data);
            Logger.Info("Protran's core has updated its XIMPLE cache ({0} cells)", data.Cells.Count);
            this.Send(data);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var protocol in this.protocolsStarted)
            {
                yield return parent.Factory.CreateManagementProvider(protocol.Name, parent, protocol);
            }
        }

        /// <summary>
        /// Gets an instance of a specific Protocol basing on its type.
        /// </summary>
        /// <param name="protocolName">The name of the protocol to search.</param>
        /// <returns>The protocol with the incoming name (if allowed), or null.</returns>
        private static string GetProtocolFullName(string protocolName)
        {
            switch (protocolName)
            {
                case "ArrivaProtocol":
                    return "Gorba.Motion.Protran.Arriva.ArrivaProtocol, Gorba.Motion.Protran.Arriva";
                case "IbisProtocol":
                    return "Gorba.Motion.Protran.Ibis.IbisProtocol, Gorba.Motion.Protran.Ibis";
                case "IOProtocol":
                    return "Gorba.Motion.Protran.IO.IOProtocol, Gorba.Motion.Protran.IO";
                case "AbuDhabiProtocol":
                    return "Gorba.Motion.Protran.AbuDhabi.AbuDhabiProtocol, Gorba.Motion.Protran.AbuDhabi";
                case "SoapIbisPlus":
                    return "Gorba.Motion.Protran.SoapIbisPlus.SoapIbisPlusProtocol, Gorba.Motion.Protran.SoapIbisPlus";
                case "VDV301":
                case "Vdv301":
                case "VDV301Protocol":
                case "Vdv301Protocol":
                    return "Gorba.Motion.Protran.Vdv301.Vdv301Protocol, Gorba.Motion.Protran.Vdv301";
                case "Gorba":
                    return "Gorba.Motion.Protran.GorbaProtocol.GorbaProtocolImpl, Gorba.Motion.Protran.GorbaProtocol";
                // LTG protocols added below
                case "XimpleProtocol":
                    return "Luminator.Motion.Protran.XimpleProtocol.XimpleProtocolImpl, Luminator.Motion.Protran.XimpleProtocol";
                case "PeripheralProtocol":
                    return "Luminator.Motion.Protran.PeripheralProtocol.PeripheralProtocolImpl, Luminator.Motion.Protran.PeripheralProtocol";
                case "AdHocMessagingProtocol":
                    return "Luminator.Motion.Protran.AdHocMessagingProtocol.AdHocMessagingProtocolImpl, Luminator.Motion.Protran.AdHocMessagingProtocol";
                default:
                    return protocolName;
            }
        }

        private Ximple PrepareCachedXimple()
        {
            return this.ximpleCache.Dump();
        }

        private void OnXimpleMessageRequestReceived(object sender, MessageEventArgs<XimpleMessageRequest> e)
        {
            if (e.Message == null)
            {
                return;
            }

            var cachedXimple = this.PrepareCachedXimple();

            MessageDispatcher.Instance.Send(e.Source, cachedXimple);
            if (e.Source != null)
            {
                Logger.Info("Medi Ximple sent to App={0}, Unit={1}, Type={2}", e.Source.Application, e.Source.Unit, e.Message.GetType());
            }
        }

        private void Send(Ximple ximple)
        {
            Logger.Info("MessageDispatcher.Instance.Broadcast<Ximple> Called");
            MessageDispatcher.Instance.Broadcast(ximple);
        }

        private void RaiseProtocolStarted(ProtocolEventArgs e)
        {
            this.ProtolcolStartedCount++;
            Logger.Info("RaiseProtocolStarted() Enter Protocols Started Count={0}", this.ProtolcolStartedCount);
            // Signal the host app is Started once all the Protocols have started successfully
            if (this.ProtolcolStartedCount >= this.ProtolcolsCount)
            {
                if (e.Protocol != null)
                {
                    Logger.Info("Protocol Name={0} Started {1} of {2}", e.Protocol.Name, this.ProtolcolStartedCount, this.ProtolcolsCount);
                    var handler = this.ProtocolStarted;
                    if (handler != null)
                    {
                        Logger.Info("Fire Protocol started event for {0}", e.Protocol.Name);
                        handler(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// Starts a protocol giving to it an array of arguments required for
        /// its execution. The protocol's "Main( ... )" function will be called.
        /// </summary>
        /// <param name="protocol">The protocol to start.</param>
        private void Start(IProtocol protocol)
        {
            // the invocation of the protocol's "Main" function could
            // be blocking. For this reason I've to call it in a
            // new Thread.
            if (protocol != null)
            {
                var callMainTh = new Thread(() => protocol.Run(this))
                {
                    Name = "Protocol_" + protocol.Name,
                    IsBackground = true
                };
                callMainTh.Start();
            }
        }

        /// <summary>
        /// Gets an instance of a specific Protocol basing on its type.
        /// </summary>
        /// <param name="protocolName">The name of the protocol to search.</param>
        /// <returns>The protocol with the incoming name (if allowed), or null.</returns>
        private IProtocol CreateProtocol(string protocolName)
        {
            var file = GetProtocolFullName(protocolName);
            var type = Type.GetType(file, true, true);
            Logger.Info("Loading Protocol File = {0}, Type = {1}", file, type);
            Debug.Assert(type != null, "Type can't be null");
            return (IProtocol)Activator.CreateInstance(type);
        }
    }
}
