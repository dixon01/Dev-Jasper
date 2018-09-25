// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Alarms
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The alarm service that logs alarms from different parts of the application.
    /// </summary>
    public class AlarmService
    {
        private static readonly Logger Logger = LogHelper.GetLogger<AlarmService>();

        /// <summary>
        /// Starts this service.
        /// </summary>
        public void Start()
        {
            Logger.Info("Start AlarmService Medi Subscribe to <Alarm> messages");
            MessageDispatcher.Instance.Subscribe<Alarm>(this.HandleAlarm);
        }

        /// <summary>
        /// Stops this service.
        /// </summary>
        public void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<Alarm>(this.HandleAlarm);
        }

        private void HandleAlarm(object sender, MessageEventArgs<Alarm> e)
        {
            var alarm = e.Message;
            if (alarm.Unit != ApplicationHelper.MachineName)
            {
                // ignore alarms that are not from our unit (they are broadcast in the entire Medi network)
                return;
            }

            var message = string.IsNullOrEmpty(alarm.Message) ? string.Empty : alarm.Message.Replace("\"", "\"\"");
            LogLevel level;
            switch (alarm.Severity)
            {
                case AlarmSeverity.Critical:
                    level = LogLevel.Fatal;
                    break;
                case AlarmSeverity.Severe:
                    level = LogLevel.Fatal;
                    break;
                case AlarmSeverity.Error:
                    level = LogLevel.Error;
                    break;
                case AlarmSeverity.Warning:
                    level = LogLevel.Warn;
                    break;
                default:
                    level = LogLevel.Info;
                    break;
            }

            Logger.Log(level, "{0};{1};{2};\"{3}\"", alarm.Category, alarm.Type, alarm.GetAttributeText(), message);
        }
    }
}
