// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301VisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301VisualizationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Vdv301
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Common.IbisIP.Discovery;
    using Gorba.Motion.Protran.Vdv301;
    using Gorba.Motion.Protran.Vdv301.Handlers;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The VDV 301 visualization service implementation.
    /// </summary>
    internal class Vdv301VisualizationService : IVdv301VisualizationService
    {
        private readonly VisualizerElementHandlerFactory handlerFactory;

        private Vdv301Protocol protocol;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301VisualizationService"/> class.
        /// </summary>
        public Vdv301VisualizationService()
        {
            this.IbisServiceLocator = new VisualizerIbisServiceLocator();

            this.handlerFactory = new VisualizerElementHandlerFactory();
            this.handlerFactory.ElementTransformed += this.HandlerFactoryOnElementTransformed;
        }

        /// <summary>
        /// Event that is fired whenever the underlying protocol gets new data from the virtual VDV 301 server.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<object>> DataUpdated;

        /// <summary>
        /// Event that is fired whenever the underlying protocol transforms an element.
        /// </summary>
        public event EventHandler<TransformationChainEventArgs> ElementTransformed;

        /// <summary>
        /// Event that is fired whenever the underlying protocol creates Ximple to be sent to Infomedia.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public Dictionary Dictionary
        {
            get
            {
                return this.Protocol.Dictionary;
            }
        }

        /// <summary>
        /// Gets the IBIS service locator for this service.
        /// </summary>
        public VisualizerIbisServiceLocator IbisServiceLocator { get; private set; }

        private Vdv301Protocol Protocol
        {
            get
            {
                lock (this)
                {
                    if (this.protocol == null)
                    {
                        this.protocol = ServiceLocator.Current.GetInstance<Vdv301Protocol>();
                        this.protocol.DataUpdated += this.ProtocolOnDataUpdated;
                        this.protocol.XimpleCreated += this.ProtocolOnXimpleCreated;
                    }
                }

                return this.protocol;
            }
        }

        /// <summary>
        /// Register a new instance of this class with the service container.
        /// This also registers all special factories used to
        /// intercept events inside the VDV 301 protocol.
        /// </summary>
        public static void Register()
        {
            Core.Protran.SetupCoreServices();
            var service = new Vdv301VisualizationService();
            service.DoRegister();
        }

        /// <summary>
        /// Raises the <see cref="DataUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDataUpdated(DataUpdateEventArgs<object> e)
        {
            var handler = this.DataUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ElementTransformed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTelegramTransformed(TransformationChainEventArgs e)
        {
            var handler = this.ElementTransformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void DoRegister()
        {
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance<IVdv301VisualizationService>(this);
            serviceContainer.RegisterInstance<IIbisServiceLocator>(this.IbisServiceLocator);
            serviceContainer.RegisterInstance<IElementHandlerFactory>(this.handlerFactory);
        }

        private void ProtocolOnDataUpdated(object sender, DataUpdateEventArgs<object> e)
        {
            this.RaiseDataUpdated(e);
        }

        private void HandlerFactoryOnElementTransformed(object sender, TransformationChainEventArgs e)
        {
            if (e.Transformations.Length <= 1)
            {
                // we don't want to show default transformations
                return;
            }

            this.RaiseTelegramTransformed(e);
        }

        private void ProtocolOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            this.RaiseXimpleCreated(e);
        }
    }
}
