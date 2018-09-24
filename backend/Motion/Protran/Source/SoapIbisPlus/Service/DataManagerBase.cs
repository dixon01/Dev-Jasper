// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Base class for all data managers.
    /// </summary>
    internal abstract class DataManagerBase : IDisposable
    {
        /// <summary>
        /// Event that is fired when this manager created some Ximple.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Raises the <see cref="TripManager.XimpleCreated"/> event.
        /// </summary>
        /// <param name="args">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs args)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}