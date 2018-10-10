// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcMaster.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcMaster type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Master
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The AHDLC master.
    /// </summary>
    public class AhdlcMaster
    {
        private const int MaxResendCount = 3;

        private static readonly TimeSpan DefaultResponseWaitTime = TimeSpan.FromSeconds(1);

        private static readonly Logger Logger = LogHelper.GetLogger<AhdlcMaster>();

        private readonly Queue<LongFrameBase> sendQueue = new Queue<LongFrameBase>();

        private readonly IFrameHandler frameHandler;

        private readonly ITimer responseWaitTimer;

        private int inactiveSlaves;

        private bool running;

        private bool canSend;

        private FunctionCode waitingResponseCode;
        private int waitingResponseAddress;

        private LongFrameBase lastSentFrame;
        private int resendCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AhdlcMaster"/> class.
        /// </summary>
        /// <param name="frameHandler">
        /// The frame handler from which to read and write frames.
        /// </param>
        public AhdlcMaster(IFrameHandler frameHandler)
        {
            this.frameHandler = frameHandler;
            this.responseWaitTimer = TimerFactory.Current.CreateTimer("AhdlcResponseWait");
            this.responseWaitTimer.AutoReset = false;
            this.responseWaitTimer.Elapsed += this.ResponseWaitTimerOnElapsed;
        }

        /// <summary>
        /// Event that is fired whenever a <see cref="StatusResponseFrame"/> is received.
        /// </summary>
        public event EventHandler<StatusResponseEventArgs> StatusReceived;

        /// <summary>
        /// Gets or sets a value indicating whether to ignore responses.
        /// If responses are ignored, we will just send data (waiting for possible
        /// responses, but not evaluating them).
        /// </summary>
        public bool IgnoreResponses { get; set; }

        /// <summary>
        /// Gets or sets the time to wait when <see cref="IgnoreResponses"/> is set to true.
        /// This wait time will always be added to the estimated wait time.
        /// </summary>
        public TimeSpan IgnoreResponseTime { get; set; }

        /// <summary>
        /// Starts this master and starts the read thread.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.canSend = true;

            this.responseWaitTimer.Interval = this.IgnoreResponses
                                                  ? this.IgnoreResponseTime
                                                  : DefaultResponseWaitTime;

            Logger.Info(
                "Starting, ignore responses={0}, response time={1:0}ms",
                this.IgnoreResponses,
                this.responseWaitTimer.Interval.TotalMilliseconds);

            var thread = new Thread(this.Run);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        /// <summary>
        /// Stops this master.
        /// </summary>
        public void Stop()
        {
            this.running = false;
        }

        /// <summary>
        /// Enqueues a frame to be sent to slaves.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// if this master is not running.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if something is wrong with the <see cref="frame"/>.
        /// </exception>
        public void EnqueueFrame(LongFrameBase frame)
        {
            if (!this.running)
            {
                throw new NotSupportedException("Can't enqueue a frame when the master is not running");
            }

            if (!frame.IsFromMaster)
            {
                throw new ArgumentException(frame.GetType().Name + " can't be sent by master");
            }

            if (frame.Address < 1 || frame.Address > 15)
            {
                throw new ArgumentException("Frame address must be between 1 and 15");
            }

            lock (this.sendQueue)
            {
                if (!this.canSend)
                {
                    this.sendQueue.Enqueue(frame);
                    return;
                }

                this.canSend = false;
            }

            if (!this.SendFrame(frame))
            {
                this.SendNextFrame();
            }
        }

        /// <summary>
        /// Raises the <see cref="StatusReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStatusReceived(StatusResponseEventArgs e)
        {
            var handler = this.StatusReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Run()
        {
            while (this.running)
            {
                FrameBase frame;
                try
                {
                    frame = this.frameHandler.ReadNextFrame();
                }
                catch (FrameDecodingException ex)
                {
                    Logger.Warn(ex, "Couldn't decode frame");
                    continue;
                }
                catch (Exception ex)
                {
                    // if the stream was closed after this master was stopped, ignore the exception
                    if (this.running)
                    {
                        Logger.Warn(ex, "Couldn't decode frame");
                    }

                    continue;
                }

                if (!this.running)
                {
                    return;
                }

                if (frame == null)
                {
                    Logger.Warn("Got unexpected end of stream");
                    continue;
                }

                this.HandleReceivedFrame(frame);
            }
        }

        private void HandleReceivedFrame(FrameBase frame)
        {
            if (frame.IsFromMaster)
            {
                // ignore my own frames
                return;
            }

            if (frame.FunctionCode != this.waitingResponseCode)
            {
                Logger.Warn("Got unexpected frame (wanted {0}): {1}", this.waitingResponseCode, frame);
                return;
            }

            if (frame.Address != this.waitingResponseAddress)
            {
                Logger.Warn("Got unexpected frame from {0} instead of {1}", frame.Address, this.waitingResponseAddress);
                return;
            }

            Logger.Debug("Received {0}", frame);
            var statusResponse = frame as StatusResponseFrame;
            if (statusResponse != null)
            {
                // reactivate the slave if we got a status response
                this.inactiveSlaves &= 0xFFFF ^ (1 << this.lastSentFrame.Address);
                this.RaiseStatusReceived(new StatusResponseEventArgs(statusResponse));
            }

            lock (this.responseWaitTimer)
            {
                this.responseWaitTimer.Enabled = false;
            }

            this.SendNextFrame();
        }

        private bool SendFrame(LongFrameBase frame)
        {
            if (frame.FunctionCode != FunctionCode.StatusRequest && (this.inactiveSlaves & (1 << frame.Address)) != 0)
            {
                Logger.Info("Couldn't send {0} to {1} because the address is inactive", frame, frame.Address);
                return false;
            }

            this.waitingResponseCode = frame.GetResponseCode();
            this.waitingResponseAddress = frame.Address;
            this.lastSentFrame = frame;
            this.resendCount = 0;
            Logger.Debug("Sending {0}", frame);
            this.WriteFrame(frame);
            return true;
        }

        private void SendNextFrame()
        {
            LongFrameBase frame;
            do
            {
                lock (this.sendQueue)
                {
                    if (this.sendQueue.Count == 0)
                    {
                        this.waitingResponseCode = 0;
                        this.waitingResponseAddress = 0;
                        this.lastSentFrame = null;
                        this.canSend = true;
                        return;
                    }

                    frame = this.sendQueue.Dequeue();
                }
            }
            while (!this.SendFrame(frame));
        }

        private void ResponseWaitTimerOnElapsed(object sender, EventArgs e)
        {
            if (this.lastSentFrame == null)
            {
                return;
            }

            if (this.IgnoreResponses)
            {
                Logger.Trace("Ignoring responses, therefore trying to send next frame immediately");
                this.SendNextFrame();
                return;
            }

            this.resendCount++;
            if (this.resendCount >= MaxResendCount)
            {
                this.inactiveSlaves |= 1 << this.lastSentFrame.Address;
                Logger.Warn(
                    "Couldn't send {0} to {1} after {2} tries, deactivating address",
                    this.lastSentFrame,
                    this.lastSentFrame.Address,
                    this.resendCount);
                this.SendNextFrame();
                return;
            }

            Logger.Debug(
                "Couldn't send {0} to {1} after {2} tries, retrying",
                this.lastSentFrame,
                this.lastSentFrame.Address,
                this.resendCount);

            this.WriteFrame(this.lastSentFrame);
        }

        private void WriteFrame(FrameBase frame)
        {
            lock (this.responseWaitTimer)
            {
                try
                {
                    this.frameHandler.WriteFrame(frame);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't write frame: " + frame, ex);
                }

                this.responseWaitTimer.Enabled = true;
            }
        }
    }
}
