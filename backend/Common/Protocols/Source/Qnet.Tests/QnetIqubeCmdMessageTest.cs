// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetIqubeCmdMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for QnetIqubeCmdMessageTest and is intended
//   to contain all QnetIqubeCmdMessageTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System;
    using System.Text;

    using Gorba.Common.Protocols.Qnet;
    using Gorba.Common.Utility.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for QnetIqubeCmdMessageTest and is intended
    /// to contain all QnetIqubeCmdMessageTest Unit Tests
    /// </summary>
    [TestClass]
    public class QnetIqubeCmdMessageTest
    {
        /// <summary>
        /// UtcNow value used for testing.
        /// </summary>
        private static readonly DateTime UtcNow = new DateTime(2012, 2, 19, 22, 46, 20);

        /// <summary>
        /// Cleanups this test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            TimeProvider.ResetToDefault();
        }

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var timeProviderMock = new Mock<TimeProvider>();
            timeProviderMock.SetupGet(t => t.UtcNow).Returns(UtcNow);
            TimeProvider.Current = timeProviderMock.Object;
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// A test for SetTaskForInfoLineText_BlinkTest
        /// </summary>
        [TestMethod]
        public void SetTaskForInfoLineText_BlinkTest()
        {
            var target = new QnetIqubeActivityMessage(); 
            const uint TaskId = 1; 
            const byte RowId = 1; 
            const string Text = "Test Rückseite"; 
            const bool BlinkTrue = true; 
            const bool BlinkFalse = false;
            const byte Align = 1; 
            const sbyte Flags = 0; 
            const sbyte Font = 0;
            const bool Scroll = true; 
            const byte Side = 3;
            var start = TimeProvider.Current.UtcNow;
            var stop = start.AddDays(1);
            const bool IsScheduledDaily = true;
            target.SetInfoLineTextActivity(TaskId, RowId, Text, BlinkTrue, Align, Flags, Font, Scroll, Side, start, stop, IsScheduledDaily);
            
            // Check blink
            Assert.AreEqual(9, target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Blink);

            target.SetInfoLineTextActivity(TaskId, RowId, Text, BlinkFalse, Align, Flags, Font, Scroll, Side, start, stop, IsScheduledDaily);
            Assert.AreEqual(0, target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Param.Blink);
        }

        /// <summary>
        /// A test for SetTaskForInfoLineText_BlinkTest
        /// </summary>
        [TestMethod]
        public void SetTaskForInfoLineText_TextEncodingTest()
        {
            var target = new QnetIqubeActivityMessage();
            const uint TaskId = 1;
            const byte RowId = 1;
            const string Text = "Test Rückseite";
            const bool BlinkTrue = true;
            const byte Align = 1;
            const sbyte Flags = 0;
            const sbyte Font = 0;
            const bool Scroll = true;
            const byte Side = 3;
            const int ExpectedLen = 14;
            var start = TimeProvider.Current.UtcNow;
            var stop = start.AddDays(1);
            const bool IsScheduledDaily = true;
            target.SetInfoLineTextActivity(TaskId, RowId, Text, BlinkTrue, Align, Flags, Font, Scroll, Side, start, stop, IsScheduledDaily);

            var actualText = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.InfoLine.Data.Values.Text;
            Assert.AreEqual(Text, actualText);
            Assert.IsTrue(actualText.Length == ExpectedLen, "The text lenght should be " + ExpectedLen.ToString());
        }

        /// <summary>
        /// A test for SetTaskForRevoke. It checks the taskId and TaskCode of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForRevokeTest()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 1;
            
            // ACT
            target.SetRevokeTask(ExpectedTaskId);

            // ASSERT
            var actualTaskId = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id;
            Assert.AreEqual(ExpectedTaskId, actualTaskId);
            var actualTaskType = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode;
            const sbyte ExpectedTaskType = (sbyte)IqubeTaskCode.ActivityRevoke;
            Assert.AreEqual(ExpectedTaskType, actualTaskType);
        }

        /// <summary>
        /// A test for SetTaskForDisplayOnOff. It checks the taskId and TaskCode of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForDisplayOnOffTest_OFF_TaskIdAndTaskCode()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const bool IsScheduledDaily = false;
            const bool DisplayOn = false;
            const DisplayMode DisplayMode = DisplayMode.AllOff;

            // ACT
            this.TargetSetDisplayOnOffActivity(target, ExpectedTaskId, DisplayOn, IsScheduledDaily, DisplayMode);

            // ASSERT
            var actualTaskId = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id;
            Assert.AreEqual(ExpectedTaskId, actualTaskId);

            var actualTaskType = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode;
            const sbyte ExpectedTaskType = (sbyte)IqubeTaskCode.ActivityDisplayOff;
            Assert.AreEqual(ExpectedTaskType, actualTaskType);
        }

        /// <summary>
        /// A test for SetTaskForDisplayOnOff. It checks the taskId and TaskCode of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForDisplayOnOffTest_ON_TaskiIdAndTaskCode()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const bool IsScheduledDaily = false;
            const bool DisplayOn = true;
            const DisplayMode DisplayMode = DisplayMode.Normal;

            // ACT
            this.TargetSetDisplayOnOffActivity(target, ExpectedTaskId, DisplayOn, IsScheduledDaily, DisplayMode);

            // ASSERT
            var actualTaskId = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id;
            Assert.AreEqual(ExpectedTaskId, actualTaskId);

            var actualTaskType = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode;
            const sbyte ExpectedTaskType = (sbyte)IqubeTaskCode.ActivityDisplayOn;
            Assert.AreEqual(ExpectedTaskType, actualTaskType);
        }

        /// <summary>
        /// A test for SetTaskForDisplayOnOff. It checks the default start and stop envent of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForDisplayOnOffTest_DefaultStartAndStopEvent()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const bool IsScheduledDaily = false;
            const bool DisplayOn = true;
            const DisplayMode DisplayMode = DisplayMode.Normal;

            // ACT
            this.TargetSetDisplayOnOffActivity(target, ExpectedTaskId, DisplayOn, IsScheduledDaily, DisplayMode);

            // ASSERT
            var actualStartEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent;
            const ExecutionCondition ExpectedStartEvent = ExecutionCondition.Immediately;
            Assert.AreEqual(ExpectedStartEvent, actualStartEvent);

            var actualStopEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent;
            const ExecutionCondition ExpectedStopEvent = ExecutionCondition.UntilAbort;
            Assert.AreEqual(ExpectedStopEvent, actualStopEvent);
        }

        /// <summary>
        /// A test for SetTaskForDisplayOnOff. It checks the default start and stop envent of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForDisplayOnOffTest_StartAndStopEventNotDaily_DateTimeShouldBeSet()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const bool DisplayOn = true;
            var start = TimeProvider.Current.UtcNow;
            var stop = start.AddDays(1).AddHours(1);
            const bool IsScheduledDaily = false;
            const ushort ExpectedStartDate = 6227; // start date from dos time
            const ushort ExpectedStartTime = 46538; // start time from dos time
            const DisplayMode DisplayMode = DisplayMode.Normal;
            const ushort ExpectedStopDate = 6228; // stop date from dos time
            const ushort ExpectedStopTime = 48586; // stop time from dos time
            // ACT
            target.SetDisplayOnOffActivity(
                ExpectedTaskId, DisplayOn, string.Empty, start, stop, IsScheduledDaily, DisplayMode, 0);

            // ASSERT
            var actualStartEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent;
            const ExecutionCondition ExpectedStartEvent = ExecutionCondition.DateTime;
            Assert.AreEqual(ExpectedStartEvent, actualStartEvent);

            var actualStopEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent;
            const ExecutionCondition ExpectedStopEvent = ExecutionCondition.DateTime;
            Assert.AreEqual(ExpectedStopEvent, actualStopEvent);

            var actualStartTime = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartTime;
            Assert.AreEqual(ExpectedStartDate, actualStartTime.Date, "Start date error.");
            Assert.AreEqual(ExpectedStartTime, actualStartTime.Time, "Start time error.");

            var actualEndTime = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.Time;
            Assert.AreEqual(ExpectedStopDate, actualEndTime.Date, "Stop date error.");
            Assert.AreEqual(ExpectedStopTime, actualEndTime.Time, "Stop time error.");
        }

        /// <summary>
        /// A test for SetTaskForDisplayOnOff. It checks the default start and stop envent of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForDisplayOnOffTest_StartAndStopEventDaily_DaySecsShouldBeSet()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const bool DisplayOn = true;
            var start = new DateTime(2012, 07, 23, 7, 12, 43);
            var stop = start.AddDays(1).AddHours(1).AddMinutes(10);
            const bool IsScheduledDaily = true;
            const DisplayMode DisplayMode = DisplayMode.Normal;
            const uint ExpectedDaySecStart = 25963;
            const uint ExpectedDaySecStop = 30163;

            // ACT
            target.SetDisplayOnOffActivity(ExpectedTaskId, DisplayOn, string.Empty, start, stop, IsScheduledDaily, DisplayMode, 0);

            // ASSERT
            var actualStartEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent;
            const ExecutionCondition ExpectedStartEvent = ExecutionCondition.Daily;
            Assert.AreEqual(ExpectedStartEvent, actualStartEvent);

            var actualStopEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent;
            const ExecutionCondition ExpectedStopEvent = ExecutionCondition.Daily;
            Assert.AreEqual(ExpectedStopEvent, actualStopEvent);

            var actualStartDaySecs = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartDaySeconds;
            Assert.AreEqual(ExpectedDaySecStart, actualStartDaySecs, "Start StartDaySeconds error.");

            var actualEndDaySecs = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.DaySecs;
            Assert.AreEqual(ExpectedDaySecStop, actualEndDaySecs, "Stop day secs error.");
        }

        /// <summary>
        /// A test for SetTaskForDisplayOnOff for daily start stop condition.
        /// </summary>
        [TestMethod]
        public void SetTaskForDisplayOnOffTest_StartAndStopEventDaily_DateTimeShouldNotBeSet()
        {
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const bool DisplayOn = true;
            var start = TimeProvider.Current.UtcNow;
            var stop = start.AddDays(1).AddHours(1);
            const bool IsScheduledDaily = true;
            const ushort ExpectedStartDate = 6227; // start date from dos time
            const ushort ExpectedStartTime = 46538; // start time from dos time
            const DisplayMode DisplayMode = DisplayMode.Normal;
            const ushort ExpectedStopDate = 6228; // stop date from dos time
            const ushort ExpectedStopTime = 48586; // stop time from dos time

            const uint ExpectedStartDaySecs = 81980;
            const uint ExpectedStopDaySecs = 85580;

            // ACT
            target.SetDisplayOnOffActivity(ExpectedTaskId, DisplayOn, string.Empty, start, stop, IsScheduledDaily, DisplayMode, 0);

            // ASSERT
            var actualStartEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StartEvent;
            const ExecutionCondition ExpectedStartEvent = ExecutionCondition.Daily;
            Assert.AreEqual(ExpectedStartEvent, actualStartEvent);

            var actualStopEvent = (ExecutionCondition)target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.StopEvent;
            const ExecutionCondition ExpectedStopEvent = ExecutionCondition.Daily;
            Assert.AreEqual(ExpectedStopEvent, actualStopEvent);

            var actualStartTime = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartTime;
            Assert.AreNotEqual(ExpectedStartDate, actualStartTime.Date, "Start date error.");
            Assert.AreNotEqual(ExpectedStartTime, actualStartTime.Time, "Start time error.");

            var actualStartDaySecs =
                target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Start.StartDaySeconds;
            Assert.AreEqual(ExpectedStartDaySecs, actualStartDaySecs, "Start DaySecs should be set instead of start time");
            var actualEndTime = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.Time;
            Assert.AreNotEqual(ExpectedStopDate, actualEndTime.Date, "Stop date error.");
            Assert.AreNotEqual(ExpectedStopTime, actualEndTime.Time, "Stop time error.");

            var actualStopDaySecs =
                target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.ExeCond.Stop.DaySecs;
            Assert.AreEqual(ExpectedStopDaySecs, actualStopDaySecs, "Stop DaySecs should be set instead of start time");
        }      

        /// <summary>
        /// A test for SetTaskForDisplayOnOff. It checks the taskId and TaskCode of the generated message.
        /// </summary>
        [TestMethod]
        public void SetTaskForVoiceTextTest_CreateObject_TaskIdAndTaskCode()
        {
            // ARRANGE
            var target = new QnetIqubeActivityMessage();
            const uint ExpectedTaskId = 2010;
            const ushort ExpectedInterval = 15;
            const bool IsScheduledDaily = false;
            const sbyte ExpectedTaskType = (sbyte)IqubeTaskCode.ActivityVoiceText;
            var expectedByteText = new byte[MessageConstantes.MaxVoiceTextLength];
            const string ExpectedText = "aaabbbccc";

            int index = 0;
            foreach (var b in Encoding.Default.GetBytes(ExpectedText))
            {
                expectedByteText[index] = b;
                index++;
            }

            var execSchedule = new ExecutionScheduleContext { IsScheduledDaily = IsScheduledDaily };

            // ACT
            target.SetVoiceTextActivity(ExpectedTaskId, "aaabbbccc", ExpectedInterval, execSchedule);
            
            // ASSERT
            var actualTaskId = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.Id;
            Assert.AreEqual(ExpectedTaskId, actualTaskId);

            var actualTaskType = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.Header.TaskCode;            
            Assert.AreEqual(ExpectedTaskType, actualTaskType);

            var actualtext = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.VoiceText.Text;
            Assert.AreEqual(ExpectedText, actualtext);

            var actualInterval = target.IqubeActivityMessage.Dta.IqubeCmdMsg.Task.VoiceText.Interval;
            Assert.AreEqual(ExpectedInterval, actualInterval);
        }
        
        // ReSharper restore InconsistentNaming
        private void TargetSetDisplayOnOffActivity(QnetIqubeActivityMessage cmd, uint taskId, bool displayOn, bool isDaily, DisplayMode displayMode)
        {
            cmd.SetDisplayOnOffActivity(taskId, displayOn, string.Empty, null, null, isDaily, displayMode, 0);
        }
    }
}