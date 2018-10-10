// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOVisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.IO
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.IO;
    using Gorba.Motion.Protran.IO.Inputs;
    using Gorba.Motion.Protran.IO.Serial;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The Visualization service for IO protocol management.
    /// </summary>
    public class IOVisualizationService : IIOVisualizationService
    {
        private readonly List<VisualizerInputHandler> inputHandlers = new List<VisualizerInputHandler>();

        private volatile bool initialized;

        /// <summary>
        /// Event that is fired every time a new Ximple is created
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event fired when the io input is being transformed.
        /// </summary>
        public event EventHandler IoInputTransforming;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        public event EventHandler<TransformationChainEventArgs> IoInputTransformed;

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public Dictionary Dictionary
        {
            get
            {
                return this.IoProtocol.Dictionary;
            }
        }

        /// <summary>
        /// Gets the config.
        /// </summary>
        public IOProtocolConfig Config
        {
            get
            {
                return this.IoProtocol.Config;
            }
        }

        /// <summary>
        /// Gets the io protocol.
        /// </summary>
        public IOProtocol IoProtocol
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IOProtocol>();
            }
        }

        /// <summary>
        /// The register.
        /// </summary>
        public static void Register()
        {
            Core.Protran.SetupCoreServices();
            var service = new IOVisualizationService();
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance<IIOVisualizationService>(service);
            var inputHandlerFactory = new VisualizationInputHandlerFactory(service);
            serviceContainer.RegisterInstance<InputHandlerFactory>(inputHandlerFactory);
            var serialPortFactory = new VisualizationSerialPortFactory(service);
            serviceContainer.RegisterInstance<SerialPortFactory>(serialPortFactory);
        }

        /// <summary>
        /// The send pin changed event.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="pinState">
        /// The pin state.
        /// </param>
        public void SendPinChangedEvent(InputHandlingConfig config, bool pinState)
        {
            foreach (var visualizerInputHandler in this.inputHandlers)
            {
                if (visualizerInputHandler.Name.Equals(config.Name))
                {
                    visualizerInputHandler.PinStateChanged(pinState);
                }
            }
        }

        private void Initialize()
        {
            if (!this.initialized)
            {
                lock (this)
                {
                    if (!this.initialized)
                    {
                        this.IoProtocol.XimpleCreated += (s, e) => this.OnXimpleCreated(e);
                        this.initialized = true;
                    }
                }
            }
        }

        private void OnXimpleCreated(XimpleEventArgs e)
        {
            EventHandler<XimpleEventArgs> handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnIoInputTransforming(EventArgs e)
        {
            EventHandler handler = this.IoInputTransforming;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnIoInputTransformed(TransformationChainEventArgs e)
        {
            EventHandler<TransformationChainEventArgs> handler = this.IoInputTransformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The visualization input handler factory.
        /// </summary>
        private class VisualizationInputHandlerFactory : InputHandlerFactory
        {
            private readonly IOVisualizationService service;

            public VisualizationInputHandlerFactory(IOVisualizationService service)
            {
                this.service = service;
            }

            public override IInputHandler CreateInputHandler(
                InputHandlingConfig input, TransformationManager transformationManager, Dictionary dictionary)
            {
                if (!input.Enabled)
                {
                    return null;
                }

                var chain = transformationManager.GetChain(input.TransfRef);
                if (chain == null)
                {
                    return null;
                }

                var inputHandler = new VisualizerInputHandler(input, chain, dictionary);
                inputHandler.Started += (s, e) => this.service.Initialize();
                inputHandler.IoInputTransforming += (s, e) => this.service.OnIoInputTransforming(e);
                inputHandler.IoInputTransformed += (s, e) => this.service.OnIoInputTransformed(e);
                inputHandler.ChainId = input.TransfRef;
                this.service.inputHandlers.Add(inputHandler);
                return inputHandler;
            }
        }

        private class VisualizationSerialPortFactory : SerialPortFactory
        {
            public VisualizationSerialPortFactory(IOVisualizationService service)
            {
            }

            public override SerialPortController CreateSerialPortController(SerialPortConfig config)
            {
                return null;
            }
        }
    }
}
