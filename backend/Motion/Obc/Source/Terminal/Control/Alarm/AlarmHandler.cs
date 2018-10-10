// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Alarm
{
    using System;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Communication;

    using NLog;

    /// <summary>
    /// The alarm handler.
    /// </summary>
    internal class AlarmHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<AlarmHandler>();

        private readonly WanManager wanManager;

        private readonly PortListener portListener;

        private readonly ITimer alarmTimer;

        private int alarmIdToSend = GenericAlarms.GenericAlarm;

        private AlarmState alarmState = AlarmState.Inactive;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmHandler"/> class.
        /// </summary>
        /// <param name="input">
        /// Input name to be used.
        /// </param>
        /// <param name="wanManager">
        /// The WAN manager.
        /// </param>
        public AlarmHandler(string input, WanManager wanManager)
        {
            this.wanManager = wanManager;

            if (input != null)
            {
                this.portListener = new PortListener(MediAddress.Broadcast, input);
                this.portListener.ValueChanged += this.PortListenerOnValueChanged;
                this.portListener.Start(TimeSpan.FromSeconds(5));

                this.alarmTimer = TimerFactory.Current.CreateTimer("AlarmTimer");
                this.alarmTimer.Interval = TimeSpan.FromSeconds(1);
                this.alarmTimer.Elapsed += this.AlarmTimerOnElapsed;
            }

            this.SetAlarmState(AlarmState.Inactive, GenericAlarms.DistressAlarm);
            MessageDispatcher.Instance.Subscribe<evDriverAlarmAck>(this.EvDriverAlarmAckEvent);
            MessageDispatcher.Instance.Subscribe<evDistressAlarm>(this.EvDistressAlarmEvent);
        }

        /// <summary>
        /// Event that is risen every time the <see cref="AlarmState"/> changes.
        /// </summary>
        public event EventHandler AlarmStateChanged;

        /// <summary>
        /// Gets the current alarm state.
        /// </summary>
        /// <value>
        ///   The <see cref="AlarmState"/>.
        /// </value>
        public AlarmState AlarmState
        {
            get
            {
                return this.alarmState;
            }

            private set
            {
                if (this.alarmState == value)
                {
                    return;
                }

                this.alarmState = value;
                this.RaiseAlarmStateChanged();
            }
        }

        /// <summary>
        ///   Sets and Sends an Alarm State to Medi with the last used alarmID
        /// </summary>
        /// <param name = "state">The new alarm state</param>
        public void SetAlarmState(AlarmState state)
        {
            this.SetAlarmState(state, this.alarmIdToSend);
        }

        /// <summary>
        ///   Sets and Sends an alarm state with specific alarmID. See GenericAlarms for alarmID. If alarm state
        ///   is Inactive the alarmID will be override to 0. Means no alarm
        /// </summary>
        /// <param name = "state">The alarm state</param>
        /// <param name = "alarmId">The alarm ID</param>
        public void SetAlarmState(AlarmState state, int alarmId)
        {
            evDriverAlarm alarm;
            lock (this)
            {
                Logger.Debug("Change alarm {0} -> {1}", this.alarmState, state);
                this.AlarmState = state;
                this.UpdateEmergencyMode();
                this.alarmIdToSend = this.alarmState == AlarmState.Inactive ? 0 : alarmId;
                alarm = new evDriverAlarm(this.alarmState, this.alarmIdToSend);
            }

            MessageDispatcher.Instance.Broadcast(alarm);
        }

        /// <summary>
        ///   True if Vehicle/Terminal is in alarm mode/state
        /// </summary>
        /// <returns>True if the alarm is active.</returns>
        public bool IsAlarmActive()
        {
            return this.AlarmState != AlarmState.Inactive;
        }

        /// <summary>
        /// Raises the <see cref="AlarmStateChanged"/> event.
        /// </summary>
        protected virtual void RaiseAlarmStateChanged()
        {
            var handler = this.AlarmStateChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateEmergencyMode()
        {
            if (this.wanManager == null)
            {
                return;
            }

            switch (this.AlarmState)
            {
                case AlarmState.Reported:
                case AlarmState.Received:
                case AlarmState.Confirmed:
                    this.wanManager.EmergencyModeEnable();
                    break;
                default:
                    this.wanManager.EmergencyModeDisable();
                    break;
            }
        }

        private void AlarmTimerOnElapsed(object sender, EventArgs e)
        {
            this.SetAlarmState(AlarmState.Reported, GenericAlarms.DistressAlarm);
        }

        private void PortListenerOnValueChanged(object sender, EventArgs e)
        {
            this.alarmTimer.Enabled = false;
            if (FlagValues.True.Equals(this.portListener.Value))
            {
                this.alarmTimer.Enabled = true;
            }
            else if (this.AlarmState == AlarmState.Ended)
            {
                // This only happens for on/off type alarm buttons
                this.SetAlarmState(AlarmState.Inactive, GenericAlarms.DistressAlarm);
            }
        }

        private void EvDriverAlarmAckEvent(object sender, MessageEventArgs<evDriverAlarmAck> messageEventArgs)
        {
            AlarmState state;
            switch (messageEventArgs.Message.State)
            {
                case AlarmAck.Received:
                    state = AlarmState.Received;
                    break;
                case AlarmAck.Confirmed:
                    state = AlarmState.Confirmed;
                    break;
                case AlarmAck.Ended:
                    state = this.alarmIdToSend == GenericAlarms.DistressAlarm && this.portListener != null
                                   ? AlarmState.Ended
                                   : AlarmState.Inactive;
                    break;
                default:
                    Logger.Error("Not yet implemented AlarmACK type: {0}", messageEventArgs.Message.State);
                    state = AlarmState.Inactive;
                    break;
            }

            this.SetAlarmState(state);
        }

        private void EvDistressAlarmEvent(object sender, MessageEventArgs<evDistressAlarm> messageEventArgs)
        {
            if (messageEventArgs.Message.AlarmSet)
            {
                this.SetAlarmState(AlarmState.Reported, GenericAlarms.DistressAlarm);
            }
        }
    }
}