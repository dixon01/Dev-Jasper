// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceHandlerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Protocols.Ximple;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Base class for all service handlers.
    /// </summary>
    public abstract class ServiceHandlerBase
    {
        private IElementHandlerFactory elementHandlerFactory;

        /// <summary>
        /// Event that is fired whenever the underlying protocol gets new data from the virtual VDV 301 server.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<object>> DataUpdated;

        /// <summary>
        /// Event that is risen whenever a new <see cref="Ximple"/> object is created by this service handler.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the element handler factory.
        /// </summary>
        protected IElementHandlerFactory HandlerFactory
        {
            get
            {
                return this.elementHandlerFactory
                       ?? (this.elementHandlerFactory = ServiceLocator.Current.GetInstance<IElementHandlerFactory>());
            }
        }

        /// <summary>
        /// Configures this service handler.
        /// </summary>
        /// <param name="context">
        /// The handler context.
        /// </param>
        public abstract void Configure(IHandlerContext context);

        /// <summary>
        /// Starts this handler and registers the necessary callbacks.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops this handler and de-registers all callbacks.
        /// </summary>
        public abstract void Stop();

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
    }
}