// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateVisualizationHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StateVisualizationHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Handlers
{
    using System;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Core;

    using Math = System.Math;

    /// <summary>
    /// The icon handler.
    /// </summary>
    internal class StateVisualizationHandler
    {
        private readonly IContext context;

        private readonly IIconBar iconBar;

        private readonly SimplePort blockState;

        private readonly SimplePort tripState;

        private readonly SimplePort bufferState;

        private readonly ITimer trafficLightTimer;

        private readonly PortListener stopRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateVisualizationHandler"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal StateVisualizationHandler(IContext context)
        {
            this.context = context;
            this.iconBar = context.UiRoot.IconBar;

            this.blockState = new SimplePort("BlockState", true, false, new FlagValues(), FlagValues.False);
            GioomClient.Instance.RegisterPort(this.blockState);

            this.tripState = new SimplePort("TripState", true, false, new FlagValues(), FlagValues.False);
            GioomClient.Instance.RegisterPort(this.tripState);

            this.bufferState = new SimplePort("BufferState", true, false, new FlagValues(), FlagValues.False);
            GioomClient.Instance.RegisterPort(this.bufferState);

            this.stopRequested = new PortListener(MediAddress.Broadcast, "StopRequested");
            this.stopRequested.ValueChanged += this.StopRequestedOnValueChanged;
            this.stopRequested.Start(TimeSpan.FromSeconds(5));

            MessageDispatcher.Instance.Subscribe<evDeviationEnded>(this.EvDeviationEndedEvent);
            MessageDispatcher.Instance.Subscribe<evDeviationStarted>(this.EvDeviationStartedEvent);
            MessageDispatcher.Instance.Subscribe<evDeviationDetected>(this.EvDeviationDetectedEvent);

            MessageDispatcher.Instance.Subscribe<evRazziaEnded>(this.EvRazziaEndedEvent);
            MessageDispatcher.Instance.Subscribe<evRazziaStart>(this.EvRazziaStartEvent);

            MessageDispatcher.Instance.Subscribe<evSpeechConnected>(this.EvSpeechConnectedEvent);
            MessageDispatcher.Instance.Subscribe<evSpeechDisconnected>(this.EvSpeechDisconnectedEvent);
            MessageDispatcher.Instance.Subscribe<evSpeechRequested>(this.EvSpeechRequestedEvent);

            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.EvBusStopReachedEvent);
            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.EvBusStopLeftEvent);
            MessageDispatcher.Instance.Subscribe<evTripStarted>(this.EvTripStartedEvent);
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.EvTripLoadedEvent);
            MessageDispatcher.Instance.Subscribe<evTripEnded>(this.EvTripEndedEvent);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.EvServiceEnded);

            MessageDispatcher.Instance.Subscribe<evMessage>(this.EvMessageEvent);

            MessageDispatcher.Instance.Subscribe<evTrafficLight>(this.EvTrafficLight);

            this.trafficLightTimer = TimerFactory.Current.CreateTimer("TrafficLightIcon");
            this.trafficLightTimer.AutoReset = false;
            this.trafficLightTimer.Elapsed += this.TrafficLightTimerOnElapsed;

            this.context.StatusHandler.DriveInfo.IsAdditionalDriveChanged += this.DriveInfoOnIsAdditionalDriveChanged;
            this.context.StatusHandler.DriveInfo.IsDrivingSchoolChanged += this.DriveInfoOnIsDrivingSchoolChanged;
            this.context.StatusHandler.DriveInfo.DriveTypeChanged += this.DriveInfoOnDriveTypeChanged;

            if (this.context.AlarmHandler != null)
            {
                this.context.AlarmHandler.AlarmStateChanged += this.AlarmHandlerOnAlarmStateChanged;
            }
        }

        private void StopRequestedOnValueChanged(object sender, EventArgs eventArgs)
        {
            this.iconBar.SetStopRequestedIcon(FlagValues.True.Equals(this.stopRequested.Value));
        }

        private void AlarmHandlerOnAlarmStateChanged(object sender, EventArgs e)
        {
            switch (this.context.AlarmHandler.AlarmState)
            {
                case AlarmState.Reported:
                case AlarmState.Received:
                    // Show alarm reported icon
                    this.iconBar.SetDriverAlarmIcon(DriverAlarmIconState.Sent);
                    break;

                case AlarmState.Confirmed:
                    // Show alarm confirmed icon
                    this.iconBar.SetDriverAlarmIcon(DriverAlarmIconState.Acknowledged);
                    break;

                ////case AlarmState.Inactive:
                default:
                    // Hide the alarm icon
                    this.iconBar.SetDriverAlarmIcon(DriverAlarmIconState.None);
                    break;
            }
        }

        private void DriveInfoOnIsAdditionalDriveChanged(object sender, EventArgs eventArgs)
        {
            this.iconBar.SetAdditionalTripIcon(this.context.StatusHandler.DriveInfo.IsAdditionalDrive);
        }

        private void DriveInfoOnIsDrivingSchoolChanged(object sender, EventArgs eventArgs)
        {
            this.iconBar.SetDrivingSchoolIcon(this.context.StatusHandler.DriveInfo.IsDrivingSchool);
        }

        private void DriveInfoOnDriveTypeChanged(object sender, EventArgs eventArgs)
        {
            this.blockState.Value = FlagValues.GetValue(
                this.context.StatusHandler.DriveInfo.DriveType != DriveType.None);
        }

        private void EvTrafficLight(object sender, MessageEventArgs<evTrafficLight> e)
        {
            this.trafficLightTimer.Enabled = false;
            switch (e.Message.State)
            {
                case TrafficLightState.Requested:
                    this.iconBar.SetTrafficLightIcon(TrafficLightIconState.Requested);
                    break;
                case TrafficLightState.Received:
                    this.iconBar.SetTrafficLightIcon(TrafficLightIconState.Received);
                    break;
                default:
                    this.iconBar.SetTrafficLightIcon(TrafficLightIconState.None);
                    break;
            }

            this.trafficLightTimer.Interval = TimeSpan.FromSeconds(Math.Max(e.Message.ApproachTime, 1));
            this.trafficLightTimer.Enabled = true;
        }

        private void TrafficLightTimerOnElapsed(object sender, EventArgs e)
        {
            this.iconBar.SetTrafficLightIcon(TrafficLightIconState.None);
        }

        private void EvMessageEvent(object sender, MessageEventArgs<evMessage> e)
        {
            if (e.Message.SubType == evMessage.SubTypes.Error)
            {
                this.iconBar.SetAlarmMessageIcon(true);
            }
            else
            {
                this.iconBar.SetInformationMessageIcon(true);
            }
        }

        private void EvTripEndedEvent(object sender, MessageEventArgs<evTripEnded> e)
        {
            this.tripState.Value = FlagValues.False;
        }

        private void EvServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.tripState.Value = FlagValues.False;
            this.bufferState.Value = FlagValues.False;
        }

        private void EvBusStopLeftEvent(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            this.bufferState.Value = FlagValues.False;
        }

        private void EvTripStartedEvent(object sender, MessageEventArgs<evTripStarted> e)
        {
            this.tripState.Value = FlagValues.True;
        }

        private void EvTripLoadedEvent(object sender, MessageEventArgs<evTripLoaded> e)
        {
            // TODO: make trip "blink"
            this.blockState.Value = FlagValues.True;
        }

        private void EvBusStopReachedEvent(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            this.bufferState.Value = FlagValues.True;
        }

        private void EvSpeechRequestedEvent(object sender, MessageEventArgs<evSpeechRequested> e)
        {
            VoiceIconState icon = VoiceIconState.Requested;
            this.iconBar.SetVoiceIcon(icon);
        }

        private void EvSpeechDisconnectedEvent(object sender, MessageEventArgs<evSpeechDisconnected> e)
        {
            VoiceIconState icon = VoiceIconState.None;
            this.iconBar.SetVoiceIcon(icon);
        }

        private void EvSpeechConnectedEvent(object sender, MessageEventArgs<evSpeechConnected> e)
        {
            VoiceIconState icon = VoiceIconState.Connected;
            this.iconBar.SetVoiceIcon(icon);
        }

        private void EvRazziaStartEvent(object sender, MessageEventArgs<evRazziaStart> e)
        {
            this.iconBar.SetRazziaIcon(true);
        }

        private void EvRazziaEndedEvent(object sender, MessageEventArgs<evRazziaEnded> e)
        {
            this.iconBar.SetRazziaIcon(false);
        }

        private void EvDeviationDetectedEvent(object sender, MessageEventArgs<evDeviationDetected> e)
        {
            this.iconBar.SetDetourIcon(true);
        }

        private void EvDeviationStartedEvent(object sender, MessageEventArgs<evDeviationStarted> e)
        {
            this.iconBar.SetDetourIcon(true);
        }

        private void EvDeviationEndedEvent(object sender, MessageEventArgs<evDeviationEnded> e)
        {
            this.iconBar.SetDetourIcon(false);
        }
    }
}