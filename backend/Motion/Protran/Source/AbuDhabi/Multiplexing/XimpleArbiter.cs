// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleArbiter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleArbiter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Multiplexing
{
    using System;
    using System.Collections.Generic;
    using System.Timers;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;

    using NLog;

    /// <summary>
    /// An arbiter implementation that has a primary and a secondary
    /// <see cref="IXimpleSource"/>. If the primary source doesn't emit
    /// any Ximple during a given <see cref="Timeout"/>, the arbiter
    /// automatically switches to the secondary source and updates
    /// all Ximple cells that were ever set in the secondary source.
    /// When the primary source emits Ximple again, this class switches
    /// back to that source (until the next timeout happens).
    /// </summary>
    public class XimpleArbiter : IXimpleSource
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly object locker = new object();
        private readonly IXimpleSource primary;
        private readonly IXimpleSource secondary;

        private readonly Dictionary<IXimpleSource, XimpleCache> caches =
            new Dictionary<IXimpleSource, XimpleCache>(2);

        private readonly Timer timeoutTimer = new Timer();

        private IXimpleSource currentSource;

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleArbiter"/> class
        /// with the two sources.
        /// </summary>
        /// <param name="primary">
        /// The primary source.
        /// </param>
        /// <param name="secondary">
        /// The secondary source.
        /// </param>
        public XimpleArbiter(IXimpleSource primary, IXimpleSource secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
            this.Timeout = 125 * 1000;

            this.primary.XimpleCreated += this.SourceOnXimpleCreated;
            this.secondary.XimpleCreated += this.SourceOnXimpleCreated;
            this.currentSource = this.primary;
            this.caches.Add(this.primary, new XimpleCache());
            this.caches.Add(this.secondary, new XimpleCache());

            this.timeoutTimer.Elapsed += this.TimeoutTimerOnElapsed;
        }

        /// <summary>
        /// Event that is fired whenever the this object creates
        /// a new ximple object.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired whenever the <see cref="CurrentSource"/>
        /// changed.
        /// </summary>
        public event EventHandler SourceChanged;

        /// <summary>
        /// Event that is fired whenever the secondary Ximple source fails.
        /// </summary>
        public event EventHandler SecondarySourceTimeout;

        /// <summary>
        /// Gets or sets the timeout after which this arbiter
        /// switches over to the secondary source.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the current source that is used.
        /// </summary>
        public IXimpleSource CurrentSource
        {
            get
            {
                return this.currentSource;
            }

            set
            {
                if (this.currentSource == value)
                {
                    // I avoid to set the same source twice.
                    return;
                }

                this.ChangeSource(value);
            }
        }

        /// <summary>
        /// Starts this arbiter. This will start the underlying timeout timer
        /// and will start forwarding Ximple created by its sources.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.RestartTimeout();
        }

        /// <summary>
        /// Stops this arbiter.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            this.timeoutTimer.Stop();
        }

        private void SourceOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            var source = sender as IXimpleSource;
            if (source == null || !this.running)
            {
                return;
            }

            lock (this.locker)
            {
                this.caches[source].Add(e.Ximple);

                if (source == this.primary && this.currentSource == this.secondary)
                {
                    this.RestartTimeout();

                    // we are in secondary, let's switch back
                    // this method will send out the necessary Ximple,
                    // we don't have to do it ourselves
                    this.ChangeSource(this.primary);
                }
                else if (this.currentSource == source)
                {
                    this.RestartTimeout();
                    this.RaiseXimpleCreated(e);
                }
            }
        }

        private void RestartTimeout()
        {
            this.timeoutTimer.Stop();
            this.timeoutTimer.Interval = this.Timeout;
            this.timeoutTimer.Start();
        }

        /// <summary>
        /// Changes to the given source.
        /// </summary>
        /// <param name="source">the new source</param>
        /// <exception cref="ArgumentException">if the current source is also the given source</exception>
        private void ChangeSource(IXimpleSource source)
        {
            if (this.currentSource == source)
            {
                throw new ArgumentException("Can't change the source to the current source", "source");
            }

            this.currentSource = source;
            Logger.Info("Changed source to {0}", source.GetType().Name);
            this.RaiseSourceChanged(EventArgs.Empty);

            // send all cached ximple data from the new source
            var cache = this.caches[source];
            var ximple = cache.Dump();
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseSourceChanged(EventArgs e)
        {
            var handler = this.SourceChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseSecondarySourceTimeout(EventArgs e)
        {
            var handler = this.SecondarySourceTimeout;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void TimeoutTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (this.currentSource == this.secondary)
            {
                Logger.Warn("Timeout on secondary source");
                this.RaiseSecondarySourceTimeout(EventArgs.Empty);
                return;
            }

            Logger.Warn("Timeout on primary source");
            this.ChangeSource(this.secondary);
        }
    }
}
