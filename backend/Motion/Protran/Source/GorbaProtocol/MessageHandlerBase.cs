// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.GorbaProtocol
{
    using System;

    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The message handler base.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    internal abstract class MessageHandlerBase<T> : IMessageHandler<T>
        where T : GorbaMessage
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerBase{T}"/> class.
        /// </summary>
        protected MessageHandlerBase()
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
        }

        /// <summary>
        /// The ximple created.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Processes the message, creating <see cref="Ximple"/> messages if required.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public abstract void ProcessMessage(T message);

        /// <summary>
        /// The raise ximple created.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        protected virtual void RaiseXimpleCreated(Ximple ximple)
        {
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        /// <summary>
        /// The raise ximple created.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }
    }
}