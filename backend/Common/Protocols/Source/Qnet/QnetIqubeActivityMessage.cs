// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetIqubeActivityMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of the qnet vdv messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Implementation of the qnet vdv messages
    /// </summary>
    public class QnetIqubeActivityMessage : QnetMessageBase
    {
        /// <summary>
        /// Contains data of the qnet message for iqube command message
        /// </summary>
        private QnetMessageStruct iqubeActivityMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetIqubeActivityMessage"/> class.
        /// </summary>
        /// <param name="srcAddr">
        /// The qnet source address of the sender of the message.
        /// </param>
        /// <param name="destAddr">
        /// The qnet destination address of the message.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetIqubeActivityMessage(ushort srcAddr, ushort destAddr, ushort gatewayAddress)
            : base(srcAddr, destAddr, gatewayAddress)
        {
            this.iqubeActivityMessage = new QnetMessageStruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetIqubeActivityMessage"/> class.
        /// The source and destination addresses are set with <see cref="QnetConstantes.QnetAddrNone"/> by default.
        /// </summary>
        public QnetIqubeActivityMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        /// <summary>
        /// Gets the qnet message for iqube command message.
        /// </summary>
        public QnetMessageStruct IqubeActivityMessage
        {
            get
            {
                return this.iqubeActivityMessage;
            }
        }

        /// <summary>
        /// Fill the right fields of the iqube command message structure for the task to send info text message
        /// </summary>
        /// <param name="taskId">
        /// The task Id.
        /// </param>
        /// <param name="rowId">
        /// The row Id.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        /// <param name="align">
        /// The align.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="scroll">
        /// The scroll.
        /// </param>
        /// <param name="side">
        /// Side where the text will be displayed
        /// <value>
        /// 1 Front
        /// </value>
        /// <value>
        /// 2 Back
        /// </value>
        /// <value>
        /// 3 Both
        /// </value>
        /// </param>
        /// <param name="startDate">
        /// The optional start date. If <b>null</b>, the task should start immediately.
        /// </param>
        /// <param name="stopDate">
        /// The optional stop date. If <b>null</b>, the task is valid until explicit revoke.
        /// </param>
        /// <param name="isScheduledDaily">
        /// Flag to indicate whether the task should be scheduled daily.
        /// </param>
        public void SetInfoLineTextActivity(
            uint taskId,
            byte rowId,
            string text,
            bool blink,
            byte align,
            sbyte flags,
            sbyte font,
            bool scroll,
            byte side,
            DateTime? startDate,
            DateTime? stopDate,
            bool isScheduledDaily)
        {
            this.FillQnetMessageHeader();

            this.iqubeActivityMessage.Dta.IqubeCmdMsg.CommandCode = (byte)IqubeCommandCode.ActivityCommand;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.SpecificParameter = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode = (sbyte)IqubeTaskCode.TaskInfoData;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id = taskId;

            this.SetStartAndStopDates(startDate, stopDate, isScheduledDaily, 0, 0);

            // Infoline
            // The InfoLineRowdId contains also the side of the iqube
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.InfoLineRowId = (byte)(side << 4);
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.InfoLineRowId += rowId;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.DataType = (sbyte)InfoLineDataType.Data;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Text = text;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Blink = (byte)(blink ? 9 : 0);

            // 9 = INFOLINE_BLINK_NORMAL
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Align = align;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Flags = flags;

            // => for eventually future use:
            // this.iqubeCommandMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Font = font;
            // Set the default font to the fon because other font are never used.
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Font = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.ReservedAsBoolean = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.ReservedAsWord = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Scroll = (byte)(scroll ? 16 : 0);

            // 16 = INFOLINE_SCROLL_AUTO
        }

        /// <summary>
        /// Fill the right fields of the iqube command message structure to request the iqube for registered
        /// activities.
        /// </summary>
        public void SetGetActivityIdsCmd()
        {
            this.FillQnetMessageHeader();
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.CommandCode = (byte)IqubeCommandCode.GetActivityIdsCommand;
        }

        /// <summary>
        /// Fill the right fields of the iqube command message structure for the task to send revoke message.
        /// </summary>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        public void SetRevokeTask(uint taskId)
        {
            this.FillQnetMessageHeader();
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.CommandCode = (byte)IqubeCommandCode.ActivityCommand;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.SpecificParameter = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode = (sbyte)IqubeTaskCode.ActivityRevoke;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id = taskId;

            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent =
                (sbyte)ExecutionCondition.Immediately;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent =
                (sbyte)ExecutionCondition.Immediately;
        }

        /// <summary>
        /// Fill the right fields of the iqube command message structure for the task to send display on/off message.
        /// </summary>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <param name="displayOn">
        /// Flag indicates if the display has to turn On or off.
        /// </param>
        /// <param name="specialText">
        /// Text that replaces the destination if DisplayMode equals
        /// <see cref="DisplayMode.ReplaceDestWithSpecialText"/>.
        /// </param>
        /// <param name="startDate">
        /// The optional start date. If <b>null</b>, the task should start immediately.
        /// </param>
        /// <param name="stopDate">
        /// The optional stop date. If <b>null</b>, the task is valid until explicit revoke.
        /// </param>
        /// <param name="isScheduledDaily">
        /// Flag to indicate whether the task should be scheduled daily.
        /// </param>
        /// <param name="displayMode">
        /// Indicates how handle the display. See <see cref="DisplayMode"/>
        /// </param>
        /// <param name="lineId">
        /// The line Id for which one the trips are hidden or displayed.
        /// </param>
        public void SetDisplayOnOffActivity(
            uint taskId,
            bool displayOn,
            string specialText,
            DateTime? startDate,
            DateTime? stopDate,
            bool isScheduledDaily,
            DisplayMode displayMode,
            ushort lineId)
        {
            this.SetOnOffActivity(taskId, displayOn, startDate, stopDate, isScheduledDaily, displayMode);

            if (lineId > 0)
            {
                this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Display.ActivityData.LineId = lineId;
            }

            if (displayMode == DisplayMode.ReplaceDestWithSpecialText)
            {
                // TODO Verify this with Markus
                this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Display.Text1 = specialText;
            }
        }

        /// <summary>
        /// Fills the right fields of the task to set text voice for TTS.
        /// </summary>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <param name="voiceText">
        /// The voice text to send.
        /// </param>
        /// <param name="interval">
        /// The interval in seconds between two announcement. Could be set with 0.
        /// </param>
        /// <param name="executionSchedule">
        /// The execution schedule context to define start and stop date/time.
        /// </param>
        public void SetVoiceTextActivity(
            uint taskId, string voiceText, ushort interval, ExecutionScheduleContext executionSchedule)
        {
            this.FillQnetMessageHeader();
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.CommandCode = (byte)IqubeCommandCode.ActivityCommand;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.SpecificParameter = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode = (sbyte)IqubeTaskCode.ActivityVoiceText;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id = taskId;

            this.SetStartAndStopDates(executionSchedule);

            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.VoiceText.Text = voiceText;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.VoiceText.Interval = interval;
        }

        /// <summary>
        /// Fill the right fields of the iqube command message structure for the task to send revoke message.
        /// </summary>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <param name="itcsProviderId">
        /// ITCS provider identifier. if set to 0, this parameter won't be evaluated during the deletion process on
        /// iqube.
        /// </param>
        /// <param name="lineId">
        /// Line identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </param>
        /// <param name="directionId">
        /// Direction identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </param>
        /// <param name="tripId">
        /// Trip identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </param>
        /// <param name="executionSchedule">
        /// The execution schedule context to define start and stop date/time.
        /// </param>
        public void SetDeleteTripActicity(
            uint taskId,
            ushort itcsProviderId,
            uint lineId,
            ushort directionId,
            uint tripId,
            ExecutionScheduleContext executionSchedule)
        {
            this.FillQnetMessageHeader();
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.CommandCode = (byte)IqubeCommandCode.ActivityCommand;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.SpecificParameter = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode = (sbyte)IqubeTaskCode.ActivityDeleteTrip;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id = taskId;

            this.SetStartAndStopDates(executionSchedule);

            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.DeleteTrip.ItcsProviderId = itcsProviderId;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.DeleteTrip.LineId = lineId;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.DeleteTrip.DirectionId = directionId;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.DeleteTrip.TripId = tripId;
        }

        /// <summary>
        ///  Returns the length of the message in bytes [0..255]. Represents the length of the data of the qnet message
        /// </summary>
        /// <returns>
        /// Type : byte
        /// Data length in bytes.
        /// </returns>
        protected override byte GetDataLenght()
        {
            return MessageConstantes.IqubeCommandMessageLength;
        }

        /// <summary>
        /// Fill the qnet message header
        /// </summary>
        private void FillQnetMessageHeader()
        {
            this.iqubeActivityMessage.Hdr.Type = (byte)MSGtyp.MsgTypIquCmd;
            this.iqubeActivityMessage.Hdr.SubTyp = 0;
            this.iqubeActivityMessage.Hdr.TimeStruct = DosDateTime.DatetimeToDosDateTime(DateTime.Now);
        }

        private void SetStartAndStopDates(ExecutionScheduleContext executionSchedule)
        {
            this.SetStartAndStopDates(
                executionSchedule.StartDate,
                executionSchedule.StopDate,
                executionSchedule.IsScheduledDaily,
                executionSchedule.NumberOfTimes,
                0);
        }

        /// <summary>
        /// Set the Start and stop date time.
        /// When the start or stop condition is <see cref="ExecutionCondition.Daily"/> then set the daySecs.
        /// </summary>
        /// <param name="startDate">
        /// Start date. Could be set to null
        /// </param>
        /// <param name="stopDate">
        /// Stop date. Could be set to null
        /// </param>
        /// <param name="isScheduledDaily">
        /// Indicates if the event is a daily event. If yes, set the day seconds field otherwise set the date and time
        /// with DOS date time values.
        /// </param>
        /// <param name="times">
        /// Indicates the number of repetitions of the execution of the activity.
        /// </param>
        /// <param name="duration">
        /// Indicates the duration if the activity (in second or in minute - TODO: verify this with Markus).
        /// </param>
        private void SetStartAndStopDates(
            DateTime? startDate, DateTime? stopDate, bool isScheduledDaily, ushort times, ushort duration)
        {
            if (isScheduledDaily)
            {
                Contract.Assert(
                    startDate.HasValue, "If the task is scheduled daily, the start date must have a value.");

                // Avoid the StyleCop message :
                if (startDate.HasValue)
                {
                    var daySecs = DaySecTime.DaySecFromDateTime(startDate.Value);
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartDaySeconds = daySecs;
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent =
                        (sbyte)ExecutionCondition.Daily;
                }

                Contract.Assert(stopDate.HasValue, "If the task is scheduled daily, the stop date must have a value.");

                // Avoid the StyleCop message :
                if (stopDate.HasValue)
                {
                    var daySecs = DaySecTime.DaySecFromDateTime(stopDate.Value);
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.DaySecs = daySecs;
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent =
                        (sbyte)ExecutionCondition.Daily;
                }
            }
            else
            {
                if (startDate.HasValue)
                {
                    var start = DosDateTime.DatetimeToDosDateTime(startDate.Value);
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartTime.Date = start.Date;
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartTime.Time = start.Time;
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent =
                        (sbyte)ExecutionCondition.DateTime;
                }
                else
                {
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent =
                        (sbyte)ExecutionCondition.Immediately;
                }

                if (stopDate.HasValue)
                {
                    var stop = DosDateTime.DatetimeToDosDateTime(stopDate.Value);
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.Time.Date = stop.Date;
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.Time.Time = stop.Time;
                    this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent =
                        (sbyte)ExecutionCondition.DateTime;
                }
                else
                {
                    if (times > 0)
                    {
                        this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent =
                            (sbyte)ExecutionCondition.SeveralTimes;
                        this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.Count.MaxTimes = times;
                    }
                    else
                    {
                        if (duration > 0)
                        {
                            // TODO : Confirm this with Markus to be sure that the duration should be set into DaySecs
                            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.DaySecs = duration;
                            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent =
                                (sbyte)ExecutionCondition.Duration;
                        }
                        else
                        {
                            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent =
                                (sbyte)ExecutionCondition.UntilAbort;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill the right fields of the iqube command message structure for the task to send display on/off message.
        /// </summary>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <param name="displayOn">
        /// Flag indicates if the display has to turn On or off.
        /// </param>
        /// <param name="startDate">
        /// The optional start date. If <b>null</b>, the task should start immediately.
        /// </param>
        /// <param name="stopDate">
        /// The optional stop date. If <b>null</b>, the task is valid until explicit revoke.
        /// </param>
        /// <param name="isScheduledDaily">
        /// Flag to indicate whether the task should be scheduled daily.
        /// </param>
        /// <param name="displayMode">
        /// Indicates how handle the display. See <see cref="DisplayMode"/>
        /// </param>
        private void SetOnOffActivity(
            uint taskId,
            bool displayOn,
            DateTime? startDate,
            DateTime? stopDate,
            bool isScheduledDaily,
            DisplayMode displayMode)
        {
            if (displayMode == DisplayMode.None || displayMode == DisplayMode.Max)
            {
                const string Format =
                    "SetOnOffActivity, bad argument value: displayMode must be diffrent than"
                    + " DisplayMode.None and DisplayMode.Max";
                throw new QnetProtocolStackException(Format);
            }

            if (displayOn && displayMode != DisplayMode.Normal)
            {
                displayMode = DisplayMode.Normal;
            }

            if (!displayOn && displayMode == DisplayMode.Normal)
            {
                const string Message =
                    "SetOnOffActivity, bad argument combination: if the displayOn is true, then the"
                    + " displayMode should be different than DisplayMode.Normal";
                throw new QnetProtocolStackException(Message);
            }

            this.FillQnetMessageHeader();
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.CommandCode = (byte)IqubeCommandCode.ActivityCommand;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.SpecificParameter = 0;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode = displayOn
                                                                                 ? (sbyte)
                                                                                   IqubeTaskCode.ActivityDisplayOn
                                                                                 : (sbyte)
                                                                                   IqubeTaskCode.ActivityDisplayOff;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Display.ActivityData.ushortParam1 = (ushort)displayMode;
            this.iqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id = taskId;

            this.SetStartAndStopDates(startDate, stopDate, isScheduledDaily, 0, 0);
        }
    }
}