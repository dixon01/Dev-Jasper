// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemotePort.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemotePort type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;

    using Gorba.Common.Gioom.Core.Messages;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// <see cref="IPort"/> implementation for a port that resides on a remote Medi node.
    /// </summary>
    internal class RemotePort : PortBase
    {
        private static readonly TimeSpan RegistrationInterval = TimeSpan.FromSeconds(30);

        private readonly IPortInfo info;

        private readonly IMessageDispatcher dispatcher;

        private readonly ITimer registrationTimer;

        private IOValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotePort"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        /// <param name="dispatcher">
        /// The message dispatcher used for sending and receiving messages
        /// </param>
        public RemotePort(IPortInfo info, IOValue initialValue, IMessageDispatcher dispatcher)
        {
            this.info = info;
            this.value = initialValue;
            this.dispatcher = dispatcher;

            if (!info.CanRead)
            {
                return;
            }

            this.registrationTimer =
                TimerFactory.Current.CreateTimer(string.Format("RemotePort-{0}-{1}", info.Address, info.Name));
            this.registrationTimer.Elapsed += (s, e) => this.SendPortChangeRegistration();
            this.registrationTimer.AutoReset = true;
            this.registrationTimer.Interval = RegistrationInterval;
            this.registrationTimer.Enabled = true;

            this.dispatcher.Subscribe<PortChangeNotification>(this.HandlePortChangeNotification);

            this.SendPortChangeRegistration();
        }

        /// <summary>
        /// Gets the information about this port.
        /// </summary>
        public override IPortInfo Info
        {
            get
            {
                return this.info;
            }
        }

        /// <summary>
        /// Gets or sets the current I/O value of this port.
        /// Setting the value might not immediately change the value
        /// returned by the getter. Especially if the port is on a remote
        /// Medi node, it might take some time until the value actually changes.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// if the port is not readable (in the getter) or writable (in the setter).
        /// </exception>
        public override IOValue Value
        {
            get
            {
                if (!this.Info.CanRead)
                {
                    throw new NotSupportedException("Port can't be read: " + this.Info.Name);
                }

                return this.value;
            }

            set
            {
                if (!this.Info.CanWrite)
                {
                    throw new NotSupportedException("Port can't be written: " + this.Info.Name);
                }

                this.SendMessage(new PortChangeRequest { Name = this.Info.Name, Value = value.Value });
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            if (this.registrationTimer != null)
            {
                this.registrationTimer.Dispose();
            }

            this.dispatcher.Unsubscribe<PortChangeNotification>(this.HandlePortChangeNotification);
        }

        private void HandlePortChangeNotification(object sender, MessageEventArgs<PortChangeNotification> e)
        {
            if (e.Message.Name != this.Info.Name || !e.Source.Equals(this.Info.Address))
            {
                return;
            }

            this.value = this.CreateValue(e.Message.Value);
            this.RaiseValueChanged(EventArgs.Empty);
        }

        private void SendPortChangeRegistration()
        {
            var interval = TimeSpan.FromMilliseconds(RegistrationInterval.TotalMilliseconds * 1.5);
            this.SendMessage(
                new PortChangeRegistration { Name = this.Info.Name, Timeout = interval });
        }

        private void SendMessage(object message)
        {
            this.dispatcher.Send(this.Info.Address, message);
        }
    }
}