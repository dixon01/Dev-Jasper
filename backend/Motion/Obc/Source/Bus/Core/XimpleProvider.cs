// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core
{
    using System;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;

    using NLog;

    /// <summary>
    /// Class responsible for creating Ximple from the current route state.
    /// </summary>
    public class XimpleProvider
    {
        private static readonly Logger Logger = LogHelper.GetLogger<XimpleProvider>();

        private readonly ITimer keepAliveTimer;

        private readonly XimpleCache ximpleCache = new XimpleCache();

        private readonly PortListener stopRequestedPort;

        private readonly PortListener doorsOpenPort;

        private bool running;

        private int currentStopIndex;
        private int lastStopCount;

        private bool? stopRequested;

        private bool drivingSchoolActive;

        private bool inStopBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleProvider"/> class.
        /// </summary>
        public XimpleProvider()
        {
            this.keepAliveTimer = TimerFactory.Current.CreateTimer("XimpleKeepAlive");
            this.keepAliveTimer.Interval = TimeSpan.FromSeconds(20);
            this.keepAliveTimer.AutoReset = false;
            this.keepAliveTimer.Elapsed += this.KeepAliveTimerOnElapsed;

            this.stopRequestedPort = new PortListener(MediAddress.Broadcast, "StopRequested");
            this.stopRequestedPort.ValueChanged += this.StopRequestedPortOnValueChanged;

            this.doorsOpenPort = new PortListener(MediAddress.Broadcast, "DoorsOpen");
            this.doorsOpenPort.ValueChanged += this.DoorsOpenPortOnValueChanged;

            // add the keep-alive cell
            this.AddCell(0, 0, 2, 0, "1");
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.keepAliveTimer.Enabled = true;

            MessageDispatcher.Instance.Subscribe<XimpleMessageRequest>(this.HandleRequest);

            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Subscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Subscribe<evAdvDelay>(this.HandleAdvDelay);
            MessageDispatcher.Instance.Subscribe<evDeviationDetected>(this.HandleDeviationDetected);
            MessageDispatcher.Instance.Subscribe<evDeviationStarted>(this.HandleDeviationStarted);
            MessageDispatcher.Instance.Subscribe<evDeviationEnded>(this.HandleDeviationEnded);
            MessageDispatcher.Instance.Subscribe<evZoneChanged>(this.HandleZoneChanged);

            this.doorsOpenPort.Start(TimeSpan.FromSeconds(10));
            this.stopRequestedPort.Start(TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            MessageDispatcher.Instance.Unsubscribe<XimpleMessageRequest>(this.HandleRequest);

            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Unsubscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Unsubscribe<evAdvDelay>(this.HandleAdvDelay);
            MessageDispatcher.Instance.Unsubscribe<evDeviationDetected>(this.HandleDeviationDetected);
            MessageDispatcher.Instance.Unsubscribe<evDeviationStarted>(this.HandleDeviationStarted);
            MessageDispatcher.Instance.Unsubscribe<evDeviationEnded>(this.HandleDeviationEnded);
            MessageDispatcher.Instance.Unsubscribe<evZoneChanged>(this.HandleZoneChanged);

            this.doorsOpenPort.Dispose();
            this.stopRequestedPort.Dispose();

            this.keepAliveTimer.Enabled = false;
        }

        private void UpdateStopIndex(int stopIndex)
        {
            try
            {
                this.AddCell(0, 10, 0, 0, RemoteEventHandler.CurrentTrip.LineName);
                this.AddCell(0, 10, 9, 0, RemoteEventHandler.CurrentTrip.RouteId.ToString());
                this.AddCell(0, 11, 0, 0, RemoteEventHandler.CurrentTrip.AnnonceExt);
                this.currentStopIndex = stopIndex;
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't handle stop reached event", ex);
            }
        }

        private void AddCell(int language, int table, int column, int row, string value)
        {
            this.ximpleCache.Add(
                new XimpleCell
                    {
                        LanguageNumber = language,
                        TableNumber = table,
                        ColumnNumber = column,
                        RowNumber = row,
                        Value = value ?? string.Empty
                    });
        }

        private void FlushXimple()
        {
            // ReSharper disable once IntroduceOptionalParameters.Local
            this.FlushXimple(null);
        }

        private void FlushXimple(MediAddress destination)
        {
            var ximple = this.ximpleCache.Dump();
            var row = 0;
            if (RemoteEventHandler.CurrentTrip != null)
            {
                for (int i = 0; i < RemoteEventHandler.CurrentTrip.Stop.Count; i++)
                {
                    if (i < this.currentStopIndex)
                    {
                        continue;
                    }

                    ximple.Cells.Add(
                        new XimpleCell
                            {
                                LanguageNumber = 0,
                                TableNumber = 12,
                                ColumnNumber = 0,
                                RowNumber = row++,
                                Value = RemoteEventHandler.CurrentTrip.Stop[i].Name1
                            });
                }
            }

            var rowCount = row;
            while (row < this.lastStopCount)
            {
                ximple.Cells.Add(
                    new XimpleCell
                        {
                            LanguageNumber = 0,
                            TableNumber = 12,
                            ColumnNumber = 0,
                            RowNumber = row++,
                            Value = string.Empty
                        });
            }

            if (destination == null)
            {
                // only update the count when we sent it to everybody
                this.lastStopCount = rowCount;
            }

            MessageDispatcher.Instance.Send(destination ?? MediAddress.Broadcast, ximple);
        }

        private void KeepAliveTimerOnElapsed(object sender, EventArgs e)
        {
            this.FlushXimple();
        }

        private void StopRequestedPortOnValueChanged(object sender, EventArgs e)
        {
            var newValue = this.stopRequestedPort.Value.Equals(FlagValues.True);
            if (this.stopRequested == newValue)
            {
                return;
            }

            this.stopRequested = newValue;
            this.AddCell(0, 0, 3, 0, newValue ? "1" : "0");
            this.FlushXimple();
        }

        private void HandleRequest(object sender, MessageEventArgs<XimpleMessageRequest> e)
        {
            this.FlushXimple(e.Source);
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            if (this.drivingSchoolActive || !this.inStopBuffer)
            {
                return;
            }

            if (RemoteEventHandler.CurrentTrip == null)
            {
                return;
            }

            try
            {
                this.UpdateStopIndex(0);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't handle trip loaded event", ex);
            }

            this.FlushXimple();
        }

        private void HandleServiceStarted(object sender, MessageEventArgs<evServiceStarted> e)
        {
            this.drivingSchoolActive = e.Message.School;
            if (e.Message.School || e.Message.ExtraService)
            {
                this.AddCell(0, 11, 0, 0, string.Empty);
            }

            this.currentStopIndex = int.MaxValue;
            this.AddCell(0, 10, 0, 0, string.Empty);
            this.AddCell(0, 10, 9, 0, string.Empty);
            this.FlushXimple();
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.UpdateStopIndex(int.MaxValue);
            this.FlushXimple();
        }

        private void HandleBusStopReached(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            this.UpdateStopIndex(e.Message.StopId);
            this.FlushXimple();
            this.inStopBuffer = true;
        }

        private void HandleBusStopLeft(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            this.UpdateStopIndex(e.Message.StopId + 1);
            this.FlushXimple();
            this.inStopBuffer = false;
        }

        private void HandleAdvDelay(object sender, MessageEventArgs<evAdvDelay> e)
        {
            this.AddCell(0, 10, 3, 0, e.Message.Delay.ToString());
            this.FlushXimple();
        }

        private void HandleDeviationDetected(object sender, MessageEventArgs<evDeviationDetected> e)
        {
            this.AddCell(0, 10, 12, 0, "1");
            this.FlushXimple();
        }

        private void HandleDeviationStarted(object sender, MessageEventArgs<evDeviationStarted> e)
        {
            this.AddCell(0, 10, 12, 0, "1");
            this.FlushXimple();
        }

        private void HandleDeviationEnded(object sender, MessageEventArgs<evDeviationEnded> e)
        {
            this.AddCell(0, 10, 12, 0, "0");
            this.FlushXimple();
        }

        private void DoorsOpenPortOnValueChanged(object sender, EventArgs e)
        {
            this.AddCell(0, 0, 4, 0, FlagValues.True.Equals(this.doorsOpenPort.Value) ? "1" : "0");
            this.FlushXimple();
        }

        private void HandleZoneChanged(object sender, MessageEventArgs<evZoneChanged> e)
        {
            this.AddCell(0, 10, 11, 0, e.Message.ZoneId.ToString());
        }
    }
}
