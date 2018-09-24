// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortListener.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Utility
{
    using System;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Helper class to listen to an <see cref="IPort"/>.
    /// This class will try to connect to the port with a given interval and will
    /// raise the <see cref="ValueChanged"/> event when the port was found and
    /// whenever its <see cref="IPort.Value"/> changes.
    /// </summary>
    public class PortListener : IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<PortListener>();

        private readonly MediAddress address;

        private readonly string name;

        private readonly ITimer retryTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortListener"/> class.
        /// </summary>
        /// <param name="address">
        /// The Medi address of the port.
        /// </param>
        /// <param name="name">
        /// The name of the port.
        /// </param>
        public PortListener(MediAddress address, string name)
        {
            this.address = address;
            this.name = name;
            
            this.retryTimer = TimerFactory.Current.CreateTimer(this.GetType().Name + "-" + name);
            this.retryTimer.AutoReset = false;
            this.retryTimer.Elapsed += (s, e) => this.BeginOpenPort();
        }

        /// <summary>
        /// Event that is risen whenever the port's value changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets the I/O value of the port or null if the port was not (yet) found.
        /// </summary>
        public IOValue Value
        {
            get
            {
                return this.Port == null ? null : this.Port.Value;
            }

            set
            {
                if (this.Port == null)
                {
                    return;
                }

                this.Port.Value = value;
            }
        }

        /// <summary>
        /// Gets the underlying port or null if the port was not (yet) found.
        /// </summary>
        public IPort Port { get; private set; }

        /// <summary>
        /// Gets the Port name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Starts this handler and tries to connect to the port.
        /// </summary>
        /// <param name="retryInterval">
        /// The retry interval.
        /// </param>
        public void Start(TimeSpan retryInterval)
        {
            Logger.Trace("Starting listener for '{0}' on {1} (retry={2})", this.name, this.address, retryInterval);
            this.retryTimer.Interval = retryInterval;
            this.BeginOpenPort();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.retryTimer.Enabled = false;

            if (this.Port == null)
            {
                return;
            }

            this.Port.ValueChanged -= this.PortOnValueChanged;
            this.Port.Dispose();
            this.Port = null;
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseValueChanged(EventArgs e)
        {
            Logger.Trace("Port {0} on {1} changed its value to '{2}'", this.name, this.address, this.Value);
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void BeginOpenPort()
        {
            GioomClient.Instance.BeginOpenPort(this.address, this.name, this.PortOpened, null);
        }

        private void PortOpened(IAsyncResult ar)
        {
            this.Port = GioomClient.Instance.EndOpenPort(ar);
            if (this.Port == null)
            {
                Logger.Debug(
                    "Couldn't find port {0} on {1}, trying again in {2:0} seconds",
                    this.name,
                    this.address,
                    this.retryTimer.Interval.TotalSeconds);
                this.retryTimer.Enabled = true;
                return;
            }

            Logger.Debug("Port {0} on {1} is now available", this.name, this.address);
            if (!this.Port.Info.CanRead)
            {
                return;
            }

            this.Port.ValueChanged += this.PortOnValueChanged;
            this.RaiseValueChanged(EventArgs.Empty);
        }

        private void PortOnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged(e);
        }
    }
}
