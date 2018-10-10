// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Protocols;
    using Gorba.Motion.Protran.IO.Inputs;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The I/O protocol which can handle serial ports and use I/O's for Ximple messages.
    /// </summary>
    public partial class IOProtocol : IProtocol, IManageable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<IOProtocol>();

        private readonly ManualResetEvent stopWait = new ManualResetEvent(false);

        private readonly TransformationManager transformationManager = new TransformationManager();

        private readonly IOProtocolConfig config;

        private readonly List<IInputHandler> inputHandlers = new List<IInputHandler>();

        private IProtocolHost protocolHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="IOProtocol"/> class.
        /// </summary>
        public IOProtocol()
        {
            var configManager = new ConfigManager<IOProtocolConfig>
                                    {
                                        FileName = PathManager.Instance.GetPath(FileType.Config, "io.xml"),
                                        EnableCaching = true
                                    };
            this.config = configManager.Config;
            configManager.XmlSchema = IOProtocolConfig.Schema;

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
        /// Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get
            {
                return "I/O";
            }
        }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the io protocol config.
        /// </summary>
        public IOProtocolConfig Config
        {
            get
            {
                return this.config;
            }
        }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        ///     The generic view dictionary.
        /// </param>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;

            this.transformationManager.Configure(this.config.Transformations);

            this.ConfigureSerialPorts();

            var factory = ServiceLocator.Current.GetInstance<InputHandlerFactory>();

            foreach (var input in this.config.Inputs)
            {
                var handlers = factory.CreateInputHandler(input, this.transformationManager, this.Dictionary);
                if (handlers != null)
                {
                    handlers.XimpleCreated += this.InputHandlerOnXimpleCreated;
                    this.inputHandlers.Add(handlers);
                }
            }
        }

        /// <summary>
        /// Stop this protocol.
        /// </summary>
        public void Stop()
        {
            foreach (var inputHandler in this.inputHandlers)
            {
                inputHandler.Stop();
            }

            this.inputHandlers.Clear();

            this.StopSerialPorts();

            this.stopWait.Set();
        }

        /// <summary>
        /// The main function of your protocol.
        /// Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        public void Run(IProtocolHost host)
        {
            this.stopWait.Reset();

            this.protocolHost = host;

            this.StartSerialPorts();

            foreach (var inputHandler in this.inputHandlers)
            {
                try
                {
                    inputHandler.Start();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't start input handler for {0}", inputHandler.Name);
                }
            }

            Logger.Debug("Running");
            this.RaiseStarted(EventArgs.Empty);

            this.stopWait.WaitOne();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var inputHandler in this.inputHandlers)
            {
                yield return parent.Factory.CreateManagementProvider(inputHandler.Name, parent, inputHandler);
            }
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        partial void ConfigureSerialPorts();

        partial void StartSerialPorts();

        partial void StopSerialPorts();

        private void InputHandlerOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            if (this.protocolHost != null)
            {
                this.protocolHost.OnDataFromProtocol(this, e.Ximple);
                Logger.Info("I/O Protocol sent a XIMPLE to Protran");
                Logger.Trace(e.Ximple.ToXmlString);

                var handler = this.XimpleCreated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}
