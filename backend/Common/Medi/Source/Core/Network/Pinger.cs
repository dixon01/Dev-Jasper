// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pinger.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Pinger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// Class that can be sued to send pings and broadcast pings.
    /// </summary>
    public class Pinger : IDisposable
    {
        private readonly IMessageDispatcher messageDispatcher;

        private readonly Dictionary<long, PingAsyncResult> requests = new Dictionary<long, PingAsyncResult>();
        private readonly Dictionary<long, BroadcastAsyncResult> broadcasts =
            new Dictionary<long, BroadcastAsyncResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Pinger"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher to use for this pinging.
        /// </param>
        public Pinger(IMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
            this.messageDispatcher.Subscribe<Pong>(this.HandlePong);
        }

        /// <summary>
        /// Should always be called once this object is no longer needed.
        /// </summary>
        public void Dispose()
        {
            this.messageDispatcher.Unsubscribe<Pong>(this.HandlePong);
        }

        #region Simple Ping

        /// <summary>
        /// Synchronously sends a ping request and waits infinitely for the response.
        /// </summary>
        /// <param name="destination">
        /// The destination of the ping.
        /// </param>
        /// <returns>
        /// the time the response took to arrive.
        /// </returns>
        public long Ping(MediAddress destination)
        {
            return this.Ping(destination, Timeout.Infinite);
        }

        /// <summary>
        /// Synchronously sends a ping request and waits the given time for the response.
        /// </summary>
        /// <param name="destination">
        /// The destination of the ping.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// the time the response took to arrive or -1 if the request timed out.
        /// </returns>
        public long Ping(MediAddress destination, int timeout)
        {
            var result = this.BeginPing(destination, r => { }, null);
            if (!result.AsyncWaitHandle.WaitOne(timeout, false))
            {
                return -1;
            }

            return this.EndPing(result);
        }

        /// <summary>
        /// Asynchronously sends a ping request.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// the async result that can be used to wait for the response.
        /// </returns>
        public IAsyncResult BeginPing(MediAddress destination, AsyncCallback callback, object state)
        {
            var result = new PingAsyncResult(callback, state);
            var ping = new Ping { Timestamp = TimeProvider.Current.TickCount };
            this.requests.Add(ping.Timestamp, result);
            this.messageDispatcher.Send(destination, ping);
            return result;
        }

        /// <summary>
        /// Asynchronous end method for the <see cref="BeginPing"/> method.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="BeginPing"/>.
        /// </param>
        /// <returns>
        /// the time the response took to arrive.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// if the argument is not one returned by <see cref="BeginPing"/>.
        /// </exception>
        public long EndPing(IAsyncResult result)
        {
            var pingResult = result as PingAsyncResult;
            if (pingResult == null)
            {
                throw new ArgumentException("Expected PingAsyncResult", "result");
            }

            return pingResult.RoundTripTime;
        }

        #endregion

        #region Broadcast Ping

        /// <summary>
        /// Synchronously sends a broadcast ping request and waits the given time for the response.
        /// </summary>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// An array of addresses of all hosts that responded to the ping request.
        /// </returns>
        public MediAddress[] BroadcastPing(TimeSpan timeout)
        {
            var result = this.BeginBroadcastPing(timeout, null, null);
            return this.EndBroadcastPing(result);
        }

        /// <summary>
        /// Asynchronously sends a broadcast ping request.
        /// </summary>
        /// <param name="timeout">
        /// The time to wait for responses after the given time the async result will be completed.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// the async result that can be used to wait for the response.
        /// </returns>
        public IAsyncResult BeginBroadcastPing(TimeSpan timeout, AsyncCallback callback, object state)
        {
            var result = new BroadcastAsyncResult(callback, state);
            var ping = new Ping { Timestamp = TimeProvider.Current.TickCount };
            this.broadcasts.Add(ping.Timestamp, result);
            this.messageDispatcher.Broadcast(ping);

            var timer = TimerFactory.Current.CreateTimer("BroadcastPing");
            timer.Interval = timeout;
            timer.AutoReset = false;
            timer.Elapsed += (sender, args) => result.Complete();
            timer.Enabled = true;
            return result;
        }

        /// <summary>
        /// Asynchronous end method for the <see cref="BeginBroadcastPing"/> method.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="BeginBroadcastPing"/>.
        /// </param>
        /// <returns>
        /// the time the response took to arrive.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// if the argument is not one returned by <see cref="BeginBroadcastPing"/>.
        /// </exception>
        public MediAddress[] EndBroadcastPing(IAsyncResult result)
        {
            var pingResult = result as BroadcastAsyncResult;
            if (pingResult == null)
            {
                throw new ArgumentException("Expected BroadcastAsyncResult", "result");
            }

            pingResult.WaitForCompletionAndVerify();
            return pingResult.Addresses.ToArray();
        }

        #endregion

        private void HandlePong(object sender, MessageEventArgs<Pong> e)
        {
            var timestamp = TimeProvider.Current.TickCount;
            PingAsyncResult result;
            if (this.requests.TryGetValue(e.Message.RequestTimestamp, out result))
            {
                this.requests.Remove(e.Message.RequestTimestamp);
                result.Complete(timestamp - e.Message.RequestTimestamp);
                return;
            }

            BroadcastAsyncResult broadcastResult;
            if (this.broadcasts.TryGetValue(e.Message.RequestTimestamp, out broadcastResult))
            {
                broadcastResult.Addresses.Add(e.Source);
            }
        }

        private class PingAsyncResult : AsyncResultBase
        {
            public PingAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            public long RoundTripTime { get; private set; }

            public void Complete(long roundTripTime)
            {
                this.RoundTripTime = roundTripTime;
                this.Complete(false);
            }
        }

        private class BroadcastAsyncResult : AsyncResultBase
        {
            public BroadcastAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Addresses = new List<MediAddress>();
            }

            public List<MediAddress> Addresses { get; private set; }

            public void Complete()
            {
                this.Complete(false);
            }
        }
    }
}
